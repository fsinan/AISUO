using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private static List<Record> loadLeaderboard()
        {
            List<Record> leaderboard = new List<Record>();

            // Error caused when the code could not locate target file is now fixed.
            if (!Directory.Exists(@"Data\")) Directory.CreateDirectory(@"Data\");
            if (!File.Exists(leaderboardDataPath)) saveLeaderboard(leaderboard);

            using (StreamReader sr = new StreamReader(leaderboardDataPath))
            {
                string headerLine = sr.ReadLine();
                string line = "";

                while ((line = sr.ReadLine()) != null)
                {
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
                    writer.WriteLine("name;score");

                    foreach(var r in leaderboard)
                    {
                        writer.WriteLine(r.name + ";" + r.score);
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