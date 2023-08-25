using NetMusicLib.Enums;
using NetMusicLib.Models;

namespace NetMusicLib.SearchProvider;
public class SearchManager
{
    private readonly List<ISearch> _searches;

    public SearchManager(params ISearch[] searches)
    {
        _searches = new List<ISearch>(searches);
    }

    public async Task<List<Music>> DoSearchesAsync(PlatformEnum searchTypes, string keyword)
    {
        var combinedResults = new List<Music>();
        foreach (var search in _searches)
        {
            if ((search.Platform & searchTypes) != 0)
            {
                combinedResults.AddRange(await search.DoSearchAsync(keyword));
            }
        }
        return combinedResults;
    }
}