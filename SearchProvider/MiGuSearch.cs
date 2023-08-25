using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchProvider;
public class MiGuSearch : ISearch
{
    public PlatformEnum Platform => PlatformEnum.MiGu;
    
    private readonly IMusicProvider _myMusicProvider;
    public MiGuSearch(IEnumerable<IMusicProvider> musicProviderList)
    {
        _myMusicProvider = musicProviderList.First(x => x.Platform == Platform);
    }
    public async Task<List<Music>> DoSearchAsync(string keyword)
    {
        return await _myMusicProvider.SearchAsync(keyword);
    }
}
