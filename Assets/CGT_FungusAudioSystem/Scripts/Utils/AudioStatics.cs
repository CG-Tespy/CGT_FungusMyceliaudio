namespace CGT.FungusExt.Audio
{
    /// <summary>
    /// To help audio-playing not sound funky
    /// </summary>
    public static class AudioStatics
    {
        public static float MinVolume { get; } = 0f;
        public static float MaxVolume { get; } = 1f;
        public static float MinPitch { get; } = -3f;
        public static float MaxPitch { get; } = 3f;
        public static float DefaultPitch { get; } = 1f;
    }
}