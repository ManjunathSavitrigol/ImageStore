using System;
using System.Collections.Generic;
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
    }
}
