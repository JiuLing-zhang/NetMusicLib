namespace NetMusicLib.Models.NetEase;
internal class MusicLyricHttpResult
{
    public MusicLyricHttpResultLrc? lrc { get; set; }
    public int code { get; set; }
}

internal class MusicLyricHttpResultLrc
{
    public string lyric { get; set; } = null!;
}