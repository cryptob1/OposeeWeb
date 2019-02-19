using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OposeeLibrary.Utilities
{
    public static class AesCryptography
    {
        static private ICryptoTransform rijndaelDecryptor;
        static private ICryptoTransform rijndaelEncryptor;
        static private byte[] rawSecretKey = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                              0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};


        //public static string EncryptBlowFish(string toEncrypt)
        //{
        //    var settingsReader = new AppSettingsReader();
        //    var key = (string)settingsReader.GetValue("passwordKey", typeof(String));
        //    Blowfish blowfish = new Blowfish(key);
        //    var returnString = blowfish.encryptString(toEncrypt);
        //    return returnString;
        //}

        //public static string DecryptBlowFish(string toDecrypt)
        //{
        //    var settingsReader = new AppSettingsReader();
        //    var key = (string)settingsReader.GetValue("passwordKey", typeof(String));
        //    var blowfish = new Blowfish(key);
        //    var returnString = blowfish.decryptString(toDecrypt);
        //    return returnString;
        //}

        
        static AesCryptography()
        {
            //SecurityKey = "123456789abcdef";
            byte[] passwordKey = EncodeDigest("Oposee99!!@#GreatLocation%^");
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndaelDecryptor = rijndael.CreateDecryptor(passwordKey, rawSecretKey);
            rijndaelEncryptor = rijndael.CreateEncryptor(passwordKey, rawSecretKey);
        }

        static public string Decrypt(string encryptedBase64)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedBase64);
            byte[] newClearData = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            if (encryptedBase64 != "")
            {
                newClearData = rijndaelDecryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            }
            return Encoding.ASCII.GetString(newClearData);
        }

        static private byte[] EncodeDigest(string text)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(text);
            return x.ComputeHash(data);
        }

        static public string Encrypt(string plainText)
        {
            var obj = Encoding.ASCII.GetBytes(plainText);
            byte[] newClearData = rijndaelEncryptor.TransformFinalBlock(obj, 0, obj.Length);
            return Convert.ToBase64String(newClearData);
        }
    }
}
