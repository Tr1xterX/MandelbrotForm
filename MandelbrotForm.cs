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
        public double speed = 2f, zoom = 2f, zoomSpeed = 0.005d;
        public int res = 5; //разрешение для изменения качества и скорости вычисления

        private async void Form1_KeyDown(object sender, KeyEventArgs e) //управление
        {
            //разрешение
            if (e.KeyCode == Keys.Q)
            {
                if (res <= 1)
                {
                    res = 1;
                }
                else res -= 1;
            }
            if (e.KeyCode == Keys.E)
            {
                res += 1;
            }

            //зум
            if (e.KeyCode == Keys.PageUp)
            {
                zoom -= zoomSpeed / zoom;
                await DrawAsync();
            }
            if (e.KeyCode == Keys.PageDown)
            {
                zoom += zoomSpeed / zoom;
                await DrawAsync();
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
                await DrawAsync();
            }
        }

        public MandelbrotForm()
        {
            InitializeComponent();
            
        }
        private void UpdateParametersLabel()
        {
            parametersLabel.Text = $"Zoom: {zoom}\n" +
                                   $"Zoom Speed: {zoomSpeed}\n" +                     
                                   $"Resolution: {res}\n" +
                                   $"Wx: {wx}\n" +
                                   $"Wy: {wy}";
        }

        private async void Timer1_TickAsync(object sender, EventArgs e)
        {
            zoom -= zoomSpeed / zoom;
            await DrawAsync();
            UpdateParametersLabel();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await DrawAsync();
        }

        public async Task DrawAsync()
        {
            int width = Width / res;
            int height = Height / res;

            if (res <= 0)
            {
                res = 1;
            }

            Bitmap frame = new Bitmap(width, height);
            Color[,] pixels = new Color[width, height];

            await Task.Run(() =>
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        double a = (double)((x + (wx / res / zoom)) - ((Width / 2d) / res)) / (double)(Width / zoom / res / 1.777f); // 16/9 = 1.77 - разрешение
                        double b = (double)((y + (wy / res / zoom)) - ((Height / 2d) / res)) / (double)(Height / zoom / res);

                        ComplexNum c = new ComplexNum(a, b); //указ коорд
                        ComplexNum z = new ComplexNum(0, 0); //z - отображение

                        int it = 0;

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

                        pixels[x, y] = Color.FromArgb((byte)(it * 2.55f), (byte)(it * 2.55f), (byte)(it * 2.55f));
                    }
                });
            });

            // Применяем массив пикселей к объекту frame
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    frame.SetPixel(x, y, pixels[x, y]);
                }
            }

            pictureBox1.Image = frame;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }


        public class ComplexNum
        {
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

        /*
        public class ComplexNumber
        {
            public double x;
            public double y;

            public ComplexNumber(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public static ComplexNumber operator+(ComplexNumber a, ComplexNumber b)
            {
                var temp = new ComplexNumber(0, 0)
                {
                    x = a.x + b.x,
                    y = a.y + b.y
                };
                return temp;
            }

            public static ComplexNumber operator*(ComplexNumber a, ComplexNumber b)
            {
                var temp = new ComplexNumber(0, 0)
                {
                    x = (a.x * b.x) - a.y * b.y,
                    y = (a.x * b.y) + a.y * b.x
                };
                return temp;
            }
        }
        */
    }
}
