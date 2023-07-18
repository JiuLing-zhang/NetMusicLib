﻿namespace NetMusicLib.Models.NetEase
{
    public class TagMusicHttpResult
    {
        public int code { get; set; }
        public TagMusicPlaylistHttpResult? playlist { get; set; }
    }

    public class TagMusicPlaylistHttpResult
    {
        public List<TagMusicTrackIdsHttpResult>? trackIds { get; set; }
    }

    public class TagMusicTrackIdsHttpResult
    {
        public long id { get; set; }
    }

    public class TagMusicSongsHttpResult
    {
        public List<TagMusicSongs> songs { get; set; } = null!;
        public List<TagMusicPrivileges> privileges { get; set; } = null!;
    }

    public class TagMusicSongs
    {
        public string name { get; set; } = null!;
        public long id { get; set; }
        public List<TagMusicAr>? ar { get; set; }
        public TagMusicAl? al { get; set; }
        public long dt { get; set; }
    }

    public class TagMusicAl
    {
        public string name { get; set; } = null!;
        public string picUrl { get; set; } = null!;

    }
    public class TagMusicAr
    {
        public string name { get; set; } = null!;
    }


    public class TagMusicPrivileges
    {
        public long id { get; set; }
        public int fee { get; set; }
    }
}