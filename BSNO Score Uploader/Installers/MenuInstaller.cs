using BSNO_Score_Uploader.UI;
using Zenject;
using SiraUtil;
using IPA.Logging;
using BSNO_Score_Uploader.UI.Views;

namespace BSNO_Score_Uploader.Installers
{
    internal class MenuInstaller : Installer
    {
        private readonly Logger _logger;

        internal MenuInstaller(Logger logger)
        {
            _logger = logger;
        }

        public override void InstallBindings()
        {
            Container.BindLoggerAsSiraLogger(_logger);
            Container.Bind<UIFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<SongLeaderboardController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<Top25Controller>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<Top26Controller>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesTo<MenuButtonUI>().AsSingle();
        }
    }
}
