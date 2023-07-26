using NetMusicLib.Enums;
using NetMusicLib.Models;

namespace NetMusicLib.MusicProvider;
internal interface IMusicProvider
{
    MusicFormatTypeEnum MusicFormatType { get; set; }
    Task InitializeAsync();

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
    Task<List<Music>> GetTagMusicsAsync(string tagId);

    /// <summary>
    /// 获取排行榜歌单
    /// </summary>
    /// <returns></returns>
    Task<List<SongMenu>> GetSongMenusFromTop();

    /// <summary>
    /// 获取排行榜歌单详情
    /// </summary>
    /// <returns></returns>
    Task<List<Music>> GetTopMusicsAsync(string topId);

    Task<List<string>> GetHotWordAsync();
    Task<List<string>> GetSearchSuggestAsync(string keyword);
    Task<List<Music>> SearchAsync(string keyword);
    Task<string> GetPlayUrlAsync(string id, string extendDataJson = "");
    Task<string> GetImageUrlAsync(string id, string extendDataJson = "");
    Task<string> GetLyricAsync(string id, string extendDataJson = "");
    Task<string> GetShareUrlAsync(string id, string extendDataJson = "");
}