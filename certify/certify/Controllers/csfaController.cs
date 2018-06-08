using certify.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace certify.Controllers
{
    public class csfaController : ApiController
    {
        AitagDBContext _db = null;
        public csfaController()
        {
            _db = new AitagDBContext();
        }
        public clsImage Get(string barcode)
        {

            barcodeRule rule = _db.BarcodeRule.Where(e => e.barcode == barcode).SingleOrDefault();

            PointF Location = new PointF(170f, 655f);

            Graphics grfx = Graphics.FromHwnd(IntPtr.Zero);

            string imageFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"images\backgroup.png";
            Bitmap bitmap = (Bitmap)Image.FromFile(imageFilePath);//load the image file

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < barcode.ToArray().Length; i++)
                {
                    using (Font arialFont = new Font("Arial", 7))
                    {
                        if (i == rule.positionRule)
                        {
                            graphics.DrawString(barcode[i].ToString(), new Font("Arial", 7, FontStyle.Italic), Brushes.Red, Location);
                        }
                        else
                        {
                            graphics.DrawString(barcode[i].ToString(), arialFont, Brushes.Black, Location);
                        }


                        Location.X += grfx.MeasureString(barcode[i].ToString(), arialFont, 10).ToSize().Width + 10;
                    }
                }

            }


            //bitmap.Save(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"image\backgroup2.png");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ((Image)bitmap).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            clsImage image = new Models.clsImage();
            image.base64 = HttpContext.Current.Server.UrlEncode(Convert.ToBase64String(ms.ToArray()));
            return image;
         


        }

       
    }
}