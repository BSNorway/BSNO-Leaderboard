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
using TMPro;

namespace BSNO_Score_Uploader.UI
{
    [ViewDefinition("BSNO_Score_Uploader.UI.Views.song-leaderboard.bsml")]
    public class SongLeaderboardController : BSMLAutomaticViewController
    {
        private readonly string webServerUrl = "https://bung-bsno-challenge.herokuapp.com";
        private JObject totalScoreData;
        private List<WeeklySongsObject> weeklySongsList;
        private int weeklySongsListIndex = 0;

        protected override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            list25.data.Clear();
            list26.data.Clear();
            totalScoreData = await GetTotalScores();
            weeklySongsList = await GetWeeklySongs();
            if (weeklySongsList[weeklySongsListIndex].songName.Length <= 50)
            {
                clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName}\n({weeklySongsList[weeklySongsListIndex].type})";
            } else
            {
                clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName.Substring(0, 50)}\n({weeklySongsList[weeklySongsListIndex].type})";
            }
            
        }

        #region SelectionUI

        [UIComponent("clickTxt")]
        private TextMeshProUGUI clickTxt;

        [UIAction("selectTxtClick")]
        private void SelectTxtClick() => UpdateLeaderboards();

        [UIAction("listLeft")]
        private void ClickListLeft()
        {
            if (weeklySongsListIndex != 0)
            {
                weeklySongsListIndex--;
                if (weeklySongsList[weeklySongsListIndex].songName.Length <= 50)
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName}\n({weeklySongsList[weeklySongsListIndex].type})";
                }
                else
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName.Substring(0, 50)}\n({weeklySongsList[weeklySongsListIndex].type})";
                }
            }
        }
        [UIAction("listRight")]
        private void ClickListRight()
        {
            if (weeklySongsListIndex != weeklySongsList.Count - 1)
            {
                weeklySongsListIndex++;
                if (weeklySongsList[weeklySongsListIndex].songName.Length <= 50)
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName}\n({weeklySongsList[weeklySongsListIndex].type})";
                }
                else
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName.Substring(0, 50)}\n({weeklySongsList[weeklySongsListIndex].type})";
                }
            }
        }

        #endregion

        #region TopLeaderboards

        [UIComponent("list25")]
        private CustomListTableData list25;
        [UIComponent("list26")]
        private CustomListTableData list26;

        [UIAction("listUp25")]
        private void ClickListUp25()
        {
            List<TableCell> tableCellList = list25.tableView.visibleCells.ToList();
            if (tableCellList[0].idx == 3)
            {
                list25.tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, true);
                return;
            }
            list25.tableView.ScrollToCellWithIdx(tableCellList[0].idx - 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("listDown25")]
        private void ClickListDown25()
        {
            List<TableCell> tableCellList = list25.tableView.visibleCells.ToList();
            list25.tableView.ScrollToCellWithIdx(tableCellList[0].idx + 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("listUp26")]
        private void ClickListUp26()
        {
            List<TableCell> tableCellList = list26.tableView.visibleCells.ToList();
            if (tableCellList[0].idx == 3)
            {
                list26.tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, true);
                return;
            }
            list26.tableView.ScrollToCellWithIdx(tableCellList[0].idx - 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("listDown26")]
        private void ClickListDown26()
        {
            List<TableCell> tableCellList = list26.tableView.visibleCells.ToList();
            list26.tableView.ScrollToCellWithIdx(tableCellList[0].idx + 4, TableView.ScrollPositionType.Beginning, true);
        }

        #endregion

        #region GetMethods

        private async Task<List<WeeklySongsObject>> GetWeeklySongs()
        {
            string response = await GetAsync($"{webServerUrl}/v2/getWeeklyMaps");
            if (response == null)
            {
                Console.WriteLine("Error. No response!");
                return new List<WeeklySongsObject>();
            }

            List<WeeklySongsObject> songList = new List<WeeklySongsObject>();
            JObject data = JObject.Parse(response);
            foreach (var song in data["songs"])
            {
                WeeklySongsObject weeklySongs = new WeeklySongsObject();
                weeklySongs.hash = song["hash"].ToString();
                weeklySongs.diff = song["diff"].ToString();
                weeklySongs.type = song["type"].ToString();
                weeklySongs.points = song["points"].ToString();
                weeklySongs.songName = song["songName"].ToString();
                songList.Add(weeklySongs);
            }
            return songList;
        }

        private async Task<JObject> GetTotalScores()
        {
            string response = await GetAsync($"{webServerUrl}/v2/getTopUsers");
            if (response == null)
            {
                Console.WriteLine("Error");
                return new JObject();
            }
            return JObject.Parse(response);
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

        #endregion

        private async Task<List<UserObject>> ParseJsonToObject(JToken data, string hash)
        {
            List<UserObject> userClassList = new List<UserObject>();
            foreach (var songs in data)
            {
                if (songs["hash"].ToString() == hash)
                {
                    foreach (var score in songs["scores"])
                    {
                        UserObject user = new UserObject
                        {
                            username = score["username"].ToString(),
                            score = Int32.Parse(score["score"].ToString()),
                            scorePr = score["scorePr"].ToString(),
                            WP = Int32.Parse(score["WP"].ToString())
                        };
                        userClassList.Add(user);
                    }
                    return userClassList;
                }
            }
            return userClassList;
        }

        private async void UpdateLeaderboards()
        {
            list25.data.Clear();
            list26.data.Clear();
            List<UserObject> userClassList25 = await ParseJsonToObject(totalScoreData["top25"], weeklySongsList[weeklySongsListIndex].hash);
            List<UserObject> userClassList26 = await ParseJsonToObject(totalScoreData["top26"], weeklySongsList[weeklySongsListIndex].hash);
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
        public string hash;
        public string diff;
        public string type;
        public string points;
        public string songName;
    }
}
