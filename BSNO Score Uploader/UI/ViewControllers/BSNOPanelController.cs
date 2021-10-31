using System;
using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BSNO_Score_Uploader.Interfaces;

namespace BSNO_Score_Uploader.UI.ViewControllers
{
    [HotReload(RelativePathToLayout = @"..\Views\LeaderboardView.bsml")]
    [ViewDefinition("BSNO_Score_Uploader.UI.Views.LeaderboardView.bsml")]
    internal class BSNOPanelController : BSMLAutomaticViewController
    {

    }
}
