using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FractalTraveler
{
    public partial class MandelbrotForm : Form
    {
        public double wx = 0, wy = 0; //коорд смещ
        public double speed = 2f, zoom = 2f, zoomSpeed = 0.001d;
        public int res = 5; //разрешение для изменения качества и скорости вычисления

        private void Form1_KeyDown(object sender, KeyEventArgs e) //управление
        {
            //разрешение
            if (e.KeyCode == Keys.Q)
            {
                res -= 1;
            }
            if (e.KeyCode == Keys.E)
            {
                res += 1;
            }
            
            //зум
            if (e.KeyCode == Keys.PageUp)
            {
                zoom -= zoomSpeed / zoom;
                Draw();
            }
            if (e.KeyCode == Keys.PageDown)
            {
                zoom += zoomSpeed / zoom;
                Draw();
            }

            //перемещение
            if (e.KeyCode == Keys.Up)
            {
                wy -= speed * (5 - Math.Abs(zoom));
            }
            if (e.KeyCode == Keys.Down)
            {
                wy += speed * (5 - Math.Abs(zoom));
            }

            if (e.KeyCode == Keys.Left)
            {
                wx -= speed * (5 - Math.Abs(zoom));
            }
            if (e.KeyCode == Keys.Right)
            {
                wx += speed * (5 - Math.Abs(zoom));
            }

            //
            if (e.KeyCode == Keys.Space)
            {
                timer1.Stop();
                Draw();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            zoom -= zoomSpeed / zoom;
            Draw();
        }

    
        public MandelbrotForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Draw();
        }


        public void Draw()
        {
            if (res <= 0)
            {
                res = 1;
            }

            Bitmap frame = new Bitmap(Width / res, Height / res);
            for (int x = 0; x < Width/res; x++)
            {
                for (int y = 0; y < Height/res; y++)
                {
                    double a = (double)((x + (wx / res / zoom)) - ((Width / 2d) / res)) / (double)(Width / zoom / res / 1.777f); // 16/9 = 1.77 - разрешение
                    double b = (double)((y + (wy / res / zoom)) - ((Height / 2d) / res)) / (double)(Height / zoom / res);

                    ComplexNum c = new ComplexNum(a, b); //указ коорд
                    ComplexNum z = new ComplexNum(0, 0); //z - отображение


                    int it = 0;
                    
                    //цикл отрисовки до магнитуды в 2 или в 100 итераций 
                    do
                    {
                        it++;
                        z.Sqr();
                        z.Add(c);
                        if (z.Magn() > 2.0d)
                        {
                            break;
                        }
                    } while (it < 100);


                    frame.SetPixel(x, y, Color.FromArgb((byte)(it * 2.55f), (byte)(it * 2.55f), (byte)(it * 2.55f)));
                }
            }

            pictureBox1.Image = frame;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }



    public class ComplexNum {
        public double a;
        public double b;

        public ComplexNum(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public void Sqr()
        {
            double tmp = (a * a) - (b * b);
            b = 2.0d * a * b;
            a = tmp;
        }

        public double Magn()
        {
            return Math.Sqrt((a * a) + (b * b));
        }

        public void Add(ComplexNum c)
        {
            a += c.a;
            b += c.b;
        }
        
    }

}
