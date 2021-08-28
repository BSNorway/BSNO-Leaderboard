using System;
using System.Linq;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using HMUI;

namespace BSNO_Score_Uploader.UI
{
    [ViewDefinition("BSNO_Score_Uploader.UI.Views.song-leaderboard.bsml")]
    public class SongLeaderboardController : BSMLAutomaticViewController
    {
        private string webServerUrl = "http://84.212.119.6:8260";

        [UIAction("btn1Click")]
        private void ClickBtn1() => ClickBtn1Action();
        [UIAction("btn2Click")]
        private void ClickBtn2() => ClickBtn2Action();
        [UIAction("btn3Click")]
        private void ClickBtn3() => ClickBtn3Action();
        [UIAction("btn4Click")]
        private void ClickBtn4() => ClickBtn4Action();

        [UIComponent("list25")]
        private CustomListTableData list25;
        [UIComponent("list26")]
        private CustomListTableData list26;

        // List scrolling
        [UIAction("listUp25")]
        private void ClickListUp25()
        {
            List<TableCell> tableCellList = list25.tableView.visibleCells.ToList();
            if (tableCellList[0].idx - 4 > 0) return;
            list25.tableView.ScrollToCellWithIdx(tableCellList[0].idx - 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("listDown25")]
        private void ClickListDown25()
        {
            List<TableCell> tableCellList = list25.tableView.visibleCells.ToList();
            if (tableCellList[0].idx + 4 >= list25.tableView.numberOfCells) return;
            list25.tableView.ScrollToCellWithIdx(tableCellList[0].idx + 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("listUp26")]
        private void ClickListUp26()
        {
            List<TableCell> tableCellList = list26.tableView.visibleCells.ToList();
            if (tableCellList[0].idx - 4 > 0) return;
            list26.tableView.ScrollToCellWithIdx(tableCellList[0].idx - 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("listDown26")]
        private void ClickListDown26()
        {
            List<TableCell> tableCellList = list26.tableView.visibleCells.ToList();
            if (tableCellList[0].idx + 4 >= list26.tableView.numberOfCells) return;
            list26.tableView.ScrollToCellWithIdx(tableCellList[0].idx + 4, TableView.ScrollPositionType.Beginning, true);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            list25.data.Clear();
            list26.data.Clear();
            _ = GetWeeklySongs();
        }

        private async Task<WeeklySongsObject> GetWeeklySongs()
        {
            string response = await GetAsync($"{webServerUrl}/getWeeklyMaps");
            if (response == null)
            {
                Console.WriteLine("Error. No response!");
                WeeklySongsObject empty = new WeeklySongsObject();
                return empty;
            }

            JObject data = JObject.Parse(response);
            WeeklySongsObject weeklySongs = new WeeklySongsObject
            {
                currentAccSong = data["currentAccSong"].ToString(),
                currentAccSongDiff = data["currentAccSongDiff"].ToString(),
                currentMidSong = data["currentMidSong"].ToString(),
                currentMidSongDiff = data["currentMidSongDiff"].ToString(),
                currentSpeedSong = data["currentSpeedSong"].ToString(),
                currentSpeedSongDiff = data["currentSpeedSongDiff"].ToString(),
                currentFunnySong = data["currentFunnySong"].ToString(),
                currentFunnySongDiff = data["currentFunnySongDiff"].ToString()
            };
            return weeklySongs;
        }

        private void ClickBtn1Action()
        {
            UpdateLeaderboards("acc");
        }

        private void ClickBtn2Action()
        {
            UpdateLeaderboards("mid");
        }

        private void ClickBtn3Action()
        {
            UpdateLeaderboards("speed");
        }

        private void ClickBtn4Action()
        {
            UpdateLeaderboards("funny");
        }

        private async Task<List<UserObject>> ParseJsonToObject(JObject data, string songCat)
        {
            List<UserObject> userClassList = new List<UserObject>();
            foreach (var item in data[songCat])
            {
                UserObject c = new UserObject
                {
                    username = item["username"].ToString(),
                    score = Int32.Parse(item["score"].ToString()),
                    scorePr = item["scorePr"].ToString(),
                    WP = Int32.Parse(item["WP"].ToString())
                };
                userClassList.Add(c);
            }
            return userClassList;
        }

        private async void UpdateLeaderboards(string songCat)
        {
            list25.data.Clear();
            list26.data.Clear();
            string response = await GetAsync($"{webServerUrl}/getTopUsers");
            if (response == null)
            {
                Console.WriteLine("Error");
                return;
            }
            List<UserObject> userClassList25 = new List<UserObject>();
            List<UserObject> userClassList26 = new List<UserObject>();
            JObject data = JObject.Parse(response);
            switch (songCat)
            {
                case "acc":
                    userClassList25 = await ParseJsonToObject(data["top25"].ToObject<JObject>(), "acc");
                    userClassList26 = await ParseJsonToObject(data["top26"].ToObject<JObject>(), "acc");
                    break;
                case "mid":
                    userClassList25 = await ParseJsonToObject(data["top25"].ToObject<JObject>(), "mid");
                    userClassList26 = await ParseJsonToObject(data["top26"].ToObject<JObject>(), "mid");
                    break;
                case "speed":
                    userClassList25 = await ParseJsonToObject(data["top25"].ToObject<JObject>(), "speed");
                    userClassList26 = await ParseJsonToObject(data["top26"].ToObject<JObject>(), "speed");
                    break;
                case "funny":
                    userClassList25 = await ParseJsonToObject(data["top25"].ToObject<JObject>(), "funny");
                    userClassList26 = await ParseJsonToObject(data["top26"].ToObject<JObject>(), "funny");
                    break;
            }

            list25.data.AddRange(Enumerable.Range(0, userClassList25.Count).Select(i =>
            {
                string rawFirstListLine = $"   {i + 1}#  {userClassList25[i].username}";
                string rawSecondListLine = $"{userClassList25[i].score} --- {userClassList25[i].scorePr + "%"} --- {userClassList25[i].WP} WP   ";
                string combinedString = rawFirstListLine + rawSecondListLine;
                int num = 60 - combinedString.Length;

                string spaceString = " ";
                for (int ind = 0; ind < num - 1; ind++)
                {
                    spaceString += " ";
                }
                string listLine = rawFirstListLine + spaceString + rawSecondListLine;
                return new CustomListTableData.CustomCellInfo(listLine);
            }).ToList());

            list26.data.AddRange(Enumerable.Range(0, userClassList26.Count).Select(i =>
            {
                string rawFirstListLine = $"   {i + 1}#  {userClassList26[i].username}";
                string rawSecondListLine = $"{userClassList26[i].score} --- {userClassList26[i].scorePr + "%"} --- {userClassList26[i].WP} WP   ";
                string combinedString = rawFirstListLine + rawSecondListLine;
                int num = 60 - combinedString.Length;

                string spaceString = " ";
                for (int ind = 0; ind < num - 1; ind++)
                {
                    spaceString += " ";
                }
                string listLine = rawFirstListLine + spaceString + rawSecondListLine;
                return new CustomListTableData.CustomCellInfo(listLine);
            }).ToList());
            list25.tableView?.ReloadData();
            list26.tableView?.ReloadData();
        }

        private async Task<string> GetAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Timeout = 400;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }

    class UserObject
    {
        public string username;
        public int score;
        public string scorePr;
        public int WP;
    }

    class WeeklySongsObject
    {
        public string currentAccSong;
        public string currentAccSongDiff;
        public string currentMidSong;
        public string currentMidSongDiff;
        public string currentSpeedSong;
        public string currentSpeedSongDiff;
        public string currentFunnySong;
        public string currentFunnySongDiff;
    }
}
