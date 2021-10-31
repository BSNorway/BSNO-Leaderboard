using Newtonsoft.Json;
using System.Collections.Generic;

namespace BSNO_Score_Uploader.Entries
{
    class BSNOLevelInfo
    {
        [JsonProperty("star_rating")]
        public Dictionary<string, float> pools;
    }
}
