﻿using System.Net;
using System.Text;
using JiuLing.CommonLibs.ExtensionMethods;
using JiuLing.CommonLibs.Net;
using JiuLing.CommonLibs.Security;
using Microsoft.Extensions.Logging;
using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.Models.KuWo;
using NetMusicLib.Utils;

namespace NetMusicLib.MusicProvider;
public class KuWoMusicProvider : IMusicProvider
{
    private readonly ILogger<KuWoMusicProvider> _logger;
    private readonly HttpClient _httpClient;
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    public PlatformEnum Platform => PlatformEnum.KuWo;
    private readonly string _reqId = JiuLing.CommonLibs.GuidUtils.GetFormatD();
    public string _csrf
    {
        get
        {
            return _cookieContainer.GetCookies(new Uri("http://www.kuwo.cn"))["kw_token"]?.Value ?? "";
        }
    }

    public KuWoMusicProvider(ILogger<KuWoMusicProvider> logger)
    {
        _logger = logger;

        var handler = new HttpClientHandler();
        handler.CookieContainer = _cookieContainer;
        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        _httpClient = new HttpClient(handler);
        _httpClient.Timeout = TimeSpan.FromSeconds(10);

        var task = Task.Run(InitializeAsync);
        task.Wait();
    }

    private async Task InitializeAsync()
    {
        //Init cookie
        await _httpClient.GetStringAsync("http://www.kuwo.cn");
    }

    public Task<List<string>> GetSearchSuggestAsync(string keyword)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Music>> SearchAsync(string keyword)
    {
        var musics = new List<Music>();

        try
        {
            string url = $"{UrlBase.KuWo.Search}?key={keyword}&pn=1&rn=20&httpsStatus=1&reqId={_reqId}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            request.Headers.Add("Referer", "http://www.kuwo.cn/");
            request.Headers.Add("Host", "kuwo.cn");
            request.Headers.Add("csrf", _csrf);

            HttpResultBase<HttpMusicSearchResult>? httpResult;
            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                httpResult = json.ToObject<HttpResultBase<HttpMusicSearchResult>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析酷我音乐搜索结果失败。");
                return musics;
            }
            if (httpResult == null || httpResult.code != 200)
            {
                return musics;
            }

            foreach (var httpMusic in httpResult.data.list)
            {
                try
                {
                    var music = new Music()
                    {
                        Id = MD5Utils.GetStringValueToLower($"{Platform}-{httpMusic.rid}"),
                        Platform = Platform,
                        IdOnPlatform = httpMusic.rid.ToString(),
                        Name = httpMusic.name.Replace("&nbsp;", " "),
                        Artist = httpMusic.artist.Replace("&nbsp;", " "),
                        Album = httpMusic.album.Replace("&nbsp;", " "),
                        Fee = GetFeeFlag(httpMusic.payInfo.listen_fragment),
                        ImageUrl = httpMusic.pic,
                        Duration = TimeSpan.FromSeconds(httpMusic.duration)
                    };
                    musics.Add(music);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "构建酷狗搜索结果失败。");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "酷我搜索失败。");
        }

