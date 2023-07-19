namespace NetMusicLib.Models.MiGu;
internal class HttpMusicTopResult
{
    public HttpMusicTopSongResult? songs { get; set; }
}

internal class HttpMusicTopSongResult
{
    public List<HttpMusicTopSongItemResult>? items { get; set; }
}

internal class HttpMusicTopSongItemResult
{
    public string copyrightId { get; set; } = null!;
    public string name { get; set; } = null!;
    public string mediumPic { get; set; } = null!;
    public string ImageUrl => $"https:{mediumPic}";
    public string duration { get; set; } = null!;
    public List<HttpMusicTopSongSingerResult>? singers { get; set; }
    public HttpMusicTopSongAlbumResult? album { get; set; }
}

internal class HttpMusicTopSongAlbumResult
{
    public string albumName { get; set; } = null!;
}
internal class HttpMusicTopSongSingerResult
{
    public string name { get; set; } = null!;
}