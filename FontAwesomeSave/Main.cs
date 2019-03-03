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
                imgIco.Image = (Image)bitmap;
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
                        switch (part[0])
                        {
                            case "fas":
                                Type = "solid";
                                break;
                            case "far":
                                Type = "regular";
                                break;
                            case "fab":
                                Type = "brands";
                                break;
                        }
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

            string outImgFile = txtPath.Text + "\\" + Name +  "_" + txtSize.Text + "_" + pnlColor.BackColor.R.ToString("X2") + pnlColor.BackColor.G.ToString("X2") + pnlColor.BackColor.B.ToString("X2") + ".png";


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

            for (int Xcount = 0; Xcount < bitmapTemp.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < bitmapTemp.Height; Ycount++)
                {
                    if (bitmapTemp.GetPixel(Xcount, Ycount).A > 0)
                    {
                        bitmapOut.SetPixel(Xcount + DifX, Ycount + DifY, bitmapTemp.GetPixel(Xcount, Ycount));
                    }
                }
            }
            if (System.IO.File.Exists(outImgFile))
                System.IO.File.Delete(outImgFile);

            Clipboard.Clear();
            Clipboard.SetImage(bitmapOut);

            bitmapOut.Save(outImgFile, ImageFormat.Png);

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
    }
}
