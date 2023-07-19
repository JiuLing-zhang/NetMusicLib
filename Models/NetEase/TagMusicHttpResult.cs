namespace NetMusicLib.Models.NetEase;

internal class TagMusicHttpResult
{
    public int code { get; set; }
    public TagMusicPlaylistHttpResult? playlist { get; set; }
}

internal class TagMusicPlaylistHttpResult
{
    public List<TagMusicTrackIdsHttpResult>? trackIds { get; set; }
}

internal class TagMusicTrackIdsHttpResult
{
    public long id { get; set; }
}

internal class TagMusicSongsHttpResult
{
    public List<TagMusicSongs> songs { get; set; } = null!;
    public List<TagMusicPrivileges> privileges { get; set; } = null!;
}

internal class TagMusicSongs
{
    public string name { get; set; } = null!;
    public long id { get; set; }
    public List<TagMusicAr>? ar { get; set; }
    public TagMusicAl? al { get; set; }
    public long dt { get; set; }
}

internal class TagMusicAl
{
    public string name { get; set; } = null!;
    public string picUrl { get; set; } = null!;

}
internal class TagMusicAr
{
    public string name { get; set; } = null!;
}


internal class TagMusicPrivileges
{
    public long id { get; set; }
    public int fee { get; set; }
}
