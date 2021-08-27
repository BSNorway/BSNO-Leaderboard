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

namespace BSNO_Score_Uploader.UI.Views
{
    [ViewDefinition("BSNO_Score_Uploader.UI.Views.Top25Leaderboard.bsml")]
    public class Top25Controller : BSMLAutomaticViewController
    {
        private string webServerUrl = "http://84.212.119.6:8260";

        [UIComponent("list25")]
        private CustomListTableData list;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            LoadLeaderboard();
        }

        private async void LoadLeaderboard()
        {
            string response = await GetAsync($"{webServerUrl}/getTop25UsersPoints");
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
            Console.WriteLine(dataList[0]);
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

    class LeaderboardDataObject
    {
        public string username;
        public string userId;
        public int WP;
    }
}
