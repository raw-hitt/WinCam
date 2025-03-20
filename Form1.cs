using System;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using WinCam.Yolo;


namespace WinCam
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;  // List of available cameras
        private VideoCaptureDevice videoSource;     // Selected camera
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No camera found!");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
            videoSource.Start();
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();

        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            StopCamera();
        }

        private void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox1.Image = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Save("capture_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                MessageBox.Show("Image saved!");
            }
        }

        private void btnCapture_Click_1(object sender, EventArgs e)
        {
            btnDetect.Text = "Detecting...";
            CaptureAndDetect();
            btnDetect.Text = "Detect";
        }
        private void CaptureAndDetect()
        {
            lblDetails.Text = "";
            Image _latestImg = pictureBox1.Image;
            StopCamera();
            pictureBox1.Image = _latestImg;

            YoloDetect pi = new YoloDetect();

            var output = pi.ProcessImages((Bitmap)_latestImg);


            int score = Convert.ToInt32((output.Item2[0].Score) * 100);

            if (output.Item1 == null || score <= 35)
            {
                lblDetails.Text = "No objects detected in the current image";
                //pictureBox1.Image = (Bitmap)_latestImg;
            }
            else
            {
                pictureBox1.Image = output.Item1;

                try
                {
                    string label = output.Item2[0].Label.Name;
                    lblDetails.Text = "Detection Score = " + score.ToString() + " and label - " + label;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected Error");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
