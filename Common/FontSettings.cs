using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace MosFontTool.Common
{
    public class FontSettings : IDisposable
    {
        Color gs_fcolor, gs_bgcolor;
        Font gs_font;
        TextRenderingHint gs_render;
        TextFormatFlags gs_format;
        int gs_blockWidth, gs_blockHeight;
        int gs_offsetX, gs_offsetY;
        string gs_chars;
        bool gs_autoFixWidth;
        int gs_minWidth;

        public Color Forecolor
        {
            get { return gs_fcolor; }
            set { gs_fcolor = value; }
        }

        public int MinWidth
        {
            get { return gs_minWidth; }
            set { gs_minWidth = value; }
        }

        public bool AutoFixWidth
        {
            set { gs_autoFixWidth = value; }
            get { return gs_autoFixWidth; }
        }

        public Color BackColor
        {
            set { gs_bgcolor=value; }
            get { return gs_bgcolor; }
        }

        public Font Font
        {
            get { return gs_font; }
            set { gs_font = value; }
        }

        public TextRenderingHint RenderingHint
        {
            get { return gs_render; }
            set { gs_render = value; }
        }

        public TextFormatFlags TextFormatFlags
        {
            get { return gs_format; }
            set { gs_format = value | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding; }
        }

        public int BlockWidth
        {
            get { return gs_blockWidth; }
            set { gs_blockWidth = value; }
        }

        public int BlockHeight
        {
            get { return gs_blockHeight; }
            set { gs_blockHeight = value; }
        }

        public int OffsetX
        {
            get { return gs_offsetX; }
            set { gs_offsetX = value; }
        }

        public int OffsetY
        {
            get { return gs_offsetY; }
            set { gs_offsetY = value; }
        }

        public string Chars
        {
            get { return gs_chars; }
            set { gs_chars = value; }
        }

        public void Dispose()
        {
            if (Font != null)
            {
                Font.Dispose();
            }
        }

        public void SetForeColor(string hexValue)
        {
            Forecolor = Misc.parseColor(hexValue);
        }

        public void SetBackColor(string hexValue)
        {
            BackColor = Misc.parseColor(hexValue);
        }

        public void SetFont(object name, FontStyle style, object size)
        {
            FontFamily ff = new FontFamily(name.ToString());
            Font = new Font(ff, Int16.Parse(size.ToString()), style, GraphicsUnit.Pixel);
        }

        public void SetBlockSize(object width, object height)
        {
            BlockWidth = Convert.ToInt16(width);
            BlockHeight = Convert.ToInt16(height);
        }

        public void SetOffset(object x, object y)
        {
            OffsetX = Convert.ToInt16(x);
            OffsetY = Convert.ToInt16(y);
        }

        public Rectangle getDrawRect()
        {
            Rectangle rectDraw = new Rectangle(0, 0, BlockWidth, BlockHeight);
            if (OffsetX > 0)
            {
                rectDraw.Offset(OffsetX, 0);
            }
            if (OffsetY > 0)
            {
                rectDraw.Offset(0, OffsetY);
            }
            return rectDraw;
        }

        public Rectangle getCopyRect()
        {
            Rectangle rectCopy = new Rectangle(0, 0, BlockWidth, BlockHeight);
            if (OffsetX < 0)
            {
                rectCopy.Offset(-OffsetX, 0);
            }
            if (OffsetY < 0)
            {
                rectCopy.Offset(0, -OffsetY);
            }
            return rectCopy;
        }

        public void processChars(bool ignoreNewLine, bool insertNewLine, bool sort, bool filter)
        {
            if (ignoreNewLine)
            {
                Chars = Regex.Replace(Chars, @"[\r\n]", "");
            }
            else if (insertNewLine)
            {
                Chars += "\r\n";
            }

            if (sort)
            {
                char[] array = Chars.ToCharArray();
                Array.Sort(array);

                if (filter)
                {
                    array = array.Distinct().ToArray();
                }
                Chars = new string(array);
            }
        }

            
    }
}
