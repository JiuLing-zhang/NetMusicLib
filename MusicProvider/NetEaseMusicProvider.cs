﻿using System.Net;
using JiuLing.CommonLibs.ExtensionMethods;
using JiuLing.CommonLibs.Net;
using JiuLing.CommonLibs.Security;
using Microsoft.Extensions.Logging;
using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.Models.NetEase;
using NetMusicLib.Utils;

namespace NetMusicLib.MusicProvider;
public class NetEaseMusicProvider : IMusicProvider
{
    private readonly HttpClient _httpClient;
    public PlatformEnum Platform => PlatformEnum.NetEase;
    private readonly ILogger<NetEaseMusicProvider> _logger;
    public NetEaseMusicProvider(ILogger<NetEaseMusicProvider> logger)
    {
        _logger = logger;

        var handler = new HttpClientHandler();
        handler.AutomaticDecompression = DecompressionMethods.All;
        _httpClient = new HttpClient(handler);
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }
    public async Task<List<string>> GetSearchSuggestAsync(string keyword)
    {
        try
        {
            var searchSuggestList = new List<string>();
            string url = $"{UrlBase.NetEase.Suggest}";

            var postData = NetEaseUtils.GetPostDataForSuggest(keyword);
            var form = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            ResultBase<SearchSuggestHttpResult>? result;
            try
            {
                result = json.ToObject<ResultBase<SearchSuggestHttpResult>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析网易搜索建议失败。");
                return searchSuggestList;
            }

            if (result == null)
            {
                _logger.LogInformation("解析网易搜索建议失败，服务器返回空。");
                return searchSuggestList;
            }
            if (result.code != 200 || result.result.songs == null)
            {
                _logger.LogInformation("解析网易搜索建议失败，服务器返回参数异常。");
                return searchSuggestList;
            }
            searchSuggestList = result.result.songs.Select(x => x.name).Distinct().ToList();
            return searchSuggestList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易搜索建议获取失败。");
            return new List<string>();
        }
    }

    public async Task<List<Music>> SearchAsync(string keyword)
    {
        var musics = new List<Music>();

        try
        {
            string url = $"{UrlBase.NetEase.Search}";

            var postData = NetEaseUtils.GetPostDataForSearch(keyword);
            var form = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);

            ResultBase<MusicSearchHttpResult>? result;
            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = json.ToObject<ResultBase<MusicSearchHttpResult>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析网易搜索结果失败。");
                return musics;
            }

            if (result == null || result.code != 200)
            {
                return musics;
            }

