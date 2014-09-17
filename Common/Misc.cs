using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MosFontTool.Common
{
    class Misc
    {
        public static void MsgErr(string message)
        {
            MessageBox.Show(message, Properties.Resources.Caption_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void MsgInf(string message)
        {
            MessageBox.Show(message, Properties.Resources.Caption_info, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static Color parseColor(string value)
        {
            value = value.PadLeft(6, '0');
            if (value.Length == 7)
            {
                value = '0' + value;
            }
            else if (value.Length == 6)
            {
                value = "ff" + value;
            }
            return Color.FromArgb(
                Convert.ToInt16(value.Substring(0, 2), 16),
                Convert.ToInt16(value.Substring(2, 2), 16),
                Convert.ToInt16(value.Substring(4, 2), 16),
                Convert.ToUInt16(value.Substring(6), 16));
        }

        public static bool isHexColorKeypressAllowed(TextBox textbox, char c)
        {
            if (c != '\b' && c != '\x10' && !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
            {
                return false;
            }
            else if (c != '\b' && textbox.Text.Length == 8 && textbox.SelectionLength < 2)
            {
                return false;
            }

            return true;
        }

        public static bool isFilled(Control[] controls, string[] messages)
        {
            int pos = 0;
            StringBuilder sb = new StringBuilder();
            foreach(Control ctrl in controls)
            {
                if (ctrl is ListBox)
                {
                    if (((ListBox)ctrl).SelectedIndex == -1)
                    {
                        sb.AppendLine(messages[pos]);
                    }
                }
                else if (ctrl is TextBox)
                {
                    if (((TextBox)ctrl).Text.Length == 0)
                    {
                        sb.AppendLine(messages[pos]);
                    }
                }
                else
                {
                    // do nothing
                }
                pos++;
            }

            if (sb.Length > 0)
            {
                Misc.MsgInf(sb.ToString());
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool isBlackLikely(Color color)
        {
            int gray = (int)(color.R * .3 + color.G * .59 + color.B * .11);
            return gray > 0x80;
        }

        public static int getGrayScale(Color color)
        {
            int gray = (int)(color.R * .3 + color.G * .59 + color.B * .11);
            return (byte)gray;
        }

        public static UInt16 getRGB565(Color color)
        {
            int val = color.B >> 3 << 11;
            val |= color.G >> 2 << 5;
            val |= color.R >> 3;

            return Convert.ToUInt16(val);
        }

        public static UInt16 getRGB565P(Color color)
        {
            int val = color.R >> 3 << 11;
            val |= color.G >> 2 << 5;
            val |= color.B >> 3;

            return Convert.ToUInt16(val);
        }


    }
}
