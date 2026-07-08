namespace Warzone.Campaign
{
    public sealed class SettingsData
    {
        public SettingsData(float masterVolume, float musicVolume, float sfxVolume, int graphicsQuality)
        {
            MasterVolume = masterVolume;
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
            GraphicsQuality = graphicsQuality;
        }

        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
        public int GraphicsQuality { get; private set; }
    }
}




