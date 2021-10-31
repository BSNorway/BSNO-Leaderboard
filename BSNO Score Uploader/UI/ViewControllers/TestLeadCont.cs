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
using UnityEngine;

namespace BSNO_Score_Uploader.UI
{
    [ViewDefinition("BSNO_Score_Uploader.UI.Views.testLead.bsml")]
    public class TestLeadCont : BSMLAutomaticViewController
    {
        [UIAction("selectTxtClick")]
        private void SelectTxtClick() => UpdateLeaderboards();

        [UIComponent("leaderboard")]
        internal LeaderboardTableView table;

        public void UpdateLeaderboards()
        {
            List<LeaderboardTableView.ScoreData> scores = new List<LeaderboardTableView.ScoreData>();
            scores.Add(new LeaderboardTableView.ScoreData(0, "You haven't set a score on this leaderboard - <size=75%>(<color=#FFD42A>0%</color>)</size>", 0, false));
            table.SetScores(scores, 1);
        }
    }
}
