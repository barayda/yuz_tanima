namespace yüztanıma
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Emgu.CV;
    using Emgu.CV.Structure;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Util;
    public partial class Form1 : Form
    {   
        private VideoCapture _capture;

        private CascadeClassifier _haarCascade;

        public Form1()
        {
            InitializeComponent(); 

            try
            {
                _haarCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Haar Cascade dosyası yüklenemedi: " + ex.Message);
                return;
            }

            try
            {
            
                _capture = new VideoCapture(0);
                if (!_capture.IsOpened)
                {
                    throw new Exception("Kamera açılamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kamera başlatılamadı: " + ex.Message);
                return;
            }

            Application.Idle += FrameIslem;
        }


        private void FrameIslem(object sender, EventArgs e)
        {
        
            Mat frame = _capture.QueryFrame();

            if (frame != null)
            {
                using (Image<Bgr, byte> imageFrame = frame.ToImage<Bgr, byte>())
                {
                    using (Image<Gray, byte> grayFrame = imageFrame.Convert<Gray, byte>())
                    {
                
                        Rectangle[] faces = _haarCascade.DetectMultiScale(
                            grayFrame,
                            1.1, 
                            10,  
                            new Size(20, 20),
                            Size.Empty 
                        );

                        foreach (Rectangle face in faces)
                        {
                            imageFrame.Draw(face, new Bgr(Color.Red), 2); 
                        }
                    } 

                    picture.Image = imageFrame.ToBitmap();
                } 

                frame.Dispose();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_capture != null)
            {
                _capture.Dispose();
            }
            if (_haarCascade != null)
            {
                _haarCascade.Dispose();
            }
            Application.Idle -= FrameIslem;
        }
    }
}
