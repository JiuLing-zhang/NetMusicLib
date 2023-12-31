﻿namespace NetMusicLib.Models.KuWo;
internal class HttpTagSongMenuResult
{
    public int code { get; set; }
    public HttpTagSongMenuResultData? data { get; set; }
}

internal class HttpTagSongMenuResultData
{
    public List<HttpTagSongMenuResultDataDatum>? data { get; set; }
}

internal class HttpTagSongMenuResultDataDatum
{
    public string img { get; set; } = null!;
    public string name { get; set; } = null!;
    public string id { get; set; } = null!;
}
