using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FontAwesomeSave
{
    public partial class Main : Form
    {
        public Config Configuration;

        public string ConfiPath = AppDomain.CurrentDomain.BaseDirectory + "\\config.xml";

        public string Type = string.Empty;
        public string Name = string.Empty;

        public string ImageFile = string.Empty;

        public Main()
        {
            InitializeComponent();
            if (!System.IO.File.Exists(ConfiPath))
            {
                Configuration = new Config();
                Config.Save(ConfiPath, Configuration);
            }
            else
            {
                Configuration = Config.Open(ConfiPath);
            }
            txtPath.Text = Configuration.DestiniPath;
            txtSize.Text = Configuration.Size;
            txtColor.Text = Configuration.Color;
        }

        private void pnlColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pnlColor.BackColor = colorDialog.Color;

                txtColor.Text = ColorTranslator.ToHtml(Color.FromArgb(colorDialog.Color.ToArgb()));

                UpdateImage();
            }
        }

        protected void UpdateImage()
        {
            if (!string.IsNullOrEmpty(ImageFile) && System.IO.File.Exists(ImageFile))
            {
                SvgDocument svgDocument = SvgDocument.Open(ImageFile);
                Bitmap bitmap = svgDocument.Draw();
                //for (int Xcount = 0; Xcount < bitmap.Width; Xcount++)
                //{
                //    for (int Ycount = 0; Ycount < bitmap.Height; Ycount++)
                //    {
                //        if (bitmap.GetPixel(Xcount, Ycount).A > 0)
                //        {
                //            bitmap.SetPixel(Xcount, Ycount, Color.FromArgb(bitmap.GetPixel(Xcount, Ycount).A, pnlColor.BackColor.R, pnlColor.BackColor.G, pnlColor.BackColor.B));
                //        }
                //    }
                //}
                imgIco.Image = bitmap;
            }
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog.SelectedPath;
                Configuration.DestiniPath = txtPath.Text;
                Config.Save(ConfiPath, Configuration);
            }
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Configuration.OrigePath = folderBrowserDialog.SelectedPath;
                Config.Save(ConfiPath, Configuration);
            }
        }

        private void txtIco_TextChanged(object sender, EventArgs e)
        {
            //<i class="fab fa-500px"></i>
            if (!string.IsNullOrEmpty(Configuration.OrigePath))
            {
                if (txtIco.Text.Contains("<i class=\"") && txtIco.Text.Contains("\"></i>"))
                {
                    string[] part = txtIco.Text.Replace("<i class=\"", string.Empty).Replace("\"></i>", string.Empty).Split(' ');
                    if (part.Length == 2)
                    {
                        //switch (part[0])
                        //{
                        //    case "fas":
                        //        Type = "solid";
                        //        break;
                        //    case "far":
                        //        Type = "regular";
                        //        break;
                        //    case "fab":
                        //        Type = "brands";
                        //        break;
                        //    case "fal":
                        //        Type = "light";
                        //        break;
                        //    case "fad":
                        //        Type = "duotone";
                        //        break;
                        //}
                        Type = part[0].Replace("fa-", string.Empty);
                        Name = part[1].Replace("fa-", string.Empty);
                        ImageFile = Configuration.OrigePath + "\\svgs\\" + Type + "\\" + Name + ".svg";
                        UpdateImage();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Configuration.DestiniPath = txtPath.Text;
            Configuration.Size = txtSize.Text;
            Config.Save(ConfiPath, Configuration);

            string outImgFile = txtPath.Text + "\\" + Name + "_" + txtSize.Text + "_" + pnlColor.BackColor.R.ToString("X2") + pnlColor.BackColor.G.ToString("X2") + pnlColor.BackColor.B.ToString("X2");


            SvgDocument svgDocument = SvgDocument.Open(ImageFile);
            Bitmap bitmap = svgDocument.Draw();
            for (int Xcount = 0; Xcount < bitmap.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < bitmap.Height; Ycount++)
                {
                    if (bitmap.GetPixel(Xcount, Ycount).A > 0)
                    {
                        bitmap.SetPixel(Xcount, Ycount, Color.FromArgb(bitmap.GetPixel(Xcount, Ycount).A, pnlColor.BackColor.R, pnlColor.BackColor.G, pnlColor.BackColor.B));
                    }
                }
            }
            Bitmap bitmapTemp = null;
            int tamanho = int.Parse(txtSize.Text);
            if (bitmap.Width >= bitmap.Height)
            {
                bitmapTemp = new Bitmap(bitmap, new Size(tamanho, tamanho * bitmap.Height / bitmap.Width));
            }
            else
            {
                bitmapTemp = new Bitmap(bitmap, new Size(tamanho * bitmap.Width / bitmap.Height, tamanho));
            }
            Bitmap bitmapOut = new Bitmap(tamanho, tamanho);

            int DifX = (tamanho - bitmapTemp.Width) / 2;
            int DifY = (tamanho - bitmapTemp.Height) / 2;

            if (comboBox1.Text != "PNG")
            {
                for (int X = 0; X < bitmapOut.Width; X++)
                {
                    for (int Y = 0; Y < bitmapOut.Height; Y++)
                    {
                        bitmapOut.SetPixel(X, Y, this.BackColor);
                    }
                }
            }

            for (int Xcount = 0; Xcount < bitmapTemp.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < bitmapTemp.Height; Ycount++)
                {
                    if (bitmapTemp.GetPixel(Xcount, Ycount).A > 0)
                    {
                        bitmapOut.SetPixel(Xcount + DifX, Ycount + DifY, bitmapTemp.GetPixel(Xcount, Ycount));
                    }
                    else
                    {
                        if (comboBox1.Text != "PNG")
                        {
                            bitmapOut.SetPixel(Xcount + DifX, Ycount + DifY, this.BackColor);
                        }
                    }
                }
            }
            Clipboard.Clear();
            Clipboard.SetImage(bitmapOut);

            switch (comboBox1.Text)
            {
                case "PNG":
                    if (System.IO.File.Exists(outImgFile + ".png"))
                        System.IO.File.Delete(outImgFile + ".png");

                    bitmapOut.Save(outImgFile + ".png", ImageFormat.Png);
                    break;
                case "BMP":
                    if (System.IO.File.Exists(outImgFile + ".bmp"))
                        System.IO.File.Delete(outImgFile + ".bmp");

                    bitmapOut.Save(outImgFile + ".bmp", ImageFormat.Bmp);
                    break;
                case "JPEG":
                    if (System.IO.File.Exists(outImgFile + ".jpeg"))
                        System.IO.File.Delete(outImgFile + ".jpeg");

                    bitmapOut.Save(outImgFile + ".jpeg", ImageFormat.Jpeg);
                    break;
                default:
                    if (System.IO.File.Exists(outImgFile + ".png"))
                        System.IO.File.Delete(outImgFile + ".png");

                    bitmapOut.Save(outImgFile + ".png", ImageFormat.Png);
                    break;
            }



        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Configuration.DestiniPath = txtPath.Text;
            Configuration.Size = txtSize.Text;
            Configuration.Color = txtColor.Text;
            Config.Save(ConfiPath, Configuration);
        }

        private void pnlColor_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtColor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ColorConverter colorConverter = new ColorConverter();
                pnlColor.BackColor = (Color)colorConverter.ConvertFromString(txtColor.Text);
                UpdateImage();
            }
            catch
            {

            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            txtIco_TextChanged(sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                panel1.BackColor = colorDialog.Color;

                txtCorFundo.Text = ColorTranslator.ToHtml(Color.FromArgb(colorDialog.Color.ToArgb()));

                this.BackColor = colorDialog.Color;
            }
        }

        private void txtCorFundo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ColorConverter colorConverter = new ColorConverter();
                panel1.BackColor = (Color)colorConverter.ConvertFromString(txtCorFundo.Text);
                this.BackColor = panel1.BackColor;
            }
            catch
            {

            }
        }
    }
}
