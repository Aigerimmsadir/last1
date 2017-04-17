using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintApp
{
    public partial class Form1 : Form
    {
        State state = State.Pen;
        Color color = Color.Red;
        Bitmap bmp;
        Queue<Point> q = new Queue<Point>();
        Graphics g;
        Point curPoint;
        GraphicsPath gp = new GraphicsPath();
        int w = 1;
        Point prevPoint;
        Shapes currentShape = Shapes.Free;
        Color colorOrigin;
        Color colorFill;




///mouse functions
        public Form1()
        {
            this.Size = new Size(900, 900);
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(pictureBox1.Image);
        }
        
            
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Pen p = new Pen(color, w);
            if (state == State.Pen || state== State.Brush)
            {
                g.DrawPath(p, gp);
            }
            else
            {
                g.FillPath(p.Brush, gp);
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (state)
            {
                case State.Pen:
                    prevPoint = e.Location;
                    break;
                case State.Fill:
                    q.Enqueue(e.Location);
                    colorOrigin = bmp.GetPixel(e.X, e.Y);
                    colorFill = color;
                    Fill();
                    break;
                default:
                    break;
            }
        }
        private void Fill()
        {
            while (q.Count > 0)
            {
                Point curPoint = q.Dequeue();
                Step(curPoint.X + 1, curPoint.Y);
                Step(curPoint.X - 1, curPoint.Y);
                Step(curPoint.X, curPoint.Y + 1);
                Step(curPoint.X, curPoint.Y - 1);
            }

            pictureBox1.Refresh();
        }

        private void Step(int x, int y)
        {
            if (x < 0) return;
            if (y < 0) return;
            if (x >= bmp.Width) return;
            if (y >= bmp.Height) return;
            if (bmp.GetPixel(x, y) != colorOrigin) return;
            bmp.SetPixel(x, y, colorFill);
            q.Enqueue(new Point(x, y));
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                curPoint = e.Location;
                if (state == State.Pen)
                {
                    switch (currentShape)
                    {
                        case Shapes.Free:
                            g.DrawLine(new Pen(color, w), prevPoint, curPoint);
                            prevPoint = curPoint;

                            break;
                        case Shapes.Line:
                            gp.Reset();
                            gp.AddLine(prevPoint, curPoint);

                            break;
                        case Shapes.Rectangle:
                            gp.Reset();
                            gp.AddRectangle(GenerateRectangle(prevPoint, curPoint));
                            break;
                        case Shapes.Triangle:
                            gp.Reset();
                            gp.AddPolygon(GenerateTriangle(prevPoint, curPoint));
                            break;
                        case Shapes.Circle:
                            gp.Reset();
                            gp.AddEllipse(GenerateEllipse(prevPoint, curPoint));
                            break;
                        default:
                            break;
                    }


                }
             else
                {

                    Pen p = new Pen(color);

                    gp.AddEllipse(prevPoint.X, prevPoint.Y, w, w);
                    prevPoint = curPoint;
                   
                }
            }

                pictureBox1.Refresh();
                mouseLocationLabel.Text = string.Format("X:{0},Y:{1}", e.X, e.Y);
            
        }





///generatefigures
        private RectangleF GenerateRectangle(Point prevPoint, Point curPoint)
        {

            return new Rectangle(Math.Min(prevPoint.X, curPoint.X), Math.Min(prevPoint.Y, curPoint.Y), Math.Abs(prevPoint.X - curPoint.X), Math.Abs(prevPoint.Y - curPoint.Y));
        }


        private RectangleF GenerateEllipse(Point prevPoint, Point curPoint)
        {

            return new Rectangle(Math.Min(prevPoint.X, curPoint.X), Math.Min(prevPoint.Y, curPoint.Y), Math.Abs(prevPoint.X - curPoint.X), Math.Abs(prevPoint.Y - curPoint.Y));
        }
        private Point[] GenerateTriangle(Point prevPoint, Point curPoint)
        {

            Point peak;
            Point border;
            if (prevPoint.Y < curPoint.Y) { peak = prevPoint; border = curPoint; }
            else { peak = curPoint; border = prevPoint; }
            int k = peak.X - border.X;
            Point third = new Point(peak.X + k, border.Y);

            Point[] points = { peak, border, third };
            return points;
        }




    






///save and open functions
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(sfd.FileName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bitmap newimage = new Bitmap(ofd.FileName);
                Bitmap cloneimage = newimage.Clone() as Bitmap;
                newimage.Dispose();
                pictureBox1.Image = cloneimage;
                g = Graphics.FromImage(pictureBox1.Image);
            }
        }





///buttonclicks
        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                color = dlg.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentShape = Shapes.Free;
        }
       
        private void button3_Click(object sender, EventArgs e)
        {
            currentShape = Shapes.Line;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawPath(new Pen(color), gp);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (currentShape == Shapes.Rectangle) currentShape = Shapes.Free;
            else currentShape = Shapes.Rectangle;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (currentShape == Shapes.Circle) currentShape = Shapes.Free;
            else currentShape = Shapes.Circle;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (currentShape == Shapes.Triangle) currentShape = Shapes.Free;
            else currentShape = Shapes.Triangle;
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox sendercombobox = (ComboBox)sender;
            w = sendercombobox.SelectedIndex;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            w++;
        }





//enums 
        enum Shapes
        {
            Free,
            Line,
            Rectangle,
            Triangle,
            Circle
        }
        enum State
        {
            Pen,
            Fill,
            Brush

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (state == State.Fill) state = State.Pen;
            else state = State.Fill;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            state = State.Brush;
        }
    }
}
