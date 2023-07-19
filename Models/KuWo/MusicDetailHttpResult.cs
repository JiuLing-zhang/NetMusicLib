namespace NetMusicLib.Models.KuWo;
internal class MusicDetailHttpResult
{
    public List<MusicDetailLrclist>? lrclist { get; set; }
}
internal class MusicDetailLrclist
{
    public string lineLyric { get; set; } = null!;
    public string time { get; set; } = null!;
}