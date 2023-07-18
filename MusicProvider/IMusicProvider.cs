﻿using NetMusicLib.Models;

namespace NetMusicLib.MusicProvider;
public interface IMusicProvider
{
    /// <summary>
    /// 获取音乐标签分类
    /// </summary>
    /// <returns></returns>
    Task<PlatformMusicTag?> GetMusicTagsAsync();

    /// <summary>
    /// 获取音乐标签对应的歌单
    /// </summary>
    /// <returns></returns>
    Task<List<SongMenu>> GetSongMenusFromTagAsync(string id, int page);

    /// <summary>
    /// 获取标签歌单详情
    /// </summary>
    /// <returns></returns>
    Task<List<MusicResultShow>> GetTagMusicsAsync(string tagId);

    /// <summary>
    /// 获取排行榜歌单
    /// </summary>
    /// <returns></returns>
    Task<List<SongMenu>> GetSongMenusFromTop();

    /// <summary>
    /// 获取排行榜歌单详情
    /// </summary>
    /// <returns></returns>
    Task<List<MusicResultShow>> GetTopMusicsAsync(string topId);

    Task<List<string>> GetHotWordAsync();
    Task<List<string>> GetSearchSuggestAsync(string keyword);
    Task<List<MusicResultShow>> SearchAsync(string keyword);
    Task<string> GetPlayUrlAsync(string id, string extendDataJson = "");
    Task<string> GetImageUrlAsync(string id, string extendDataJson = "");
    Task<string> GetLyricAsync(string id, string extendDataJson = "");
    Task<string> GetShareUrlAsync(string id, string extendDataJson = "");
}