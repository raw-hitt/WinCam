using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCam
{
    internal class YoloDetect
    {
        private Yolov5 yolo;

        public YoloDetect()
        {
            yolo = new Yolov5(AppDomain.CurrentDomain.BaseDirectory + "best.onnx");
            yolo.SetupLabels(new string[] { "Clubs", "Dimonds", "Spades", "hearts" });
        }

        internal Tuple<Image, List<YoloPrediction>> ProcessImages(System.Drawing.Image image)
        {

            List<YoloPrediction> predictions = yolo.Predict(image);


            var graphics = Graphics.FromImage(image);

            foreach (var prediction in predictions.OrderByDescending(x=>x.Score)) // iterate predictions to draw results
            {
                double score = Math.Round(prediction.Score, 2);

                graphics.DrawRectangles(new Pen(prediction.Label.Color, 1),
                    new[] { prediction.Rectangle });

                var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

                graphics.DrawString($"{prediction.Label.Name} ({score})",
                    new Font("Consolas", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color),
                    new PointF(x, y));
            }

            return Tuple.Create(image, predictions);
        }

    }

  
}