        return musics;
    }

    private FeeEnum GetFeeFlag(string listenFragment)
    {
        if (listenFragment == "1")
        {
            return FeeEnum.Demo;
        }

        return FeeEnum.Free;
    }

    public Task<List<SongMenu>> GetSongMenusFromTop()
    {
        return Task.FromResult(KuWoUtils.GetSongMenusFromTop());
    }

    public async Task<List<Music>> GetTopMusicsAsync(string topId)
    {
        var musics = new List<Music>();
        try
        {
            string url = $"{UrlBase.KuWo.GetTopMusicsUrl}?bangId={topId}&pn=1&rn=20&httpsStatus=1&reqId={_reqId}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            request.Headers.Add("Referer", "http://www.kuwo.cn/rankList");
            request.Headers.Add("Host", "www.kuwo.cn");
            request.Headers.Add("csrf", _csrf);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var bangMusics = KuWoUtils.GetSongMenuMusics(json);

            foreach (var bangMusic in bangMusics)
            {
                try
                {
                    musics.Add(new Music()
                    {
                        Id = MD5Utils.GetStringValueToLower($"{Platform}-{bangMusic.rid}"),
                        Platform = Platform,
                        IdOnPlatform = bangMusic.rid.ToString(),
                        Name = bangMusic.name.Replace("&nbsp;", " "),
                        Artist = bangMusic.artist.Replace("&nbsp;", " "),
                        Album = bangMusic.album.Replace("&nbsp;", " "),
                        Fee = GetFeeFlag(bangMusic.payInfo.listen_fragment),
                        Duration = TimeSpan.FromSeconds(bangMusic.duration),
                        ImageUrl = bangMusic.pic
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "酷我榜单歌曲添加失败。");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "酷我榜单歌曲获取失败。");
        }
        return musics;
    }

    public async Task<List<string>> GetHotWordAsync()
    {
        var reslt = new List<string>();
        try
        {
            string url = $"{UrlBase.KuWo.HotWord}?key=&httpsStatus=1&reqId={_reqId}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            request.Headers.Add("Referer", "http://www.kuwo.cn/");
            request.Headers.Add("Host", "www.kuwo.cn");
            request.Headers.Add("csrf", _csrf);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            reslt = KuWoUtils.GetHotWord(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "酷我热搜词获取失败。");
        }
        return reslt;
    }

    public Task<string> GetShareUrlAsync(string id, string extendDataJson = "")
    {
        return Task.FromResult($"{UrlBase.KuWo.GetMusicPlayPage}/{id}");
    }

    public async Task<string> GetLyricAsync(string id, string extendDataJson = "")
    {
        //获取歌曲详情
        var url = $"{UrlBase.KuWo.GetMusicDetail}?musicId={id}&httpsStatus=1&reqId={_reqId}";
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        HttpResultBase<MusicDetailHttpResult>? httpResult;
        try
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            httpResult = json.ToObject<HttpResultBase<MusicDetailHttpResult>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "酷我歌曲详情获取失败。");
            return "";
        }
        if (httpResult == null)
        {
            _logger.LogError(new Exception($"服务器返回异常，ID:{id}"), "酷我歌曲详情获取失败。");
            return "";
        }
        if (httpResult.status != 200)
        {
            _logger.LogError(new Exception($"服务器返回状态异常：{httpResult.message ?? ""}，ID:{id}"), "酷我歌曲详情获取失败。");
            return "";
        }

        //处理歌词
        var sbLyrics = new StringBuilder();
        if (httpResult.data.lrclist != null && httpResult.data.lrclist.Count > 0)
        {
            foreach (var lyricInfo in httpResult.data.lrclist)
            {
                var ts = TimeSpan.FromSeconds(Convert.ToDouble(lyricInfo.time));
                string minutes = ts.Minutes.ToString();
                string seconds = ts.Seconds.ToString();
                string milliseconds = ts.Milliseconds.ToString();
                string time = $"[{minutes.PadLeft(2, '0')}:{seconds.PadLeft(2, '0')}.{milliseconds.PadLeft(3, '0')}]";
                sbLyrics.AppendLine($"{time}{lyricInfo.lineLyric}");
            }
        }

        return sbLyrics.ToString();
    }

    public async Task<PlatformMusicTag?> GetMusicTagsAsync()
    {
        try
        {
            string html = await _httpClient.GetStringAsync("http://www.kuwo.cn").ConfigureAwait(false);
            var hotTags = KuWoUtils.GetHotTags(html);
            if (!hotTags.Any())
            {
                throw new Exception("酷我热门标签获取失败");
            }
            string url = $"{UrlBase.KuWo.GetAllTypesUrl}?httpsStatus=1&reqId={_reqId}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            request.Headers.Add("Referer", "http://www.kuwo.cn/");
            request.Headers.Add("Host", "www.kuwo.cn");
            request.Headers.Add("csrf", _csrf);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var allTypes = KuWoUtils.GetAllTypes(json);
            if (!allTypes.Any())
            {
                throw new Exception("酷我歌曲标签获取失败");
            }
            return new PlatformMusicTag(hotTags, allTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "酷我标签获取失败。");
            return default;
        }
    }

    public async Task<List<SongMenu>> GetSongMenusFromTagAsync(string id, int page)
    {
        var songMenus = new List<SongMenu>();
        try
        {
            string url = $"{UrlBase.KuWo.GetTagSongMenuUrl}?pn={page}&rn=20&id={id}&httpsStatus=1&reqId={_reqId}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            request.Headers.Add("Referer", "http://www.kuwo.cn/playlists");
            request.Headers.Add("Host", "www.kuwo.cn");
            request.Headers.Add("csrf", _csrf);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var tagSongMenus = KuWoUtils.GetTagSongMenus(json);

            foreach (var tagSongMenu in tagSongMenus)
            {
                try
                {
                    songMenus.Add(new SongMenu()
                    {
                        Id = tagSongMenu.id,
                        ImageUrl = tagSongMenu.img,
                        Name = tagSongMenu.name,
                        LinkUrl = ""
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "酷我标签歌单添加失败。");
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "酷我标签歌单获取失败。");
        }
        return songMenus;
    }

    public async Task<List<Music>> GetTagMusicsAsync(string tagId)
    {
        var musics = new List<Music>();
        try
        {
            string url = $"{UrlBase.KuWo.GetTagMusicsUrl}?pid={tagId}&pn=1&rn=20&httpsStatus=1&reqId={_reqId}";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            request.Headers.Add("Referer", "http://www.kuwo.cn/playlists");
            request.Headers.Add("Host", "www.kuwo.cn");
            request.Headers.Add("csrf", _csrf);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var bangMusics = KuWoUtils.GetSongMenuMusics(json);

            foreach (var bangMusic in bangMusics)
            {
                try
                {
                    musics.Add(new Music()
                    {
                        Id = MD5Utils.GetStringValueToLower($"{Platform}-{bangMusic.rid}"),
                        Platform = Platform,
                        IdOnPlatform = bangMusic.rid.ToString(),
                        Name = bangMusic.name.Replace("&nbsp;", " "),
                        Artist = bangMusic.artist.Replace("&nbsp;", " "),
                        Album = bangMusic.album.Replace("&nbsp;", " "),
                        Fee = GetFeeFlag(bangMusic.payInfo.listen_fragment),
                        Duration = TimeSpan.FromSeconds(bangMusic.duration),
                        ImageUrl = bangMusic.pic
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "酷我榜单歌曲添加失败。");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "酷我榜单歌曲获取失败。");
        }
        return musics;
    }

    public async Task<string> GetPlayUrlAsync(string id, string extendDataJson = "")
    {
        try
        {
            string url = $"{UrlBase.KuWo.GetMusicUrl}?mid={id}&type=convert_url";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("User-Agent", BrowserDefaultHeader.EdgeUserAgent);
            request.Headers.Add("Referer", "http://www.kuwo.cn/");
            request.Headers.Add("Host", "www.kuwo.cn");
            request.Headers.Add("csrf", _csrf);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var resultObj = json.ToObject<HttpResultBase<HttpPlayUrlResult>>();
            if (resultObj == null || resultObj.code != 200 || resultObj.data.url.IsEmpty())
            {
                _logger.LogInformation($"更新酷我播放地址失败，歌曲：{id}。");
                return "";
            }
            return resultObj.data.url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新酷我播放地址失败。");
            return "";
        }
    }

    public Task<string> GetImageUrlAsync(string id, string extendDataJson = "")
    {
        throw new NotImplementedException();
    }
}