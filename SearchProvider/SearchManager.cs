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

    public Task<List<Music>> DoSearchesAsync(PlatformEnum searchTypes, string keyword)
    {
        var tasks = new List<Task<List<Music>>>();
        foreach (var search in _searches)
        {
            if ((search.Platform & searchTypes) != 0)
            {
                tasks.Add(Task.Run(async () => await search.DoSearchAsync(keyword)));
            }
        }
        var combinedResults = new List<Music>();
        if (tasks.Any())
        {
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                combinedResults.AddRange(task.Result);
            }
        }
        return Task.FromResult(combinedResults);
    }
}