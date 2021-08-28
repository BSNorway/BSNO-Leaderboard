using IPA;
using BSNO_Score_Uploader.Installers;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;

namespace BSNO_Score_Uploader
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; private set; }

        [Init]
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            zenjector.OnMenu<MenuInstaller>().WithParameters(logger);
        }
    }
}
