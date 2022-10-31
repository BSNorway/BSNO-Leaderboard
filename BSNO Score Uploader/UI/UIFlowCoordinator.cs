using HMUI;
using BeatSaberMarkupLanguage;
using BSNO_Score_Uploader.UI.Views;
using Zenject;
using System;

namespace BSNO_Score_Uploader.UI
{
    public class UIFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlowCoordinator;

        private SongLeaderboardController _songLeaderboardController;
        private Top25Controller _top25Controller;
        private Top26Controller _top26Controller;

        [Inject]
        public void Construct(MainFlowCoordinator mainFlowCoordinator, SongLeaderboardController songLeaderboardController, Top25Controller top25Controller, Top26Controller top26Controller)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _songLeaderboardController = songLeaderboardController;
            _top25Controller = top25Controller;
            _top26Controller = top26Controller;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    SetTitle("BSNO Leaderboard");
                    showBackButton = true;
                    this.ProvideInitialViewControllers(_songLeaderboardController, _top26Controller, _top25Controller);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            _mainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
