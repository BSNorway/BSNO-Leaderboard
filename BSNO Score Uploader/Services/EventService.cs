using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BS_Utils.Utilities;
using Newtonsoft.Json;

namespace BSNO_Score_Uploader.Services
{
    public class EventService
    {
        public void Initialize()
        {
            BSEvents.levelCleared += BSEvents_levelLeft;
        }

        async private void BSEvents_levelLeft(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            if (arg1.gameMode != "Solo") return; // Do nothing if gamemode is not solo
            string path = Environment.CurrentDirectory + "/UserData/";
            if(File.Exists(path + "NjsFixer.json")) // Checks if njsfixer has been used to alter the map njs
            {
                NjsFixerConfig config = JsonConvert.DeserializeObject<NjsFixerConfig>(File.ReadAllText(path + "NjsFixer.json"));
                if (config.dontForceNJS == false)
                {
                    return; // If njs is forced then disable score upload
                }
            }

            bool firstTime = true;
            int i = 0;
            while (i < 3) // Attempt to upload score 3 times before stopping
            {
                switch (firstTime)
                {
                    case true:
                        bool result = await AttemptScoreUpload(arg1, arg2);
                        switch (result)
                        {
                            case true:
                                Console.WriteLine("BSNO-ScoreUploader: Score has been uploaded!");
                                return;
                            case false:
                                Console.WriteLine($"BSNO-ScoreUploader: Retry Nr. {i}");
                                i++;
                                firstTime = false;
                                break;
                        }
                        break;
                    case false:
                        await Task.Delay(20);
                        bool result2 = await AttemptScoreUpload(arg1, arg2);
                        switch (result2)
                        {
                            case true:
                                Console.WriteLine("BSNO-ScoreUploader: Score has been uploaded!");
                                return;
                            case false:
                                Console.WriteLine($"BSNO-ScoreUploader: Retry Nr. {i}");
                                i++;
                                firstTime = false;
                                break;
                        }
                        break;
                }
                
            }
        }

        async Task<bool> AttemptScoreUpload(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            try
            {
                if (arg2.gameplayModifiers.ghostNotes || arg2.gameplayModifiers.noArrows || arg2.gameplayModifiers.noBombs || arg2.gameplayModifiers.zenMode || arg2.gameplayModifiers.disappearingArrows || arg2.gameplayModifiers.songSpeed != GameplayModifiers.SongSpeed.Normal || arg2.gameplayModifiers.demoNoObstacles)
                {
                    // If modifiers is turned on then dont upload
                    return true;
                }

                UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
                string userId = userInfo.platformUserId;
                string username = userInfo.userName;
                string levelHashRaw = arg1.difficultyBeatmap.level.levelID;
                string levelHash = levelHashRaw.Substring(13);
                string songName = arg1.difficultyBeatmap.level.songName;
                string songDiff = arg1.difficultyBeatmap.difficultyRank.ToString();
                int totalNotes = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData.cuttableNotesType;
                DateTime currentDate = DateTime.Now;
                string modVersion = "0.0.3";

                var httpWebReq = (HttpWebRequest)WebRequest.Create("http://84.212.119.6:8260/json");
                httpWebReq.ContentType = "application/json";
                httpWebReq.Method = "POST";
                httpWebReq.Timeout = 400;

                using (var streamWriter = new StreamWriter(httpWebReq.GetRequestStream()))
                {
                    LevelResults levelResults = new LevelResults(userId, username, levelHash, songName, currentDate, arg2.modifiedScore, arg2.averageCutScore, arg2.maxCombo, arg2.missedCount, songDiff, totalNotes, modVersion);

                    streamWriter.Write(JsonConvert.SerializeObject(levelResults));
                }

                var httpResponse = (HttpWebResponse)httpWebReq.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
                return true;
            }
            catch
            {
                Console.WriteLine("BSNO-ScoreUploader: Error sending score");
                return false;
            }
        }
    }

    class LevelResults
    {
        public string userId;
        public string username;
        public string levelHash;
        public string songName;
        public DateTime currentDate;
        public int modifiedScore;
        public int averageCutScore;
        public int maxCombo;
        public int missedCount;
        public string songDiff;
        public int totalNotes;
        public string modVersion;

        public LevelResults(string userId, string username, string levelHash, string songName, DateTime currentDate, int modifiedScore, int averageCutScore, int maxCombo, int missedCount, string songDiff, int totalNotes, string modVersion)
        {
            this.userId = userId;
            this.username = username;
            this.levelHash = levelHash;
            this.songName = songName;
            this.currentDate = currentDate;
            this.modifiedScore = modifiedScore;
            this.averageCutScore = averageCutScore;
            this.maxCombo = maxCombo;
            this.missedCount = missedCount;
            this.songDiff = songDiff;
            this.totalNotes = totalNotes;
            this.modVersion = modVersion;
        }
    }

    class NjsFixerConfig
    {
        public bool dontForceNJS;
    }
}
