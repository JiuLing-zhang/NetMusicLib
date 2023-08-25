using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchProvider;
public class NetEaseSearch : ISearch
{
    public PlatformEnum Platform => PlatformEnum.NetEase;

    private readonly IMusicProvider _myMusicProvider;
    public NetEaseSearch(IEnumerable<IMusicProvider> musicProviderList)
    {
        _myMusicProvider = musicProviderList.First(x => x.Platform == Platform);
    }
    public async Task<List<Music>> DoSearchAsync(string keyword)
    {
        return await _myMusicProvider.SearchAsync(keyword);
    }
}
