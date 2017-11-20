using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZSpriteTools
{
    public static class FileUtilities
    {
        public static bool IsFileLocked(string filename)
        {
            try
            {
                using (File.Open(filename, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);

                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }

        public static void WriteAllBytes(string filename, byte[] bytes)
        {
            if(IsFileLocked(filename))
            {
                MessageBox.Show($"File {filename} is locked. Do you have it open in another program?", "Error");
                return;
            }

            File.WriteAllBytes(filename, bytes);
        }

        public static void WriteAllText(string filename, string text)
        {
            if (IsFileLocked(filename))
            {
                MessageBox.Show($"File {filename} is locked. Do you have it open in another program?", "Error");
                return;
            }

            File.WriteAllText(filename, text);
        }
    }
}
