using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BS_Utils.Utilities;
using Newtonsoft.Json;
using Zenject;

namespace BSNO_Score_Uploader.Services
{
    public class EventService : IInitializable, IDisposable
    {
        public void Initialize()
        {
            BSEvents.levelCleared += BSEvents_levelLeft;
        }

        public void Dispose()
        {
            BSEvents.levelCleared -= BSEvents_levelLeft;
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

            int i = 0;
            retry:
            try
            {
                bool result = await AttemptScoreUpload(arg1, arg2);
                if (result)
                {
                    Console.WriteLine("BSNO-ScoreUploader: Score has been uploaded!");
                }
                else if (!result && i < 3)
                {
                    Console.WriteLine($"BSNO-ScoreUploader: Retry Nr. {i}");
                    i++;
                    goto retry;
                }
                else
                {
                    Console.WriteLine("BSNO-ScoreUploader: Score failed to upload.");
                }
            }
            catch
            {
                if(i >= 3)
                {
                    Console.WriteLine("BSNO-ScoreUploader: Score failed to upload.");
                }
                else
                {
                    Console.WriteLine($"BSNO-ScoreUploader: Retry Nr. {i}");
                    i++;
                    goto retry;
                }
            }
        }

        async Task<bool> AttemptScoreUpload(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            try
            {
                if (arg2.gameplayModifiers.ghostNotes || arg2.gameplayModifiers.noArrows || arg2.gameplayModifiers.noBombs || arg2.gameplayModifiers.zenMode || arg2.gameplayModifiers.disappearingArrows || arg2.gameplayModifiers.songSpeed != GameplayModifiers.SongSpeed.Normal)
                {
                    // If modifiers is turned on then dont upload
                    return true;
                }

                UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
                string levelHash = arg1.difficultyBeatmap.level.levelID.Substring(13);
                string songName = arg1.difficultyBeatmap.level.songName;
                string songDiff = arg1.difficultyBeatmap.difficultyRank.ToString();
                
                var httpWebReq = (HttpWebRequest)WebRequest.Create($"{Config.webserverUrl}/api/v2/json");
                httpWebReq.ContentType = "application/json";
                httpWebReq.Method = "POST";
                httpWebReq.Timeout = 400;

                using (var streamWriter = new StreamWriter(httpWebReq.GetRequestStream()))
                {
                    LevelResults levelResults = new LevelResults(userInfo.platformUserId, userInfo.userName, levelHash, songName, DateTime.Now, arg2.modifiedScore, (int)Math.Round(arg2.averageCutScoreForNotesWithFullScoreScoringType), arg2.maxCombo, arg2.missedCount, songDiff, Config.modVersion);
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
        public string modVersion;

        public LevelResults(string userId, string username, string levelHash, string songName, DateTime currentDate, int modifiedScore, int averageCutScore, int maxCombo, int missedCount, string songDiff, string modVersion)
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
            this.modVersion = modVersion;
        }
    }

    class NjsFixerConfig
    {
        public bool dontForceNJS;
    }
}
