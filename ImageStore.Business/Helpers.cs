using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImageStore.Business
{
    public static class Helpers
    {
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "M9A3N8J0U0N6A4T3H4";
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                if (cipherText != "" && cipherText != null)
                {
                    string text = cipherText.Trim();
                    Int32 data = text.Length % 4;
                    if (data > 0)
                        text += new string('=', 4 - data);
                    string EncryptionKey = "M9A3N8J0U0N6A4T3H4";
                    byte[] cipherBytes = Convert.FromBase64String(text);
                    using (Aes encryptor = Aes.Create())
                    {
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherBytes, 0, cipherBytes.Length);
                                cs.Close();
                            }
                            cipherText = Encoding.Unicode.GetString(ms.ToArray());
                        }
                    }
                }

            }
            catch (Exception)
            {
                return cipherText;
            }

            return cipherText;
        }       

        public static string SaveFile(HttpPostedFileBase file, string folder, string filename)
        {
            try
            {
                string imagesPath = System.Web.HttpContext.Current.Server.MapPath($"~/{folder}"); // Or file save folder, etc.
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }
                string extension = Path.GetExtension(file.FileName);
                string newFileName = $"{filename}{extension}";
                string saveToPath = Path.Combine(imagesPath, newFileName);
                file.SaveAs(saveToPath);
                return "~/" + folder +"/"+ filename + extension;
            }
            catch 
            {
            }
            return "";
        }

        public static string CompressAndSaveImage(System.Drawing.Image inputImage, int quality, string folder, string filename, int width, string isWatermarkEnabled = "false")
        {
            try
            {
                using (Bitmap mybitmap = new Bitmap(inputImage, width, CalculateHeight(width, CalculateAspectRatio(inputImage.Width, inputImage.Height))))
                {
                    
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    System.Drawing.Imaging.Encoder myencoder = System.Drawing.Imaging.Encoder.Quality;

                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myencoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;


                    //check if watermarking is enabled

                    if(isWatermarkEnabled == "true")
                    {
                        Graphics graphicimage = Graphics.FromImage(mybitmap);

                        //watermarking images by size
                        StringFormat stringformat = new StringFormat();
                        stringformat.Alignment = StringAlignment.Far;
                        Color stringcolor = ColorTranslator.FromHtml("#ff0000");
                        string stringonimage = width + "";

                        graphicimage.DrawString(stringonimage, new Font("arail", 20, FontStyle.Regular), new SolidBrush(stringcolor), new Point(268, 245), stringformat);

                    }

                    //create folder if not created
                    string imagesPath = System.Web.HttpContext.Current.Server.MapPath($"~/{folder}"); // Or file save folder, etc.
                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }
                    //string extension = Path.GetExtension();
                    string newFileName = $"{filename}.JPEG";
                    string saveToPath = Path.Combine(imagesPath, newFileName);


                    mybitmap.Save(saveToPath, jpgEncoder, myEncoderParameters);
                    return "~/" + folder + "/" + filename + ".JPEG";
                }
            }
            catch { }
            return "";
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        private static int CalculateHeight(int width, string aspectRatio)
        {
            string[] ratioParts = aspectRatio.Split(':');
            if (ratioParts.Length != 2 || !int.TryParse(ratioParts[0], out int aspectWidth) || !int.TryParse(ratioParts[1], out int aspectHeight))
            {
                throw new ArgumentException("Invalid aspect ratio format. Use 'width:height'.");
            }

            double ratio = (double)aspectHeight / aspectWidth;
            return (int)(width * ratio);
        }

        private static string CalculateAspectRatio(int width, int height)
        {
            int gcd = GCD(width, height);
            int aspectWidth = width / gcd;
            int aspectHeight = height / gcd;

            return $"{aspectWidth}:{aspectHeight}";
        }

        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static void WriteErrorLog(string description)
        {
            try
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("~\\LogFile");
                string strFile = path + @"\error" + DateTime.Now.Date.ToString("ddMMyyyy") + ".txt";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var objStreamWriter = new StreamWriter(strFile, true);
                objStreamWriter.WriteLine("Error Message : " + description + "  Date:" + DateTime.Now);
                objStreamWriter.Dispose();

            }
            catch (Exception)
            {
            }
        }
    }
}
