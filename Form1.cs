using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace GSC_Lab1
{
    public partial class Form1 : Form
    {
        Graphics g;
        Pen DrawPen = new Pen(Color.Black, 1);

        // Код типа закрашивания, где
        // 0 - как неоринетированного
        // 1 - как ориентированного
        short paintType = 0;

        //Код типа вывода
        //0 - с отрисвкой граничного мноугольника
        //1 - без отрисовки граничного многоугольника
        short outputType = 0;

        List<Point> VertexList = new List<Point>();        
        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics(); //инициализация графики
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        static bool detectCW(Point A, Point B, Point C)
        {
            double square_triangle = 0.5 * (A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y));

            if (square_triangle < 0) { return true; } 
            else { return false; }
        }

        private void comboBox1_SelectType(object sender, EventArgs e)
        {
            paintType = (short)comboBox1.SelectedIndex;
        }

        private void comboBox2_SelectColor(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex) // выбор цвета 
            {
                case 0:
                    DrawPen.Color = Color.Black;
                    break;
                case 1:
                    DrawPen.Color = Color.Red;
                    break;
                case 2:
                    DrawPen.Color = Color.Green;
                    break;
                case 3:
                    DrawPen.Color = Color.Blue;
                    break;
            }
        }

        private void comboBox3_SelectOutType(object sender, EventArgs e)
        {
            outputType = (short)comboBox3.SelectedIndex;
            g.Clear(Color.White);
            VertexList.Clear();
        }

        public void PaintFigure(Pen DrPen)
        {

            int Ymin = VertexList.Min(point => point.Y);
            int Ymax = VertexList.Max(point => point.Y);

            Ymin = Math.Max(Ymin, 0);
            Ymax = Math.Min(Ymax, 480);

           


            

            //отрисовка как для неориентированного многоугольника
            if (paintType== 0) {

                List<int> xPoints = new List<int>();

                int nextVert = 0;

                //две смежные точки
                Point firPoint = new Point();
                Point secPoint = new Point();

                for (int y = Ymin; y <= Ymax; y++)
                {
                    //очистка списка х координат точек пересечения строки со сторонами многоугольника
                    xPoints.Clear();

                    for (int vertNum = 0; vertNum < VertexList.Count; vertNum++)
                    {
                        //определение индекса вершины смежной текущей
                        if (vertNum == VertexList.Count - 1)
                        {
                            nextVert = 0;
                        }
                        else
                        {
                            nextVert = vertNum + 1;
                        }
                        //координаты двух смежных отрезков
                        float Yi = VertexList[vertNum].Y;
                        float Xi = VertexList[vertNum].X;
                        float Yk = VertexList[nextVert].Y;
                        float Xk = VertexList[nextVert].X;
                        int x;
                        if (((Yi < y) && (Yk >= y)) || ((Yi >= y) && (Yk < y)))
                        {
                            if (Xi - Xk != 0)
                            {
                                float A = (Yi - Yk) / (Xi - Xk);
                                float C = Yk - A * Xk;
                                x = (int)((y - C) / A);
                                
                            }
                            else
                            {
                                x = (int)Xi;
                            }
                            xPoints.Add(x);
                        }
                    }
                    //сортировка списка х координат 
                    xPoints.Sort();

                    //случай, если у каждого пересечения есть пара
                    if (xPoints.Count % 2 == 0 && xPoints.Count > 1)
                    {
                        for (int xb = 0; xb < xPoints.Count; xb += 2)
                        {
                            firPoint = new Point(xPoints[xb], y);
                            secPoint = new Point(xPoints[xb + 1], y);
                            g.DrawLine(DrPen, firPoint, secPoint);
                        }
                    }
                    //нечетное число перечечений
                    else if (xPoints.Count > 2)
                    {
                        for (int xb = 0; xb < xPoints.Count - 1; xb++)
                        {
                            firPoint = new Point(xPoints[xb], y);
                            secPoint = new Point(xPoints[xb + 1], y);
                            g.DrawLine(DrPen, firPoint, secPoint);
                        }
                    }
                }
            }
            //отрисовка как для ориентированного многоугольника
            else
            {
                int jYmax = 0;
                for (int g = 0; g < VertexList.Count; g++)
                {
                    if (VertexList[g].Y == Ymax) { jYmax = g; break; }
                }
                Point A, B, C;
                if (jYmax > 0) { A = VertexList[jYmax - 1]; } else { A = VertexList[VertexList.Count - 1]; }
                if (jYmax < VertexList.Count - 1) { C = VertexList[jYmax + 1]; } else { C = VertexList[0]; }
                B = VertexList[jYmax];
                bool CW = detectCW(A, B, C);
                /*
                if (CW) { }

                for (int vertNum = 0; vertNum < VertexList.Count; vertNum++)
                {

                    for (int i = 1; i < VertexList.Count; i++;){
                        


                    }
                }


                */

                    //MessageBox.Show(CW.ToString());
                }
        }
        // Обработчик события
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            VertexList.Add(new Point() { X = e.X, Y = e.Y });

            g.DrawEllipse(DrawPen, e.X - 2, e.Y - 2, 5, 5);
           // g.DrawString("V(" + VertexList.Count + ")",new Font("Arial", 14), Brushes.Black, VertexList[VertexList.Count-1]);

            if ((e.Button == MouseButtons.Right))// Конец ввода
            {
                if(VertexList.Count >= 3)
                {
                    if (outputType == 0)
                    {
                        g.DrawLine(DrawPen, VertexList[VertexList.Count - 1], VertexList[0]);
                        g.DrawLine(DrawPen, VertexList[VertexList.Count - 1], VertexList[VertexList.Count - 2]);
                        PaintFigure(DrawPen);
                    }
                    PaintFigure(DrawPen);
                    VertexList.Clear();
                }
                else if(VertexList.Count == 2)
                {
                    if (outputType == 0)
                    {
                        g.DrawLine(DrawPen, VertexList[VertexList.Count - 1], VertexList[VertexList.Count - 2]);
                    }  
                    MessageBox.Show("Недостаточное количество точек для многоугольника. Вы поставили только 2 точки, необходимо не менее 3.");
                }
                else { MessageBox.Show("Недостаточное количество точек для многоугольника. Вы поставили только 1 точку, необходимо не менее 3."); }
            }
            else
            {
                if(outputType == 0)
                {
                    if(VertexList.Count > 1)
                    {
                        g.DrawLine(DrawPen, VertexList[VertexList.Count-1], VertexList[VertexList.Count-2]);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            VertexList.Clear();
        }
    }
}
