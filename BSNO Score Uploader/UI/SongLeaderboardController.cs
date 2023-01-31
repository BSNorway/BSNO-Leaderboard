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
        private JObject totalScoreData;
        private List<WeeklySongsObject> weeklySongsList;
        private int weeklySongsListIndex = 0;
        private string currentRankCatagory = "none";

        protected override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            overallList.data.Clear();
            mapList.data.Clear();
            totalScoreData = await GetTotalScores();
            weeklySongsList = await GetWeeklySongs();
            if (weeklySongsList[weeklySongsListIndex].songName.Length <= 50)
            {
                clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName}\n[{weeklySongsList[weeklySongsListIndex].diff}]\n({weeklySongsList[weeklySongsListIndex].type})";
            }
            else
            {
                clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName.Substring(0, 50)}\n[{weeklySongsList[weeklySongsListIndex].diff}]\n({weeklySongsList[weeklySongsListIndex].type})";
            }

        }

        #region RankButtons

        [UIComponent("topRankText")]
        private TextMeshProUGUI topRankText;

        [UIAction("top10btn")]
        private async Task Top10BtnActionAsync()
        {
            topRankText.text = "TOP 10";
            currentRankCatagory = "top10";
            await UpdateOverallLeaderboard("getTop10UsersPoints");
            mapList.data.Clear();
        }

        [UIAction("top25btn")]
        private async Task Top25BtnActionAsync()
        {
            topRankText.text = "TOP 25";
            currentRankCatagory = "top25";
            await UpdateOverallLeaderboard("getTop25UsersPoints");
            mapList.data.Clear();
        }

        [UIAction("top50btn")]
        private async Task Top50BtnActionAsync()
        {
            topRankText.text = "TOP 50";
            currentRankCatagory = "top50";
            await UpdateOverallLeaderboard("getTop50UsersPoints");
            mapList.data.Clear();
        }

        [UIAction("top100btn")]
        private async Task Top100BtnActionAsync()
        {
            topRankText.text = "TOP 51+";
            currentRankCatagory = "top100";
            await UpdateOverallLeaderboard("getTop100UsersPoints");
            mapList.data.Clear();
        }

        private async Task UpdateOverallLeaderboard(string apiEndpoint)
        {
            overallList.data.Clear();
            string response = await GetAsync($"{Config.webserverUrl}/api/v2/{apiEndpoint}");
            if (response == null)
            {
                Console.WriteLine("Error. No response!");
            }

            List<LeaderboardDataObject> dataList = new List<LeaderboardDataObject>();
            JObject data = JObject.Parse(response);
            foreach (var item in data["users"])
            {
                LeaderboardDataObject c = new LeaderboardDataObject
                {
                    username = item["username"].ToString(),
                    userId = item["userId"].ToString(),
                    WP = Int32.Parse(item["WP"].ToString())
                };
                dataList.Add(c);
            }
            overallList.data.AddRange(Enumerable.Range(0, dataList.Count).Select(i =>
            {
                string rawFirstListLine = $"   {i + 1}#  {dataList[i].username}";
                string rawSecondListLine = $"{dataList[i].WP} WP   ";
                string combinedString = rawFirstListLine + rawSecondListLine;
                int num = 80 - combinedString.Length;

                string spaceString = " ";
                for (int ind = 0; ind < num - 1; ind++)
                {
                    spaceString += " ";
                }
                string listLine = rawFirstListLine + spaceString + rawSecondListLine;
                return new CustomListTableData.CustomCellInfo(listLine);
            }).ToList());
            overallList.tableView?.ReloadData();
        }

        class LeaderboardDataObject
        {
            public string username;
            public string userId;
            public int WP;
        }

        #endregion

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
                if (weeklySongsList[weeklySongsListIndex].songName.Length <= 40)
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName}\n[{weeklySongsList[weeklySongsListIndex].diff}]\n({weeklySongsList[weeklySongsListIndex].type})";
                }
                else
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName.Substring(0, 40)}\n[{weeklySongsList[weeklySongsListIndex].diff}]\n({weeklySongsList[weeklySongsListIndex].type})";
                }
            }
        }
        [UIAction("listRight")]
        private void ClickListRight()
        {
            if (weeklySongsListIndex != weeklySongsList.Count - 1)
            {
                weeklySongsListIndex++;
                if (weeklySongsList[weeklySongsListIndex].songName.Length <= 40)
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName}\n[{weeklySongsList[weeklySongsListIndex].diff}]\n({weeklySongsList[weeklySongsListIndex].type})";
                }
                else
                {
                    clickTxt.text = $"{weeklySongsList[weeklySongsListIndex].songName.Substring(0, 40)}\n[{weeklySongsList[weeklySongsListIndex].diff}]\n({weeklySongsList[weeklySongsListIndex].type})";
                }
            }
        }

        #endregion

        #region TopLeaderboards

        [UIComponent("overallList")]
        private CustomListTableData overallList;
        [UIComponent("mapList")]
        private CustomListTableData mapList;

        [UIAction("mapListUp")]
        private void ClickListUp25()
        {
            List<TableCell> tableCellList = mapList.tableView.visibleCells.ToList();
            if (tableCellList[0].idx == 3)
            {
                mapList.tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, true);
                return;
            }
            mapList.tableView.ScrollToCellWithIdx(tableCellList[0].idx - 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("mapListDown")]
        private void ClickListDown25()
        {
            List<TableCell> tableCellList = mapList.tableView.visibleCells.ToList();
            mapList.tableView.ScrollToCellWithIdx(tableCellList[0].idx + 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("overallListUp")]
        private void ClickListUp26()
        {
            List<TableCell> tableCellList = overallList.tableView.visibleCells.ToList();
            if (tableCellList[0].idx == 3)
            {
                overallList.tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, true);
                return;
            }
            overallList.tableView.ScrollToCellWithIdx(tableCellList[0].idx - 4, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("overallListDown")]
        private void ClickListDown26()
        {
            List<TableCell> tableCellList = overallList.tableView.visibleCells.ToList();
            overallList.tableView.ScrollToCellWithIdx(tableCellList[0].idx + 4, TableView.ScrollPositionType.Beginning, true);
        }

        #endregion

        #region GetMethods

        private async Task<List<WeeklySongsObject>> GetWeeklySongs()
        {
            string response = await GetAsync($"{Config.webserverUrl}/api/v2/getWeeklyMaps");
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
            string response = await GetAsync($"{Config.webserverUrl}/api/v2/getTopUsers");
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

        private Task<List<UserObject>> ParseJsonToObject(JToken data, string hash)
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
                    return Task.FromResult(userClassList);
                }
            }
            return Task.FromResult(userClassList);
        }

        private async void UpdateLeaderboards()
        {
            if (currentRankCatagory == "none") return; // Dont do anything if no catagory has been chosen
            mapList.data.Clear();
            List<UserObject> userMapList = await ParseJsonToObject(totalScoreData[currentRankCatagory], weeklySongsList[weeklySongsListIndex].hash);
            mapList.data.AddRange(Enumerable.Range(0, userMapList.Count).Select(i =>
            {
                string rawFirstListLine = $"   {i + 1}#  {userMapList[i].username}";
                string rawSecondListLine = $"{userMapList[i].score} --- {userMapList[i].scorePr + "%"} --- {userMapList[i].WP} WP   ";
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
            mapList.tableView?.ReloadData();
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
