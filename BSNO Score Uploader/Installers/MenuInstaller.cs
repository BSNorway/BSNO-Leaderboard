using BSNO_Score_Uploader.UI;
using Zenject;
using IPA.Logging;
using BSNO_Score_Uploader.UI.Views;
using BSNO_Score_Uploader.Services;

namespace BSNO_Score_Uploader.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<UIFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<SongLeaderboardController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<Top25Controller>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<Top26Controller>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesTo<MenuButtonUI>().AsSingle();
            Container.BindInterfacesTo<EventService>().AsSingle();
        }
    }
}
