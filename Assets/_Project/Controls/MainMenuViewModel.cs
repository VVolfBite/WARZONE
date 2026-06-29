namespace Warzone.Controls
{
    public sealed class MainMenuViewModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string PrimaryActionLabel { get; set; }
        public string SecondaryActionLabel { get; set; }
        public string TertiaryActionLabel { get; set; }
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public int GraphicsQuality { get; set; }
    }
}
