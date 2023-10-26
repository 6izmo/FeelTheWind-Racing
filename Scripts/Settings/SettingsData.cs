public class SettingsData 
{
    public bool FirstStartGame { get; set; }
    public bool FPS { get; set; }
    public bool FullScreen { get; set; }
    public bool MotionBlur { get; set; }
    public ResItem ResItem { get; set; }
    public AntiAliasingMode AntiAliasing { get; set; }
    public float EffectsVolume { get; set; }
    public float MusicVolume { get; set; }

    public static Newtonsoft.Json.JsonSerializerSettings SerializeSettings()
    {
        return new Newtonsoft.Json.JsonSerializerSettings
        {
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        };
    }
}
