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
    public partial class FormOpenFontDialog : Form
    {
        string pFontSize = null;

        public FormOpenFontDialog()
        {
            InitializeComponent();

            string[] fontSizeList = FormFontViewer.getFontSizeList();
            string prevFontSize = null;
            Size radioSize = new Size(120, 30);
            int radioOriX = 25;
            Point radioLocation = new Point(radioOriX, 30);
            foreach (string fontSize in fontSizeList)
            {
                if (prevFontSize == null)
                {
                    prevFontSize = fontSize.Substring(0, 2);
                }
                else if (fontSize.Substring(0, 2) != prevFontSize)
                {
                    radioLocation.Y += radioSize.Height;
                    radioLocation.X = radioOriX;
                    prevFontSize = fontSize.Substring(0, 2);
                }

                RadioButton radio = new RadioButton();
                radio.Text = fontSize;
                radio.Location = radioLocation;
                radio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
                this.groupBox1.Controls.Add(radio);

                radioLocation.X += radioSize.Width;
            }
        }

        private void btnPickFont_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.txtFontFile.Text = this.openFileDialog1.FileName;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public string getFontFileName()
        {
            return this.txtFontFile.Text;
        }

        public System.IO.Stream getFontFileStream()
        {
            if (this.txtFontFile.Text.Length != 0)
            {
                return this.openFileDialog1.OpenFile();
            }

            return null;
        }

        public string getFontSize()
        {
            return pFontSize;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.txtFontFile.Text.Length == 0)
            {
                MessageBox.Show("Please select font file.");
                return;
            }

            if (pFontSize == null)
            {
                MessageBox.Show("Please select font type.");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            pFontSize = ((RadioButton)sender).Text;
        }
    }
}
