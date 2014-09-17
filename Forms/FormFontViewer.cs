using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MosFontTool
{
    public partial class FormFontViewer : Form
    {
        System.IO.Stream pFontReadStream;
        string pFontFileName;
        int pPageNo = 0;
        Size pFontSize;
        Bitmap pBitmap;
        Graphics pGraphics;
        FormOpenFontDialog pDialog = null;
        FormFontDraw pFormFontDraw = null;

        static string[] FONT_SIZE_LIST = new string[] { "12x6", "12x8", "12x12", "16x8", "16x16", "24x24", "40x20", "40x40", "48x24", "48x48" };

        public FormFontViewer()
        {
            InitializeComponent();
            this.pBitmap = new Bitmap(930, 910);
            this.pGraphics = Graphics.FromImage(pBitmap);

            this.toolStripStatusLabel1.Visible = false;
            this.toolStripProgressBar1.Visible = false;

            this.comboBoxFontSize.Items.AddRange(FONT_SIZE_LIST.ToArray());
            this.Icon = Properties.Resources.fontViewrIcon;
        }

        public static string[] getFontSizeList()
        {
            return FONT_SIZE_LIST;
        }

        private Size getSize(string value)
        {
            return new Size(Int16.Parse(value.Substring(3)), Int16.Parse(value.Substring(0, 2)));
        }

        private void btnPickFont_Click(object sender, EventArgs e)
        {
            if (pDialog == null) pDialog = new FormOpenFontDialog();

            if (pDialog.ShowDialog() != DialogResult.OK) return;

            pFontReadStream = pDialog.getFontFileStream();
            pFontSize = this.getSize(pDialog.getFontSize());
            pFontFileName = pDialog.getFontFileName();

            this.comboBoxFontSize.SelectedIndexChanged -= new EventHandler(this.comboBoxFontSize_SelectedIndexChanged);
            this.comboBoxFontSize.SelectedItem = pDialog.getFontSize();
            this.comboBoxFontSize.SelectedIndexChanged += new EventHandler(this.comboBoxFontSize_SelectedIndexChanged);

            redraw();
        }

        private void redraw()
        {
            pPageNo = 0;
            drawPage(pPageNo);

            this.toolStripStatusLabel1.Text = String.Format("{0}   Font size:{1}x{2}", pFontFileName, pFontSize.Height, pFontSize.Width);
        }

        private void drawPage(int pageNo)
        {
            if (pFontReadStream == null) return;

            const int space = 5;
            const int ox = 50;
            const int oy = 20;
            Point pt = new Point(ox, oy);

            pFontReadStream.Position = pageNo * 16 * 16 * (pFontSize.Width == 12 ? 16 : pFontSize.Width) * pFontSize.Height / 8;

            pGraphics.Clear(this.pictureBox1.BackColor);
            this.toolStripStatusLabel1.Visible = false;
            this.toolStripProgressBar1.Visible = true;

            List<byte> list = new List<byte>();
            bool eof = false;
            int tmp;
            for (int row = 0; row < 16; row++)
            {
                pGraphics.DrawString((pageNo * 16 * 16 + row * 16).ToString("X4"), DefaultFont, Brushes.Black, new Point(10, oy + row * (space + pFontSize.Height)));
                for (int col = 0; col < 16; col++)
                {
                    for (int i = 0; i < (pFontSize.Width == 12 ? 16 : pFontSize.Width) * pFontSize.Height / 8; i++)
                    {
                        tmp = pFontReadStream.ReadByte();
                        if (tmp == -1)
                        {
                            eof = true; break;
                        }
                        list.Add((byte)tmp);
                    }
                    if (eof) break;
                    if (this.radioButtonRowScan.Checked)
                    {
                        drawFontAsRowScan(pt, pGraphics, list, pFontSize);
                    }
                    else
                    {
                        drawFontAsColumnScan(pt, pGraphics, list, pFontSize);
                    }
                    pt.X += space + pFontSize.Width;
                    list.Clear();

                    this.toolStripProgressBar1.Value = Math.Min((row * 16 + col) / 16 * 16, 100);
                }
                if (eof) break;
                pt.Y += space + pFontSize.Height;
                pt.X = ox;
            }

            pictureBox1.Image = pBitmap;
            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = true;
        }

        // draw one char
        private void drawFontAsRowScan(Point pt, Graphics graph, List<byte> list, Size fontSize)
        {
            Pen pen;
            int mask;
            int bytesPerLine = (int)Math.Ceiling((decimal)fontSize.Width / 8);
            List<byte>.Enumerator enm = list.GetEnumerator();
            enm.MoveNext();
            byte byteData = enm.Current;
            for (int row = 0; row < fontSize.Height; row++)
            {
                for (int bytes = 0; bytes < bytesPerLine; bytes++)
                {
                    mask = 0x80;
                    for (int bits = 0; bits < 8; bits++)
                    {
                        pen = ((byteData & mask) == mask ? Pens.Black : Pens.White);
                        graph.DrawRectangle(pen, pt.X, pt.Y, 1, 1);
                        mask = mask >> 1;
                        pt.X += 1;

                        if (bytes * 8 + bits == fontSize.Width - 1) break;
                    }
                    enm.MoveNext();
                    byteData = enm.Current;
                }
                pt.Y += 1;
                pt.X -= fontSize.Width;
            }
        }

        // draw one char
        private void drawFontAsColumnScan(Point pt, Graphics graph, List<byte> list, Size fontSize)
        {
            Pen pen;
            int mask;
            int bytesPerLine = (int)Math.Ceiling((decimal)fontSize.Width / 8);
            List<byte>.Enumerator enm = list.GetEnumerator();
            enm.MoveNext();
            byte byteData = enm.Current;
            for (int col = 0; col < fontSize.Width; col++)
            {
                for (int bytes = 0; bytes < bytesPerLine; bytes++)
                {
                    mask = 0x80;
                    for (int bits = 0; bits < 8; bits++)
                    {
                        pen = ((byteData & mask) == mask ? Pens.Black : Pens.White);
                        graph.DrawRectangle(pen, pt.X, pt.Y, 1, 1);
                        mask = mask >> 1;
                        pt.Y += 1;

                        if (bytes * 8 + bits == fontSize.Height - 1) break;
                    }
                    enm.MoveNext();
                    byteData = enm.Current;
                }
                pt.X += 1;
                pt.Y -= fontSize.Height;
            }
        }

        private void FormFontViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (pFontReadStream != null)
            {
                pFontReadStream.Close();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            drawPage(++pPageNo);
        }

        private void buttonPrevous_Click(object sender, EventArgs e)
        {
            if (pPageNo == 0) return;

            drawPage(--pPageNo);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (pFontReadStream == null) return;

            if (this.saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            System.IO.Stream ss = this.saveFileDialog1.OpenFile();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(ss);
            pFontReadStream.Position = 0;

            int byt;
            int pos = 0;
            while (true)
            {
                byt = pFontReadStream.ReadByte();
                if (byt == -1) break;

                pos++;
                if (pos == 32)
                {
                    sw.WriteLine("0x" + byt.ToString("X2") + ",");
                    pos = 0;
                }
                else
                {
                    sw.Write("0x" + byt.ToString("X2") + ",");
                }
            }

            sw.Close();
            MessageBox.Show("Font array export done.");
        }

        private void radioButtonScanMode_CheckedChanged(object sender, EventArgs e)
        {
            drawPage(pPageNo);
        }

        private void comboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pFontSize = getSize(this.comboBoxFontSize.SelectedItem.ToString());
            redraw();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            pFormFontDraw = new FormFontDraw();
            pFormFontDraw.Owner = this;
            pFormFontDraw.Show();
            this.Hide();
        }
    }
}
