namespace WinCam
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStop;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            btnStop = new Button();
            btnDetect = new Button();
            lblDetails = new Label();
            btnStart = new Button();
            btnCapture = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(15, 19);
            pictureBox1.Margin = new Padding(4, 5, 4, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 749);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(148, 781);
            btnStop.Margin = new Padding(4, 5, 4, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(125, 62);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop Camera";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnDetect
            // 
            btnDetect.Location = new Point(148, 853);
            btnDetect.Margin = new Padding(4, 5, 4, 5);
            btnDetect.Name = "btnDetect";
            btnDetect.Size = new Size(125, 62);
            btnDetect.TabIndex = 3;
            btnDetect.Text = "Detect";
            btnDetect.UseVisualStyleBackColor = true;
            btnDetect.Click += btnCapture_Click_1;
            // 
            // lblDetails
            // 
            lblDetails.AutoSize = true;
            lblDetails.Location = new Point(345, 800);
            lblDetails.Name = "lblDetails";
            lblDetails.Size = new Size(33, 25);
            lblDetails.TabIndex = 4;
            lblDetails.Text = "---";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(15, 781);
            btnStart.Margin = new Padding(4, 5, 4, 5);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(125, 62);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start Camera";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnCapture
            // 
            btnCapture.Location = new Point(15, 853);
            btnCapture.Margin = new Padding(4, 5, 4, 5);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new Size(125, 62);
            btnCapture.TabIndex = 5;
            btnCapture.Text = "Capture";
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(838, 1050);
            Controls.Add(btnCapture);
            Controls.Add(lblDetails);
            Controls.Add(btnDetect);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(pictureBox1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Camera App";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load_1;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnDetect;
        private Label lblDetails;
        private Button btnStart;
        private Button btnCapture;
    }
}
