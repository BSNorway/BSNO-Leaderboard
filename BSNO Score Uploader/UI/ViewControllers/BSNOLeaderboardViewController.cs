using System;
using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;


namespace BSNO_Score_Uploader.UI.ViewControllers
{
    [HotReload(RelativePathToLayout = @"..\Views\BSNOLeaderboardView.bsml")]
    [ViewDefinition("BSNO_Score_Uploader.UI.Views.BSNOLeaderboardView.bsml")]
    public class BSNOLeaderboardViewController : BSMLAutomaticViewController
    {
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            UpdateLeaderboard();
        }

        [UIComponent("leaderboard")]
        internal LeaderboardTableView table;

        public void UpdateLeaderboard()
        {
            List<LeaderboardTableView.ScoreData> scores = new List<LeaderboardTableView.ScoreData>();
            scores.Add(new LeaderboardTableView.ScoreData(0, "You haven't set a score on this leaderboard - <size=75%>(<color=#FFD42A>0%</color>)</size>", 0, false));
            table.SetScores(scores, 1);
        }
    }
}
