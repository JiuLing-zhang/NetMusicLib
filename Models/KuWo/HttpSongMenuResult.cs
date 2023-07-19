namespace NetMusicLib.Models.KuWo;
internal class HttpSongMenuResult
{
    public int code { get; set; }
    public HttpSongMenuData? data { get; set; }
}

internal class HttpSongMenuData
{
    public List<HttpSongMenuDataList>? musicList { get; set; }
}

internal class HttpSongMenuDataList
{
    public string artist { get; set; } = null!;
    public string pic { get; set; } = null!;
    public int rid { get; set; }
    public int duration { get; set; }
    public string album { get; set; } = null!;
    public string name { get; set; } = null!;
    public HttpSongMenuDataListPayInfo payInfo { get; set; } = null!;
}

internal class HttpSongMenuDataListPayInfo
{
    public string listen_fragment { get; set; } = null!;
}

