using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage;
using BSNO_Score_Uploader.UI;

namespace BSNO_Score_Uploader.Installers
{
    public class Startup
    {
        private MenuButton _menuButton;

        public void AddButton()
        {
            if (_menuButton != null) return;
                _menuButton = new MenuButton("BSNO", "BSNO weekly leaderboard", SummonFlowCoordinator);

            MenuButtons.instance.RegisterButton(_menuButton);
        }

        private void SummonFlowCoordinator()
        {
            var flowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModMenuController>();
            if (flowCoordinator != null) return;
                BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator);
        }
    }
}
