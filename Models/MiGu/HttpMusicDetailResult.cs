namespace NetMusicLib.Models.MiGu;
internal class HttpMusicDetailResult
{
    public List<HttpMusicDetailResource>? resource { get; set; }
}

internal class HttpMusicDetailResource
{
    public List<HttpMusicDetailResourceNewRateFormats> newRateFormats { get; set; } = null!;
    public string lrcUrl { get; set; } = null!;
}

internal class HttpMusicDetailResourceNewRateFormats
{
    public string formatType { get; set; } = null!;
    public string? url { get; set; } = null!;
    public string? iosUrl { get; set; } = null!;
}
