using System;

public class Info
{
    public string FileName { get; set; }
    public string FilePathArg { get; set; }
    public string FilePath { get; set; }
    public string FileDirArg { get; set; }
    public string FileDir { get; set; }
    public State State { get; set; }
    public string StateString { get; set; }
    public long PositionMillisec { get; set; }
    public TimeSpan Position { get; set; }
    public long DurationMillisec { get; set; }
    public TimeSpan Duration { get; set; }
    public int VolumeLevel { get; set; }
    public bool Muted { get; set; }
    public double PlaybackRate { get; set; }
    public string SizeString { get; set; }
    public int ReloadTime { get; set; }
    public string Version { get; set; }

    public static State IntToState(int i)
    {
        switch (i)
        {
            case 0:
                return State.Stoped;
            case 1:
                return State.Paused;
            case 2:
                return State.Playing;
            default:
                return State.None;
        }
    }
}