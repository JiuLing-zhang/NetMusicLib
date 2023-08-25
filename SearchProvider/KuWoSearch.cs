using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchProvider;
public class KuWoSearch : ISearch
{
    public PlatformEnum Platform => PlatformEnum.KuWo;

    private readonly IMusicProvider _myMusicProvider;
    public KuWoSearch(IEnumerable<IMusicProvider> musicProviderList)
    {
        _myMusicProvider = musicProviderList.First(x => x.Platform == Platform);
    }
    public async Task<List<Music>> DoSearchAsync(string keyword)
    {
        return await _myMusicProvider.SearchAsync(keyword);
    }
}