using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CSVEditor
{
    class CSVManager
    {
        public struct Record
        {
            public string name;
            public string score;
        }

        static string leaderboardDataPath = @"Data\Data1.dat";

        private static string encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("AISUO17"));
                //Always release the resources and flush data
                //of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes("AISUO17");

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static string decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("AISUO17"));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes("AISUO17");
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private static List<Record> loadLeaderboard()
        {
            List<Record> leaderboard = new List<Record>();

            // Error caused when the code could not locate target file is now fixed.
            if (!Directory.Exists(@"Data\")) Directory.CreateDirectory(@"Data\");
            if (!File.Exists(leaderboardDataPath)) saveLeaderboard(leaderboard);

            using (StreamReader sr = new StreamReader(leaderboardDataPath))
            {
                string headerLine = decrypt(sr.ReadLine(), true);
                string line = "";

                while ((line = sr.ReadLine()) != null)
                {
                    line = decrypt(line, true);
                    string[] tokens = line.Split(';');

                    var r = new Record();
                    r.name = tokens[0];
                    r.score = tokens[1];

                    leaderboard.Add(r);
                }
            }

            return leaderboard;
        }

        private static void saveLeaderboard(List<Record> oldLeaderboard)
        {
            var leaderboard = oldLeaderboard.OrderByDescending(x => int.Parse(x.score));

            using (FileStream fs = new FileStream(leaderboardDataPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(encrypt("name;score", true));

                    foreach(var r in leaderboard)
                    {
                        writer.WriteLine(encrypt(r.name + ";" + r.score, true));
                    }
                }
            }
        }

        public static void addRecord(string name, int score)
        {
            var leaderboard = loadLeaderboard();

            var r = new Record();
            r.name = name;
            r.score = score.ToString();
            leaderboard.Add(r);

            saveLeaderboard(leaderboard);
        }

        public static List<Record> getTop10Leaderboard()
        {
            var leaderboard = loadLeaderboard();
            var top10 = new List<Record>();
            var count = 0;

            foreach(var r in leaderboard)
            {
                if (count >= 10) break;

                top10.Add(r); count++;
            }

            // Add dummy records to complete 10 list.
            for (int i = 0; i < 10 - count; i++)
            {
                var dummy = new Record();
                dummy.name = null;
                dummy.score = null;

                top10.Add(dummy);
            }

            return top10;
        }
    }
}