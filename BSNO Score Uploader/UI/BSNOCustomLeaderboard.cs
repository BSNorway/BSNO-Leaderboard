using System;
using BSNO_Score_Uploader.Entries;
using BSNO_Score_Uploader.Interfaces;
using BSNO_Score_Uploader.UI.ViewControllers;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;

namespace BSNO_Score_Uploader.UI
{
    internal class BSNOCustomLeaderboard : CustomLeaderboard, IDisposable, IDifficultyBeatmapUpdater
    {
        private readonly CustomLeaderboardManager customLeaderboardManager;

        private readonly ViewController bsnoPanelController;
        protected override ViewController panelViewController => bsnoPanelController;

        private readonly BSNOLeaderboardViewController bsnoLeaderboardViewController;
        protected override ViewController leaderboardViewController => bsnoLeaderboardViewController;

        internal BSNOCustomLeaderboard(CustomLeaderboardManager customLeaderboardManager, BSNOPanelController bsnoPanelController, BSNOLeaderboardViewController mainLeaderboardViewController)
        {
            this.customLeaderboardManager = customLeaderboardManager;
            this.bsnoPanelController = bsnoPanelController;
            this.bsnoLeaderboardViewController = mainLeaderboardViewController;
        }

        public void Dispose()
        {
            customLeaderboardManager.Unregister(this);
        }

        public void DifficultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap, BSNOLevelInfo levelInfoEntry)
        {
            if (levelInfoEntry != null)
            {
                customLeaderboardManager.Register(this);
            }
            else if (levelInfoEntry == null)
            {
                customLeaderboardManager.Unregister(this);
            }
        }
    }
}
