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

namespace BSNO_Score_Uploader.UI.Views
{
    [ViewDefinition("BSNO_Score_Uploader.UI.Views.Top26Leaderboard.bsml")]
    public class Top26Controller : BSMLAutomaticViewController
    {
        private string webServerUrl = "http://84.212.119.6:8260";

        [UIComponent("list26")]
        private CustomListTableData list;

        [UIAction("listUp")]
        private void ClickListUp()
        {
            List<TableCell> tableCellList = list.tableView.visibleCells.ToList();
            if (tableCellList[0].idx - 6 > 0) return;
            list.tableView.ScrollToCellWithIdx(tableCellList[0].idx - 6, TableView.ScrollPositionType.Beginning, true);
        }
        [UIAction("listDown")]
        private void ClickListDown()
        {
            List<TableCell> tableCellList = list.tableView.visibleCells.ToList();
            if (tableCellList[0].idx + 6 >= list.tableView.numberOfCells) return;
            list.tableView.ScrollToCellWithIdx(tableCellList[0].idx + 6, TableView.ScrollPositionType.Beginning, true);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            LoadLeaderboard();
        }

        private async void LoadLeaderboard()
        {
            list.data.Clear();
            string response = await GetAsync($"{webServerUrl}/getTop26UsersPoints");
            if (response == null)
            {
                Console.WriteLine("Error. No response!");
            }

            List<LeaderboardDataObject> dataList = new List<LeaderboardDataObject>();
            JObject data = JObject.Parse(response);
            foreach (var item in data["lead"])
            {
                LeaderboardDataObject c = new LeaderboardDataObject
                {
                    username = item["username"].ToString(),
                    userId = item["userId"].ToString(),
                    WP = Int32.Parse(item["WP"].ToString())
                };
                dataList.Add(c);
            }
            list.data.AddRange(Enumerable.Range(0, dataList.Count).Select(i =>
            {
                string rawFirstListLine = $"   {i + 1}#  {dataList[i].username}";
                string rawSecondListLine = $"{dataList[i].WP} WP   ";
                string combinedString = rawFirstListLine + rawSecondListLine;
                int num = 100 - combinedString.Length;

                string spaceString = " ";
                for (int ind = 0; ind < num - 1; ind++)
                {
                    spaceString += " ";
                }
                string listLine = rawFirstListLine + spaceString + rawSecondListLine;
                return new CustomListTableData.CustomCellInfo(listLine);
            }).ToList());
            list.tableView?.ReloadData();
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
}
