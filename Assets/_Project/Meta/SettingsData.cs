namespace Warzone.Meta
{
    public sealed class SettingsData
    {
        public SettingsData(float masterVolume, float sfxVolume, float musicVolume, int graphicsQuality)
        {
            MasterVolume = masterVolume;
            SfxVolume = sfxVolume;
            MusicVolume = musicVolume;
            GraphicsQuality = graphicsQuality;
        }

        public float MasterVolume { get; }
        public float SfxVolume { get; }
        public float MusicVolume { get; }
        public int GraphicsQuality { get; }
    }
}
