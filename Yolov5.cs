using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace WinCam
{
    public class Yolov5 : IDisposable
    {
        private readonly InferenceSession _inferenceSession;
        private YoloModel _model = new YoloModel();

        public Yolov5(string ModelPath, bool useCuda = false)
        {
            try
            {

                if (useCuda)
                {
                    Microsoft.ML.OnnxRuntime.SessionOptions opts = Microsoft.ML.OnnxRuntime.SessionOptions.MakeSessionOptionWithCudaProvider();
                    _inferenceSession = new InferenceSession(ModelPath, opts);
                }
                else
                {
                    Microsoft.ML.OnnxRuntime.SessionOptions opts = new Microsoft.ML.OnnxRuntime.SessionOptions();
                    _inferenceSession = new InferenceSession(ModelPath, opts);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



            /// Get model info
            get_input_details();
            get_output_details();
        }

        public void SetupLabels(string[] labels)
        {
            try
            {
                int i = 0;
                _model.Labels = new List<YoloLabeling>();
                foreach (string label in labels)
                {
                    i++;
                    YoloLabeling ybl = new YoloLabeling() { Id = i, Name = label };

                    _model.Labels.Add(ybl);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

       

        public List<YoloPrediction> Predict(Image image, float conf_thres = 0, float iou_thres = 0)
        {
            if (conf_thres > 0f)
            {
                _model.Confidence = conf_thres;
                _model.MulConfidence = conf_thres + 0.05f;
            }
            if (iou_thres > 0f)
            {
                _model.Overlap = iou_thres;
            }
            return Supress(ParseOutput(Inference(image), image));
        }

        /// <summary>
        /// Removes overlaped duplicates (nms).
        /// </summary>
        private List<YoloPrediction> Supress(List<YoloPrediction> items)
        {
            var result = new List<YoloPrediction>(items);

            foreach (var item in items) // iterate every prediction
            {
                foreach (var current in result.ToList()) // make a copy for each iteration
                {
                    if (current == item) continue;

                    var (rect1, rect2) = (item.Rectangle, current.Rectangle);

                    System.Drawing.RectangleF intersection = RectangleF.Intersect(rect1, rect2);

                    float intArea = intersection.Width*intersection.Height; // intersection area
                    float unionArea = (rect1.Width * rect1.Height) + (rect2.Width+rect2.Height) - intArea; // union area
                    float overlap = intArea / unionArea; // overlap ratio

                    if (overlap >= _model.Overlap)
                    {
                        if (item.Score >= current.Score)
                        {
                            result.Remove(current);
                        }
                    }
                }
            }

            return result;
        }

        private DenseTensor<float>[] Inference(Image img)
        {
            Bitmap resized = null;

            if (img.Width != _model.Width || img.Height != _model.Height)
            {
                resized = YoloUtils.ResizeImage(img, _model.Width, _model.Height); // fit image size to specified input size
            }
            else
            {
                resized = new Bitmap(img);
            }

            var inputs = new List<NamedOnnxValue> // add image as onnx input
            {
                NamedOnnxValue.CreateFromTensor("images", YoloUtils.ExtractPixels(resized))
            };

            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> result = _inferenceSession.Run(inputs); // run inference

            var output = new List<DenseTensor<float>>();

            foreach (var item in _model.Outputs) // add outputs for processing
            {
                output.Add(result.First(x => x.Name == item).Value as DenseTensor<float>);
            };

            return output.ToArray();
        }

        private List<YoloPrediction> ParseOutput(DenseTensor<float>[] output, Image image)
        {
            return _model.UseDetect ? ParseDetect(output[0], image) : ParseSigmoid(output, image);
        }

        private List<YoloPrediction> ParseDetect(DenseTensor<float> output, Image image)
        {
            var result = new ConcurrentBag<YoloPrediction>();

            var (w, h) = (image.Width, image.Height); // image w and h
            var (xGain, yGain) = (_model.Width / (float)w, _model.Height / (float)h); // x, y gains
            var gain = Math.Min(xGain, yGain); // gain = resized / original

            var (xPad, yPad) = ((_model.Width - w * gain) / 2, (_model.Height - h * gain) / 2); // left, right pads

            Parallel.For(0, (int)output.Length / _model.Dimensions, (i) =>
            {
                if (output[0, i, 4] <= _model.Confidence) return; // skip low obj_conf results

                Parallel.For(5, _model.Dimensions, (j) =>
                {
                    output[0, i, j] = output[0, i, j] * output[0, i, 4]; // mul_conf = obj_conf * cls_conf
                });

                Parallel.For(5, _model.Dimensions, (k) =>
                {
                    if (output[0, i, k] <= _model.MulConfidence) return; // skip low mul_conf results

                    float xMin = ((output[0, i, 0] - output[0, i, 2] / 2) - xPad) / gain; // unpad bbox tlx to original
                    float yMin = ((output[0, i, 1] - output[0, i, 3] / 2) - yPad) / gain; // unpad bbox tly to original
                    float xMax = ((output[0, i, 0] + output[0, i, 2] / 2) - xPad) / gain; // unpad bbox brx to original
                    float yMax = ((output[0, i, 1] + output[0, i, 3] / 2) - yPad) / gain; // unpad bbox bry to original

                    xMin = YoloUtils.Clamp(xMin, 0, w - 0); // clip bbox tlx to boundaries
                    yMin = YoloUtils.Clamp(yMin, 0, h - 0); // clip bbox tly to boundaries
                    xMax = YoloUtils.Clamp(xMax, 0, w - 1); // clip bbox brx to boundaries
                    yMax = YoloUtils.Clamp(yMax, 0, h - 1); // clip bbox bry to boundaries

                    YoloLabeling label = _model.Labels[k - 5];

                    var prediction = new YoloPrediction(label, output[0, i, k])
                    {
                        Rectangle = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin)
                    };

                    result.Add(prediction);
                });
            });

            return result.ToList();
        }

        private List<YoloPrediction> ParseSigmoid(DenseTensor<float>[] output, Image image)
        {
            return new List<YoloPrediction>();
        }

        private void prepare_input(Image img)
        {
            Bitmap bmp = YoloUtils.ResizeImage(img, _model.Width, _model.Height);

        }

        private void get_input_details()
        {
            _model.Height = _inferenceSession.InputMetadata["images"].Dimensions[2];
            _model.Width = _inferenceSession.InputMetadata["images"].Dimensions[3];
        }

        private void get_output_details()
        {
            _model.Outputs = _inferenceSession.OutputMetadata.Keys.ToArray();
            _model.Dimensions = _inferenceSession.OutputMetadata[_model.Outputs[0]].Dimensions[2];
            _model.UseDetect = !(_model.Outputs.Any(x => x == "score"));
        }

        public void Dispose()
        {
            _inferenceSession.Dispose();
        }
    }
}
