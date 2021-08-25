using HMUI;
using BeatSaberMarkupLanguage;
using BSNO_Score_Uploader.UI.Views;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BSNO_Score_Uploader.UI
{
    class ModMenuController : FlowCoordinator
    {
        private SongLeaderboardController _songLeaderboardController;
        private Top25Controller _top25Controller;
        private Top26Controller _top26Controller;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("BSNO Leaderboard");
                showBackButton = true;
            }

            _songLeaderboardController = BeatSaberUI.CreateViewController<SongLeaderboardController>();
            _top25Controller = BeatSaberUI.CreateViewController<Top25Controller>();
            _top26Controller = BeatSaberUI.CreateViewController<Top26Controller>();
            this.ProvideInitialViewControllers(_songLeaderboardController, _top26Controller, _top25Controller);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
