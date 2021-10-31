using BSNO_Score_Uploader.Entries;

namespace BSNO_Score_Uploader.Interfaces
{
    internal interface IDifficultyBeatmapUpdater
    {
        public void DifficultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap, BSNOLevelInfo levelInfoEntry);
    }
}
