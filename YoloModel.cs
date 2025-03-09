using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCam
{
    public class YoloModel
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int Dimensions { get; set; }
      
        public float Confidence { get; set; }
        public float MulConfidence { get; set; }
        public float Overlap { get; set; }

        public string[] Outputs { get; set; }
        public List<YoloLabeling> Labels { get; set; }
        public bool UseDetect { get; set; }
    }
}
