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
        // Video yakalama nesnesi
        private VideoCapture _capture;
        // Yüz tespiti için Cascade Classifier (Haar Sınıflandırıcı)
        private CascadeClassifier _haarCascade;

        public Form1()
        {
            InitializeComponent(); // Windows Forms başlatma metodu

            // 1. Haar Cascade Classifier'ı yükleme
            // "haarcascade_frontalface_default.xml" dosyasının uygulamanızın çalıştığı dizinde olması gerekir.
            // Bu dosya Emgu CV kurulumunda mevcuttur.
            try
            {
                _haarCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Haar Cascade dosyası yüklenemedi: " + ex.Message);
                return;
            }

            // 2. Video Yakalamayı Başlatma
            try
            {
                // 0, genellikle varsayılan kamerayı temsil eder
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

            // 3. Frame yakalama olayını ayarlama
            // Her frame (çerçeve) yakalandığında FrameIslem metodu çalışacak
            Application.Idle += FrameIslem;
        }


        private void FrameIslem(object sender, EventArgs e)
        {
            // 1. Kameradan yeni frame'i (çerçeveyi) al
            Mat frame = _capture.QueryFrame();

            if (frame != null)
            {
                // **!!! ÖNEMLİ DEĞİŞİKLİK BURADA BAŞLIYOR !!!**
                // Mat nesnesini Bgr renk uzayında bir Image nesnesine dönüştür
                // ve ona 'imageFrame' adını ver. Bu değişkeni using bloğu içinde tanımla.
                using (Image<Bgr, byte> imageFrame = frame.ToImage<Bgr, byte>())
                {
                    // 1.1. Yüz tespiti için Gri Tonlamalı Image nesnesi oluştur
                    using (Image<Gray, byte> grayFrame = imageFrame.Convert<Gray, byte>())
                    {
                        // 2. Yüzleri tespit et
                        Rectangle[] faces = _haarCascade.DetectMultiScale(
                            grayFrame,
                            1.1, // Scale Factor
                            10,  // Minimum Neighbors
                            new Size(20, 20), // Minimum tespit boyutu
                            Size.Empty // Maksimum tespit boyutu
                        );

                        // 3. Tespit edilen her yüzün etrafına dikdörtgen çiz
                        // Çizim işlemi 'imageFrame' üzerinde yapılmalıdır!
                        foreach (Rectangle face in faces)
                        {
                            imageFrame.Draw(face, new Bgr(Color.Red), 2); // Kırmızı renkte dikdörtgen
                        }
                    } // grayFrame burada otomatik dispose edilir

                    // 4. İşlenmiş frame'i PictureBox'ta göster
                    // 'imageFrame' şu an tanımlıdır ve Bitmap özelliğini kullanabiliriz.
                    picture.Image = imageFrame.ToBitmap();
                } // imageFrame burada otomatik dispose edilir

                // 5. Mat nesnesini dispose et
                frame.Dispose();
            }
        }
        // Uygulama kapanırken kaynakları serbest bırak
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
