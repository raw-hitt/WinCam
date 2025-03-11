using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCam.Models
{
    public class YoloPrediction
    {
        public YoloLabeling Label { get; set; }
        public RectangleF Rectangle { get; set; }
        public float Score { get; set; }


        public YoloPrediction(YoloLabeling label, float confidence) : this(label)
        {
            Score = confidence;
        }

        public YoloPrediction(YoloLabeling label)
        {
            Label = label;
        }
    }
}
