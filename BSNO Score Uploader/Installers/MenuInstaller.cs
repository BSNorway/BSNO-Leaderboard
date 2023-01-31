using BSNO_Score_Uploader.UI;
using Zenject;
using BSNO_Score_Uploader.Services;

namespace BSNO_Score_Uploader.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<UIFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<SongLeaderboardController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesTo<MenuButtonUI>().AsSingle();
            Container.BindInterfacesTo<EventService>().AsSingle();
        }
    }
}