            foreach (var song in result.result.songs)
            {
                try
                {
                    string artistName = "";
                    if (song.ar.Count > 0)
                    {
                        artistName = string.Join("、", song.ar.Select(x => x.name).ToList());
                    }
                    var music = new Music()
                    {
                        Id = MD5Utils.GetStringValueToLower($"{Platform}-{song.id}"),
                        Platform = Platform,
                        IdOnPlatform = song.id.ToString(),
                        Name = song.name,
                        Artist = artistName,
                        Album = song.al.name,
                        ImageUrl = $"{song.al.picUrl}?param=250y250",
                        Duration = TimeSpan.FromMilliseconds(song.dt),
                        Fee = GetFeeFlag(song.privilege)
                    };
                    musics.Add(music);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "构建网易搜索结果失败。");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易搜索失败。");
        }
        return musics;
    }

    private FeeEnum GetFeeFlag(MusicSearchHttpResultSongPrivilege privilege)
    {

        if (privilege == null)
        {
            return FeeEnum.Free;
        }
        if (privilege.fee == 1)
        {
            return FeeEnum.Vip;
        }

        //猜测fee=0 flag=256 应该是地区无权限，样本太少没法分析
        //TODO 验证后在增加相关逻辑
        return FeeEnum.Free;
    }

    public Task<List<SongMenu>> GetSongMenusFromTop()
    {
        return Task.FromResult(NetEaseUtils.GetSongMenusFromTop());
    }

    public Task<List<string>> GetHotWordAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetShareUrlAsync(string id, string extendDataJson = "")
    {
        return Task.FromResult($"{UrlBase.NetEase.GetMusicPlayPage}?id={id}");
    }

    public async Task<string> GetLyricAsync(string id, string extendDataJson = "")
    {
        try
        {
            //获取歌词
            var url = $"{UrlBase.NetEase.Lyric}";
            var postData = NetEaseUtils.GetPostDataForLyric(id);
            var form = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            request.Headers.Add("Accept", "application/json, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var lyricResult = json.ToObject<MusicLyricHttpResult>();
            if (lyricResult == null)
            {
                return "";
            }
            if (lyricResult.code != 200)
            {
                return "";
            }

            if (lyricResult.lrc == null)
            {
                return "";
            }
            return lyricResult.lrc.lyric;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易歌词获取失败。");
            return "";
        }
    }

    public async Task<PlatformMusicTag?> GetMusicTagsAsync()
    {
        try
        {
            //热门标签
            string url = $"{UrlBase.NetEase.GetHotTagsUrl}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var html = System.Text.Encoding.UTF8.GetString(buffer);

            var hotTags = NetEaseUtils.GetHotTags(html);
            if (!hotTags.Any())
            {
                throw new Exception("网易热门标签获取失败");
            }
            //全部标签
            url = $"{UrlBase.NetEase.GetAllTypesUrl}";
            request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            html = System.Text.Encoding.UTF8.GetString(buffer);
            var allTypes = NetEaseUtils.GetAllTypes(html);
            if (!allTypes.Any())
            {
                throw new Exception("网易歌曲标签获取失败");
            }
            return new PlatformMusicTag(hotTags, allTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易热搜获取失败。");
            return default;
        }
    }

    public async Task<List<SongMenu>> GetSongMenusFromTagAsync(string id, int page)
    {
        try
        {
            var limit = 35;
            var offset = (page - 1) * limit;
            string url = $"{UrlBase.NetEase.GetSongMenusFromTagUrl}?order=hot&cat={id}&limit={limit}&offset={offset}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var html = System.Text.Encoding.UTF8.GetString(buffer);

            return NetEaseUtils.GetSongMenusFromTag(html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易标签获取歌单失败。");
            return new List<SongMenu>();
        }
    }

    public async Task<List<Music>> GetTagMusicsAsync(string tagId)
    {
        try
        {
            var musics = new List<Music>();
            string url = $"{UrlBase.NetEase.GetTagMusicsUrl}";

            var postData = NetEaseUtils.GetPostDataForTagMusics(tagId);
            var form = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            TagMusicHttpResult? result;
            try
            {
                result = json.ToObject<TagMusicHttpResult>();
            }
            catch (Exception)
            {
                return musics;
            }

            if (result == null || result.code != 200 || result.playlist == null || result.playlist.trackIds == null || result.playlist.trackIds.Count == 0)
            {
                return musics;
            }

            List<long> tracksId = result.playlist.trackIds.Select(x => x.id).ToList();
            url = $"{UrlBase.NetEase.GetSongDetailUrl}";
            postData = NetEaseUtils.GetSongDetailPostData(tracksId);
            form = new FormUrlEncodedContent(postData);
            request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            TagMusicSongsHttpResult? songResult;
            try
            {
                songResult = json.ToObject<TagMusicSongsHttpResult>();
                if (songResult == null || songResult.songs == null)
                {
                    return musics;
                }
            }
            catch (Exception)
            {
                return musics;
            }

            foreach (var track in songResult.songs)
            {
                string artist = "";
                if (track.ar != null && track.ar.Count >= 1)
                {
                    artist = track.ar[0].name ?? "";
                }

                string album = "";
                string imageUrl = "";
                if (track.al != null)
                {
                    album = track.al.name ?? "";
                    imageUrl = track.al.picUrl ?? "";
                }

                FeeEnum fee = FeeEnum.Free;
                if (songResult.privileges != null)
                {
                    var privilege = songResult.privileges.FirstOrDefault(x => x.id == track.id);
                    if (privilege != null)
                    {
                        if (privilege.fee == 1)
                        {
                            fee = FeeEnum.Vip;
                        }
                    }
                }

                var music = new Music()
                {
                    Id = MD5Utils.GetStringValueToLower($"{Platform}-{track.id}"),
                    Platform = Platform,
                    IdOnPlatform = track.id.ToString(),
                    Name = track.name ?? "",
                    Artist = artist,
                    Album = album,
                    ImageUrl = $"{imageUrl}?param=250y250",
                    Duration = TimeSpan.FromMilliseconds(track.dt),
                    Fee = fee,
                };

                musics.Add(music);
            }
            return musics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易标签歌单加载失败。");
            return new List<Music>();
        }
    }
    public async Task<List<Music>> GetTopMusicsAsync(string topId)
    {
        try
        {
            var musics = new List<Music>();
            string url = $"{UrlBase.NetEase.GetTagMusicsUrl}";

            var postData = NetEaseUtils.GetPostDataForTagMusics(topId);
            var form = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            TagMusicHttpResult? result;
            try
            {
                result = json.ToObject<TagMusicHttpResult>();
            }
            catch (Exception)
            {
                return musics;
            }

            if (result == null || result.code != 200 || result.playlist == null || result.playlist.trackIds == null || result.playlist.trackIds.Count == 0)
            {
                return musics;
            }

            List<long> tracksId = result.playlist.trackIds.Select(x => x.id).ToList();
            url = $"{UrlBase.NetEase.GetSongDetailUrl}";
            postData = NetEaseUtils.GetSongDetailPostData(tracksId);
            form = new FormUrlEncodedContent(postData);
            request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            TagMusicSongsHttpResult? songResult;
            try
            {
                songResult = json.ToObject<TagMusicSongsHttpResult>();
                if (songResult == null || songResult.songs == null)
                {
                    return musics;
                }
            }
            catch (Exception)
            {
                return musics;
            }

            foreach (var track in songResult.songs)
            {
                string artist = "";
                if (track.ar != null && track.ar.Count >= 1)
                {
                    artist = track.ar[0].name ?? "";
                }

                string album = "";
                string imageUrl = "";
                if (track.al != null)
                {
                    album = track.al.name ?? "";
                    imageUrl = track.al.picUrl ?? "";
                }

                FeeEnum fee = FeeEnum.Free;
                if (songResult.privileges != null)
                {
                    var privilege = songResult.privileges.FirstOrDefault(x => x.id == track.id);
                    if (privilege != null)
                    {
                        if (privilege.fee == 1)
                        {
                            fee = FeeEnum.Vip;
                        }
                    }
                }

                var music = new Music()
                {
                    Id = MD5Utils.GetStringValueToLower($"{Platform}-{track.id}"),
                    Platform = Platform,
                    IdOnPlatform = track.id.ToString(),
                    Name = track.name ?? "",
                    Artist = artist,
                    Album = album,
                    ImageUrl = $"{imageUrl}?param=250y250",
                    Duration = TimeSpan.FromMilliseconds(track.dt),
                    Fee = fee,
                };

                musics.Add(music);
            }

            return musics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易排行榜歌单加载失败。");
            return new List<Music>();
        }
    }

    public async Task<string> GetPlayUrlAsync(string id, string extendDataJson = "")
    {
        try
        {
            string url = $"{UrlBase.NetEase.GetMusic}";
            var postData = NetEaseUtils.GetPostDataForMusicUrl(id);
            var form = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = form
            };
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var httpResult = json.ToObject<ResultBase<MusicUrlHttpResult>>();
            if (httpResult == null)
            {
                return "";
            }
            if (httpResult.code != 200)
            {
                return "";
            }

            if (httpResult.data.Count == 0)
            {
                return "";
            }

            return httpResult.data[0].url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网易获取播放地址失败。");
            return "";
        }
    }

    public Task<string> GetImageUrlAsync(string id, string extendDataJson = "")
    {
        throw new NotImplementedException();
    }
}
