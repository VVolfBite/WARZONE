namespace Warzone.Meta
{
    public sealed class AudioSettingsData
    {
        public AudioSettingsData(float masterVolume, float musicVolume, float sfxVolume)
        {
            MasterVolume = masterVolume;
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
        }

        public float MasterVolume { get; }
        public float MusicVolume { get; }
        public float SfxVolume { get; }
    }
}
