using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MosFontTool.Common;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace MosFontTool
{
    public partial class FormFontDraw : Form
    {
        /// <summary>
        /// Graphics of Content for draw Font char list
        /// </summary>
        Graphics graphicsOfContent;
        /// <summary>
        /// Indicates which alignment be selected
        /// </summary>
        Button ButtonAlignChecked;
        /// <summary>
        /// For store user choices
        /// </summary>
        FontSettings fontSettings;
        bool isBlockAdjusting = false;

        public FormFontDraw()
        {
            InitializeComponent();

            // create graphics for draw font
            this.FormFontDraw_Resize(null, null);

            // file system font families listbox
            System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
            foreach (System.Drawing.FontFamily family in fonts.Families)
            {
                listBoxSystemFont.Items.Add(family.Name);
            }
            // init alignment setting
            buttonAlignment_Click(button5, null);

            // demo string for font draw
            richTextBoxCharInput.Text = Properties.Resources.inputChars;
            // set form icon
            this.Icon = Properties.Resources.fontViewrIcon;

            // set default values
            buttonResetBlock_Click(null, null);

            // set trackbar's parameter
            trackBarBlockWidth.Minimum = (int)numericUpDownBlockWidth.Minimum;
            trackBarBlockWidth.Maximum = (int)numericUpDownBlockWidth.Maximum;
            trackBarBlockWidth.Value = (int)numericUpDownBlockWidth.Value;
            trackBarBlockHeight.Minimum = (int)numericUpDownBlockHeight.Minimum;
            trackBarBlockHeight.Maximum = (int)numericUpDownBlockHeight.Maximum;
            trackBarBlockHeight.Value = (int)numericUpDownBlockHeight.Value;
            trackBarOffsetX.Minimum = (int)numericUpDownOffsetX.Minimum;
            trackBarOffsetX.Maximum = (int)numericUpDownOffsetX.Maximum;
            trackBarOffsetX.Value = (int)numericUpDownOffsetX.Value;
            trackBarOffsetY.Minimum = (int)numericUpDownOffsetY.Minimum;
            trackBarOffsetY.Maximum = (int)numericUpDownOffsetY.Maximum;
            trackBarOffsetY.Value = (int)numericUpDownOffsetY.Value;

        }

        /// <summary>
        /// Get user choice and save to fontSettings
        /// </summary>
        /// <returns>if validation failured return false</returns>
        private bool getSettings()
        {
            if (fontSettings != null)
            {
                fontSettings.Dispose();
                vScrollBar1.Value = 0;
            }

            try
            {
                fontSettings = new FontSettings();
                fontSettings.SetForeColor(textBoxForecolor.Text);
                fontSettings.SetBackColor(textBoxBackgroundColor.Text);

                FontStyle fntStyle = radioButton1.Checked ? FontStyle.Regular :
                    radioButton2.Checked ? FontStyle.Italic :
                    radioButton3.Checked ? FontStyle.Bold : FontStyle.Bold | FontStyle.Italic;
                fontSettings.SetFont(listBoxSystemFont.SelectedItem, fntStyle, listBoxSize.SelectedItem);

                fontSettings.RenderingHint = radioButton5.Checked ? System.Drawing.Text.TextRenderingHint.SingleBitPerPixel :
                    radioButton6.Checked ? System.Drawing.Text.TextRenderingHint.AntiAliasGridFit :
                    radioButton7.Checked ? System.Drawing.Text.TextRenderingHint.ClearTypeGridFit :
                    System.Drawing.Text.TextRenderingHint.SystemDefault;

                fontSettings.TextFormatFlags = ButtonAlignChecked == button1 ? TextFormatFlags.Left | TextFormatFlags.Top :
                    ButtonAlignChecked == button2 ? TextFormatFlags.HorizontalCenter | TextFormatFlags.Top :
                    ButtonAlignChecked == button3 ? TextFormatFlags.Right | TextFormatFlags.Top :
                    ButtonAlignChecked == button4 ? TextFormatFlags.Left | TextFormatFlags.VerticalCenter :
                    ButtonAlignChecked == button6 ? TextFormatFlags.Right | TextFormatFlags.VerticalCenter :
                    ButtonAlignChecked == button7 ? TextFormatFlags.Left | TextFormatFlags.Bottom :
                    ButtonAlignChecked == button8 ? TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom :
                    ButtonAlignChecked == button9 ? TextFormatFlags.Right | TextFormatFlags.Bottom :
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

                fontSettings.SetBlockSize(numericUpDownBlockWidth.Value, numericUpDownBlockHeight.Value);
                fontSettings.SetOffset(numericUpDownOffsetX.Value, numericUpDownOffsetY.Value);

                fontSettings.Chars = richTextBoxCharInput.Text;
                fontSettings.processChars(checkBoxIgnoreNewLine.Checked, checkBoxInsertNewLine.Checked,
                    checkBoxSortChars.Checked, checkBoxFilterChars.Checked);

                // clear zoom image
                if (pictureBoxZoom.Image != null)
                {
                    pictureBoxZoom.Image.Dispose();
                    pictureBoxZoom.Image = null;
                    pictureBoxZoom.Refresh();
                    labelCurrent.Text = "";
                }

                fontSettings.AutoFixWidth = checkBoxAutoFixWidth.Checked;
                fontSettings.MinWidth = (int)numericUpDownMinWidth.Value;
                if (fontSettings.MinWidth > fontSettings.BlockWidth)
                {
                    fontSettings.MinWidth = fontSettings.BlockWidth;
                }
            }
            catch (Exception ex)
            {
                Misc.MsgErr(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Show char bitmap list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonView_Click(object sender, EventArgs e)
        {
            // required check
            Control[] controls = new Control[] { listBoxSystemFont, listBoxSize, richTextBoxCharInput, textBoxForecolor, textBoxBackgroundColor };
            string[] messages = new string[]{Properties.Resources.Err_font_required,Properties.Resources.Err_size_required,
                Properties.Resources.Err_char_required,Properties.Resources.Err_forecolor_required,Properties.Resources.Err_backcolor_required};
            if (!Misc.isFilled(controls, messages)) return;
            // if invalidation do nothing
            if (!getSettings()) return;

            // set scrollbar parameters
            vScrollBar1.Maximum = FontDrawExtend.getScrollMaximum(pictureBoxPage.ClientSize.Height, fontSettings);
            vScrollBar1.LargeChange = FontDrawExtend.getScrollLargeChange(fontSettings, pictureBoxPage.ClientSize.Height);
            vScrollBar1.Tag = 1;  // prevent redraw
            vScrollBar1.Value = 0;
            vScrollBar1.Tag = null;

            // draw char list
            vScrollBar1_Scroll(vScrollBar1, new ScrollEventArgs(ScrollEventType.First, 0));
        }

        /// <summary>
        /// Color value input control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxForecolor_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            char c = e.KeyChar;
            e.Handled = !Misc.isHexColorKeypressAllowed(txt, c);
        }

        /// <summary>
        /// Save alignment choice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAlignment_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button == ButtonAlignChecked)
            {
                return;
            }
            else if (ButtonAlignChecked != null)
            {
                ButtonAlignChecked.BackColor = SystemColors.Control;
            }
            button.BackColor = button.FlatAppearance.CheckedBackColor;
            ButtonAlignChecked = button;
        }

        /// <summary>
        /// Reset block settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResetBlock_Click(object sender, EventArgs e)
        {
            buttonAlignment_Click(button5, null);

            numericUpDownBlockWidth.Value = Properties.Settings.Default.DefaultBlockWidth;
            numericUpDownBlockHeight.Value = Properties.Settings.Default.DefaultBlockHeight;
            numericUpDownOffsetX.Value = Properties.Settings.Default.DefaultOffsetX;
            numericUpDownOffsetY.Value = Properties.Settings.Default.DefaultOffsetY;
        }

        /// <summary>
        /// Scroll content view(picturebox's image)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (vScrollBar1.Tag != null) return;  // set tag values for prevent redraw 
            if (fontSettings == null) return;

            vScrollBar1.Tag = 1;

            FontDrawExtend.DrawContentImage(graphicsOfContent, pictureBoxPage.ClientSize.Height, fontSettings, e.NewValue);
            pictureBoxPage.Refresh();

            vScrollBar1.Tag = null;
        }

        /// <summary>
        /// Set checkboxInserNewLine's status when ignoreNewLine changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxIgnoreNewLine_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxInsertNewLine.Enabled = !checkBoxIgnoreNewLine.Checked;
            if (!checkBoxInsertNewLine.Enabled) checkBoxInsertNewLine.Checked = false;
        }

        /// <summary>
        /// Set checkboxIgnoreNewLine's status when insertNewLine changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxInsertNewLine_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxIgnoreNewLine.Enabled = !checkBoxInsertNewLine.Checked;
            if (!checkBoxIgnoreNewLine.Enabled) checkBoxIgnoreNewLine.Checked = false;
        }

        /// <summary>
        /// Set filter checkbox status
        /// if sort is not selected, filter will be disable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxSortChars_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxFilterChars.Enabled = checkBoxSortChars.Checked;
            if (!checkBoxFilterChars.Enabled) checkBoxFilterChars.Checked = false;
        }

        /// <summary>
        /// Recreate Image for Content pictruebox When form resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormFontDraw_Resize(object sender, EventArgs e)
        {
            // release previous image resource
            if (graphicsOfContent != null)
            {
                graphicsOfContent.Dispose();
                pictureBoxPage.Image.Dispose();
            }
            // when form status is minimum, can't get picturebox size
            if (pictureBoxPage.Width <= 0 || pictureBoxPage.Height <= 0)
            {
                return;
            }

            // create new size bitmap for picturebox of content
            Bitmap bmp = new Bitmap(pictureBoxPage.ClientSize.Width, pictureBoxPage.ClientSize.Height);
            graphicsOfContent = Graphics.FromImage(bmp);
            graphicsOfContent.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            pictureBoxPage.Image = bmp;

            // if previous status is content showed, then redraw it
            vScrollBar1_Scroll(vScrollBar1, new ScrollEventArgs(ScrollEventType.ThumbPosition, vScrollBar1.Value));
        }

        /// <summary>
        /// Show Single Char's Zoomed Large Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxPage_MouseClick(object sender, MouseEventArgs e)
        {
            // if the content never showed, do nothing
            if (fontSettings == null) return;

            // get index of char which current clicked
            int indexOfChar = FontDrawExtend.getCurrentCharIndex(e.X, e.Y, vScrollBar1.Value, fontSettings);
            // if index outof bound, do nothing
            if (indexOfChar == -1) return;

            // release previous image resource
            if (pictureBoxZoom.Image != null)
            {
                pictureBoxZoom.Image.Dispose();
            }

            // redraw and show it
            using (Bitmap bmp = FontDrawExtend.DrawChar(fontSettings, indexOfChar))
            {
                pictureBoxZoom.Image = FontDrawExtend.ZoomBmp(bmp, pictureBoxZoom.ClientSize, fontSettings);
                pictureBoxZoom.Refresh();
            }

            // display char information
            labelCurrent.Text = "Char Unicode: 0x" + indexOfChar.ToString("X");
        }

        /// <summary>
        /// Export Font Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // check output settings
            COLOR_MODE colorMode = radioButton8.Checked ? COLOR_MODE.MONO :
                radioButton9.Checked ? COLOR_MODE.GRAY4BITS :
                radioButton10.Checked ? COLOR_MODE.RGB565 :
                radioButton11.Checked ? COLOR_MODE.RGB565P :
                COLOR_MODE.UNKONOWN;
            if (colorMode == COLOR_MODE.UNKONOWN)
            {
                Misc.MsgInf(Properties.Resources.Inf_colorMode_required);
                return;
            }
            SCAN_MODE scanMode = radioButtonRow1.Checked ? SCAN_MODE.ROW_SCAN :
                radioButtonColumn1.Checked ? SCAN_MODE.COLUMN_SCAN :
                SCAN_MODE.UNKOWN;
            if (scanMode == SCAN_MODE.UNKOWN)
            {
                Misc.MsgInf(Properties.Resources.Inf_scanMode_required);
                return;
            }
            string arrayName = textBoxArrayName.Text;
            if (arrayName.Length == 0)
            {
                Misc.MsgInf(Properties.Resources.Inf_arrayName_required);
                return;
            }
            
            // if content never showed, check input parameters, and show it
            if (fontSettings == null)
            {
                this.toolStripButtonView_Click(null, null);
                if (fontSettings == null) return;
            }

            // user canceled
            if (this.saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            if (radioButton13.Checked)
            {
                saveAsBitmap(saveFileDialog1.FileName);
                return;
            }

            // get stream for write font data
            List<byte> bytes;
            int iNewLinePosition = Properties.Settings.Default.NewLinePositionOfExport;
            int pos = 1;
            Size size = new Size();
            StringBuilder sb = new StringBuilder();
            using(Stream ss = this.saveFileDialog1.OpenFile())
            {
                using(StreamWriter sw = new System.IO.StreamWriter(ss))
                {
                    if (!fontSettings.AutoFixWidth)
                    {
                        sw.WriteLine(string.Format(@"const unsigned char {0} = {{", arrayName));
                    }
                    using (Bitmap bmp = new Bitmap(fontSettings.BlockWidth, fontSettings.BlockHeight))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            // output chars data one by one
                            foreach (char c in fontSettings.Chars)
                            {
                                FontDrawExtend.DrawChar(g, fontSettings, c);
                                bytes = FontDrawExtend.getCharFontData(bmp, colorMode, scanMode, fontSettings, out size);

                                if (fontSettings.AutoFixWidth)
                                {
                                    sw.WriteLine(@"

// char ""{0}""
const unsigned char {1}_{2:X2}_dat[] = {{", c, arrayName, (int)c);
                                    pos = 1;
                                }
                                foreach (byte b in bytes)
                                {
                                    sw.Write("0x{0:X2},", b);
                                    if (iNewLinePosition > 0 && pos == iNewLinePosition)
                                    {
                                        sw.Write(Environment.NewLine);
                                        pos = 0;
                                    }
                                    pos++;
                                }
                                if (fontSettings.AutoFixWidth)
                                {
                                    sw.WriteLine("};");
                                    sw.WriteLine(@"mos_font_data_t {0}_{1:X2} = {{
{2},{3},
&{4}_{5:X2}_dat[0]}};", arrayName, (int)c, size.Width, size.Height, arrayName, (int)c);
                                    sb.Append(String.Format("&{0}_{1:X2},", arrayName, (int)c));
                                }
                            }
                        }
                    }
                    if (fontSettings.AutoFixWidth)
                    {
                        sw.WriteLine(@"


// data list
mos_font_data_t* {0}[] = {{{1}}};", arrayName, sb.ToString());
                    }
                    else
                    {
                        sw.WriteLine("}");
                    }
                }
            }

            // ask for open export file
            if (MessageBox.Show(Properties.Resources.Inf_openFileConfirm, 
                Properties.Resources.Caption_exportMsg, 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(this.saveFileDialog1.FileName);
            }

        }

        private void saveAsBitmap(string filename)
        {
            int y = 0;
            Rectangle rect = new Rectangle(0, 0, fontSettings.BlockWidth, fontSettings.BlockHeight);
            using (Bitmap bmp = new Bitmap(fontSettings.BlockWidth, fontSettings.BlockHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (Bitmap bmpOutput = new Bitmap(fontSettings.BlockWidth, fontSettings.BlockHeight * fontSettings.Chars.Length))
                    {
                        using (Graphics grpOutput = Graphics.FromImage(bmpOutput))
                        {
                            // output chars data one by one
                            foreach (char c in fontSettings.Chars)
                            {
                                FontDrawExtend.DrawChar(g, fontSettings, c);
                                grpOutput.DrawImage(bmp, 0, y, rect, GraphicsUnit.Pixel);
                                y += fontSettings.BlockHeight;
                            }
                        }
                        bmpOutput.Save(filename, ImageFormat.Bmp);
                    }
                }
            }
            Misc.MsgInf(Properties.Resources.Inf_bmpExpdortComplete);
        }

        /// <summary>
        /// Show form color picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonColorPicker_Click(object sender, EventArgs e)
        {
            using (FormColorPicker form = new FormColorPicker())
            {
                this.Hide();
                form.ShowDialog();
                this.Show();
            }
        }

        /// <summary>
        /// Show form font viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonFontViewer_Click(object sender, EventArgs e)
        {
            using (FormFontViewer form = new FormFontViewer())
            {
                this.Hide();
                form.ShowDialog();
                this.Show();
            }
        }

        /// <summary>
        /// Show about dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            Misc.MsgInf(Properties.Resources.Inf_about);
        }

        /// <summary>
        /// Update related control's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownBlockWidth_ValueChanged(object sender, EventArgs e)
        {
            if (isBlockAdjusting) return;

            isBlockAdjusting = true;

            if (sender == numericUpDownBlockWidth)
            {
                trackBarBlockWidth.Value = (int)numericUpDownBlockWidth.Value;
            }
            else if (sender == numericUpDownBlockHeight)
            {
                trackBarBlockHeight.Value = (int)numericUpDownBlockHeight.Value;
            }
            else if (sender == numericUpDownOffsetX)
            {
                trackBarOffsetX.Value = (int)numericUpDownOffsetX.Value;
            }
            else if (sender == numericUpDownOffsetY)
            {
                trackBarOffsetY.Value = (int)numericUpDownOffsetY.Value;
            }
            else if (sender == numericUpDownMinWidth)
            {
                trackBarMinWidth.Value = (int)numericUpDownMinWidth.Value;
            }
            else if (sender == trackBarBlockWidth)
            {
                numericUpDownBlockWidth.Value = trackBarBlockWidth.Value;
            }
            else if (sender == trackBarBlockHeight)
            {
                numericUpDownBlockHeight.Value = trackBarBlockHeight.Value;
            }
            else if (sender == trackBarOffsetX)
            {
                numericUpDownOffsetX.Value = trackBarOffsetX.Value;
            }
            else if (sender == trackBarOffsetY)
            {
                numericUpDownOffsetY.Value = trackBarOffsetY.Value;
            }
            else if (sender == trackBarMinWidth)
            {
                numericUpDownMinWidth.Value = trackBarMinWidth.Value;
            }

            isBlockAdjusting = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < 128; i++)
            {
                sb.Append(Convert.ToChar(i));
            }
            this.richTextBoxCharInput.Text = sb.ToString();
        }
        
    }
}
