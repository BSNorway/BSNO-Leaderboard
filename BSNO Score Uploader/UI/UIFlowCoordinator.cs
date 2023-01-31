using HMUI;
using BeatSaberMarkupLanguage;
using Zenject;
using System;

namespace BSNO_Score_Uploader.UI
{
    public class UIFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlowCoordinator;

        private SongLeaderboardController _songLeaderboardController;

        [Inject]
        public void Construct(MainFlowCoordinator mainFlowCoordinator, SongLeaderboardController songLeaderboardController)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _songLeaderboardController = songLeaderboardController;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    SetTitle("BSNO Leaderboard");
                    showBackButton = true;
                    this.ProvideInitialViewControllers(_songLeaderboardController);
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
