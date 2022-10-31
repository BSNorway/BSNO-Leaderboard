using IPA;
using BSNO_Score_Uploader.Installers;
using SiraUtil.Zenject;

namespace BSNO_Score_Uploader
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        [Init]
        public void Init(Zenjector zenjector)
        {
            zenjector.Install<MenuInstaller>(Location.Menu);
        }
    }
}
