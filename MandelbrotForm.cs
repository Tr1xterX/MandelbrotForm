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
        public double step = 2f, zoom = 2f, zoomStep = 0.1d; 
        public int res = 3; //разрешение для изменения качества и скорости вычисления

        private async void Form1_KeyDown(object sender, KeyEventArgs e) //управление
        {

            //разрешение
            if (e.KeyCode == Keys.R)
            {
                if (res <= 1)
                {
                    res = 1;
                    await DrawAsync();
                }
                else { res -= 1; await DrawAsync(); }
            }
            if (e.KeyCode == Keys.T)
            {
                res += 1;
            }

            //Zoom
            if (e.KeyCode == Keys.E)
            {
                double newZoom = zoom - zoomStep;
                if (newZoom > 0) // Проверяем, чтобы новое значение zoom было положительным
                {
                    zoom = newZoom;
                    zoomStep = CalculateZoomStep(zoom);
                    await DrawAsync();
                }
            }
            if (e.KeyCode == Keys.Q)
            {
                zoom += zoomStep;
                zoomStep = CalculateZoomStep(zoom);
                await DrawAsync();
            }

            //перемещение
            if (e.KeyCode == Keys.W)
            {
                wy -= step * (Math.Abs(zoom)*3);
                await DrawAsync();
            }
            if (e.KeyCode == Keys.S)
            {
                wy += step * (Math.Abs(zoom)*3);
                await DrawAsync();
            }

            if (e.KeyCode == Keys.A)
            {
                wx -= step * (Math.Abs(zoom)*3);
                await DrawAsync();
            }
            if (e.KeyCode == Keys.D)
            {
                wx += step * (Math.Abs(zoom)*3);
                await DrawAsync();
            }

            UpdateParametersLabel();
        }

        // Метод для вычисления нового zoomStep с использованием логарифмической зависимости
        private double CalculateZoomStep(double zoom)
        {
            double minZoomStep = 0.0001;
            double maxZoomStep = 0.1;

            // Масштабирование zoom для приведения его к более удобному диапазону значений
            double scaledZoom = Math.Log(zoom + 1);

            // Определение нового значения zoomStep на основе логарифмической зависимости от scaledZoom
            double newZoomStep = minZoomStep + (maxZoomStep - minZoomStep) * (1 - Math.Pow(Math.E, -scaledZoom));

            return newZoomStep;
            //натуральный логарифм Math.Log, медленнее изменять scaledZoom с увеличением zoom
        }

        private async void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                zoom -= zoomStep;
                await DrawAsync();
            }
            else if (e.Button == MouseButtons.Right)
            {
                zoom += zoomStep;
                await DrawAsync();
            }
        }

        public MandelbrotForm()
        {
            InitializeComponent();
            this.MouseDown += Form1_MouseDown;

        }
        private void UpdateParametersLabel()
        {
            parametersLabel.Text = $"Zoom: {zoom}\n" +
                                   $"Zoom Step: {zoomStep}\n" +
                                   $"Resolution: {res}\n" +
                                   $"X: {wx}\n" +
                                   $"Y: {wy}\n" +
                                   $"Step: {step}\n"+
                                   $"CalculateZoomStep: {CalculateZoomStep(zoom)}\n";
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await DrawAsync();
            UpdateParametersLabel();
        }

        public async Task DrawAsync()
        {
            // Проверяем, была ли нажата клавиша управления
           

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
