using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine.SceneManagement;
using UnityEngine;
using BSNO_Score_Uploader.Services;
using BSNO_Score_Uploader.Installers;
using IPALogger = IPA.Logging.Logger;
using System.Linq.Expressions;

namespace BSNO_Score_Uploader
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private Startup _startup;
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        private EventService _eventService;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("BSNO Score Uploader initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("BSNO_Score_UploaderController").AddComponent<BSNO_Score_UploaderController>();

            _eventService = new EventService();
            _eventService.Initialize();
            _startup = new Startup();
            _startup.AddButton();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
            _eventService = new EventService();
            _eventService.Dispose();
        }
    }
}
