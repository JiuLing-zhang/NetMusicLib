using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchProvider;
public class KuGouSearch : ISearch
{
    public PlatformEnum Platform => PlatformEnum.KuGou;

    private readonly IMusicProvider _myMusicProvider;
    public KuGouSearch(IEnumerable<IMusicProvider> musicProviderList)
    {
        _myMusicProvider = musicProviderList.First(x => x.Platform == Platform);
    }

    public async Task<List<Music>> DoSearchAsync(string keyword)
    {
        return await _myMusicProvider.SearchAsync(keyword);
    }
}