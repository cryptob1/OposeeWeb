using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OposeeLibrary.Utilities
{
    public static class Helper
    {
        


        public static void CreateDirectories(string _dir)
        {
            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }
        }

        public static void DeleteDirectory(string _dir, bool Subdirectory)
        {
            if (Directory.Exists(_dir))
            {
                Directory.Delete(_dir, Subdirectory);
            }
        }

        public static void DeleteFiles(string _dir)
        {
            if (Directory.Exists(_dir))
            {
                Array.ForEach(Directory.GetFiles(_dir), File.Delete);
            }
        }

        public static List<string> GetFiles(string _dir)
        {
            List<string> _lstFiles = new List<string>();

            if (Directory.Exists(_dir))
            {
                string[] filespath = Directory.GetFiles(_dir, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string _file in filespath)
                {
                    _lstFiles.Add(_file.ToString());
                }
            }
            return _lstFiles;
        }


        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        
        public static bool IsValidEmail(string emailAddress)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                  + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                  + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                  + @"[a-zA-Z]{2,}))$";
            System.Text.RegularExpressions.Regex reStrict = new System.Text.RegularExpressions.Regex(patternStrict);
            bool isStrictMatch = reStrict.IsMatch(emailAddress);
            return isStrictMatch;
        }

        public static bool IsValidPattern(string vToValidate, string vRegex)
        {
            string patternStrict = vRegex;
            System.Text.RegularExpressions.Regex reStrict = new System.Text.RegularExpressions.Regex(patternStrict);
            bool isStrictMatch = reStrict.IsMatch(vToValidate);
            return isStrictMatch;
        }
        
    }
}
