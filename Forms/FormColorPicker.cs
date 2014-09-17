// Image Color Picker
// by mostone@hotmail.com
// http://blog.csdn.net/mostone
// 2013-10-06

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MosFontTool.Common;

namespace MosFontTool
{
    public partial class FormColorPicker : Form
    {

        Bitmap bmpZoom = null;
        Bitmap bmpSrc = null;

        public FormColorPicker()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.fontViewrIcon;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult isDone = openFileDialog1.ShowDialog();
                if (isDone == DialogResult.OK)
                {
                    pictureBox1.Image = null;
                    pictureBox2.Image = null;
                    bmpSrc = null;
                    bmpZoom = null;

                    Image img = Bitmap.FromFile(openFileDialog1.FileName);
                    pictureBox1.Image = img;
                    bmpSrc = new Bitmap(img);
                }
            }
            catch (Exception ex)
            {
                pictureBox1.Image = null;
                bmpSrc = null;
                MessageBox.Show(ex.Message);
            }

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            const int zoomSize = 8;
            if (this.bmpSrc == null)
            {
                return;
            }

            bmpZoom = null;
            bmpZoom = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            Graphics grpDst = Graphics.FromImage(bmpZoom);

            // zoom to 8x
            int width = pictureBox2.Width / zoomSize;
            int height = pictureBox2.Height / zoomSize;

            int offsetX = width / 2;
            int offsetY = height / 2;

            int x = e.X - offsetX;
            int y = e.Y - offsetY;
            if (offsetX + e.X >= bmpSrc.Width)
            {
                x = bmpSrc.Width - offsetX * 2;
            }
            if (x < 0)
            {
                x = 0;
            }

            if (offsetY + e.Y >= bmpSrc.Height)
            {
                y = bmpSrc.Height - offsetY * 2;
            }
            if (y < 0)
            { 
                y = 0;
            }

            Color color;
            int oriX = x;
            for (int row = 0; row < pictureBox2.Height; row += zoomSize)
            {
                if (y >= bmpSrc.Height) break;

                for (int col = 0; col < pictureBox2.Width; col += zoomSize)
                {
                    if (x >= bmpSrc.Width) break;

                    // get pixel color
                    color = bmpSrc.GetPixel(x, y);
                    // draw zoom block
                    grpDst.FillRectangle(new SolidBrush(color), col, row, zoomSize, zoomSize);
                    x++;
                }
                x = oriX;
                y++;
            }

            pictureBox2.Image = bmpZoom;

        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (bmpZoom == null) return;

            int x = (e.X >= bmpZoom.Width) ? bmpZoom.Width - 1 : e.X;
            int y = (e.Y >= bmpZoom.Height) ? bmpZoom.Height - 1 : e.Y;
            Color color = bmpZoom.GetPixel(x, y);
            labelColor.BackColor = color;
            String val = color.ToArgb().ToString("X");
            textBox1.Text = "#" + (val.Length == 8 ? val.Substring(2) : val);
            textBox2.Text = "#" + Misc.getRGB565(color).ToString("X");
            textBox3.Text = "#" + Misc.getRGB565P(color).ToString("X");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Image Color Picker

    by mostone@hotmail.com
    http://blog.csdn.net/mostone
    2013-10-06", "About...");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StreamWriter strm = new StreamWriter("tmp.c", false);
            int val;

            for (int i = 0; i < bmpSrc.Height; i++)
            {
                for (int j = 0; j < bmpSrc.Width; j++)
                {
                    val = Misc.getRGB565P(bmpSrc.GetPixel(j,i));
                    strm.Write("0x{0},", (val & 0xff).ToString("X2"));
                    strm.Write("0x{0},", (val >> 8).ToString("X2"));
                }
                strm.WriteLine();
            }

            strm.Close();

            MessageBox.Show("export done.");
        }
    }
}
