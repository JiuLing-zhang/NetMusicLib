namespace NetMusicLib.Models.KuWo;

internal class HttpMusicTagResult
{
    public int code { get; set; }
    public List<HttpMusicTagDatum>? data { get; set; }
}

internal class HttpMusicTagDatum
{
    public List<HttpMusicTagDatumData> data { get; set; } = null!;
    public string name { get; set; } = null!;
}

internal class HttpMusicTagDatumData
{
    public string name { get; set; } = null!;
    public string id { get; set; } = null!;
}
