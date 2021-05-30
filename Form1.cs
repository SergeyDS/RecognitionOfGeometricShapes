using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.Util;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;


namespace RecognitionOfGeometricShapes
{
    
    
    public partial class Form1 : Form
    {
        // Распознаем геометрические фигуры 
        private Image<Bgr, byte> inputImage = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    inputImage = new Image<Bgr, byte>(openFileDialog1.FileName);
                    pictureBox1.Image = inputImage.Bitmap;


                }
                else
                {
                    MessageBox.Show("Изображение не выбрано ! ", "Ошибка !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void найтиФигурыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //88
            Image<Gray, byte> grayImage = inputImage.SmoothGaussian(5).Convert<Gray,byte>().ThresholdBinaryInv(new Gray(230),new Gray(255));
            VectorOfVectorOfPoint conturs = new VectorOfVectorOfPoint();

            Mat hierarchy = new Mat();

            CvInvoke.FindContours(grayImage, conturs, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < conturs.Size; i++)
            {
                double perimetr = CvInvoke.ArcLength(conturs[i],true);
                VectorOfPoint approximation = new VectorOfPoint();

                CvInvoke.ApproxPolyDP(conturs[i], approximation, 0.04 * perimetr ,true);
                CvInvoke.DrawContours(inputImage,conturs,i,new MCvScalar(0,0,255),2);


                //Moments moments = CvInvoke.Moments(conturs[i]);
                MCvMoments moments = CvInvoke.Moments(conturs[i]);

                int x = (int)(moments.M10 / moments.M00);
                int y = (int)(moments.M01 / moments.M00);

                if (approximation.Size==3)
                {
                    CvInvoke.PutText(inputImage,"Trangle",new Point(x,y),Emgu.CV.CvEnum.FontFace.HersheyPlain,1,new MCvScalar(0,0,0),1);

                }

                if (approximation.Size==4)
                {
                    Rectangle rect = CvInvoke.BoundingRectangle(conturs[i]);
                    double aspectRatio = (double)rect.Width / (double)rect.Height;

                    if (aspectRatio >= 0.95 && aspectRatio <= 1.05)
                    {
                        CvInvoke.PutText(inputImage, "Square", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(0, 0, 0), 1);
                    }
                    else
                    {
                        CvInvoke.PutText(inputImage, "Rectangle", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(0, 0, 0), 1);
                    }
                }

                if (approximation.Size == 5)
                {
                    CvInvoke.PutText(inputImage, "Pentagon", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(0, 0, 0), 1);

                }
                if (approximation.Size == 6)
                {
                    CvInvoke.PutText(inputImage, "Hexagon", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(0, 0, 0), 1);

                }

                if (approximation.Size > 6)
                {
                    CvInvoke.PutText(inputImage, "Circle", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(0, 0, 0), 1);

                }

                

                

                pictureBox2.Image = inputImage.Bitmap;
            }
        }
    }
}
