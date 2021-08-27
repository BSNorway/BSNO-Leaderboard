using IPA;
using UnityEngine;
using BSNO_Score_Uploader.Services;
using BSNO_Score_Uploader.Installers;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;

namespace BSNO_Score_Uploader
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; private set; }
        private EventService _eventService;

        [Init]
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            zenjector.OnMenu<MenuInstaller>().WithParameters(logger);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            new GameObject("BSNO_Score_UploaderController").AddComponent<BSNO_Score_UploaderController>();

            _eventService = new EventService();
            _eventService.Initialize();
        }
    }
}
