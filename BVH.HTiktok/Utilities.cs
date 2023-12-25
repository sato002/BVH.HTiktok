using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BVH.HTiktok
{
    public static class Utilities
    {
        private static Random rand = new Random();
        public static string RandomString(int length)
        {
            const string poolLower = "abcdefghijklmnopqrstuvwxyz";
            const string poolNum = "0123456789";
            const string poolUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string poolSymbol = "@$!";
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                int selectPool = i > 3 ? rand.Next(0, 5) % 4 : i;
                var c = new char();
                switch (selectPool)
                {
                    case 1: c = poolNum[rand.Next(0, poolNum.Length)]; break;
                    case 2: c = poolUpper[rand.Next(0, poolUpper.Length)]; break;
                    case 3: c = poolSymbol[rand.Next(0, poolSymbol.Length)]; break;
                    default: c = poolLower[rand.Next(0, poolLower.Length)]; break;
                }
                builder.Append(c);
            }
            return builder.ToString();
        }

        public static string RandomEmailSubfix(int length)
        {
            const string poolLower = "abcdefghijklmnopqrstuvwxyz";
            const string poolNum = "0123456789";
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                int selectPool = i > 1 ? rand.Next(0, 2) : i;
                var c = new char();
                switch (selectPool)
                {
                    case 1: c = poolNum[rand.Next(0, poolNum.Length)]; break;
                    default: c = poolLower[rand.Next(0, poolLower.Length)]; break;
                }
                builder.Append(c);
            }
            return builder.ToString();
        }

        private static string[] _allNames;
        public static string RandomName()
        {
            _allNames = _allNames ?? File.ReadAllLines("Assets/names.txt");
            Random rnd1 = new Random();
            return _allNames[rnd1.Next(_allNames.Length)];
        }

        public static async Task<string> ImageUrlToBase64(string imageUrl)
        {
            using (var httClient = new HttpClient())
            {
                var imageBytes = await httClient.GetByteArrayAsync(imageUrl);
                return Convert.ToBase64String(imageBytes);
            }
        }

    }
}
