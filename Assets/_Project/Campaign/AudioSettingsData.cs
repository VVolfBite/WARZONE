namespace Warzone.Campaign
{
    public sealed class AudioSettingsData
    {
        public AudioSettingsData(float masterVolume, float musicVolume, float sfxVolume)
        {
            MasterVolume = masterVolume;
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
        }

        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
    }
}




