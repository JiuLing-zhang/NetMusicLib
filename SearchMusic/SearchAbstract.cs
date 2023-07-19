using NetMusicLib.Enums;
using NetMusicLib.Models;

namespace NetMusicLib.SearchMusic;
internal abstract class SearchAbstract
{
    private SearchAbstract? _nextHandler;
    private readonly PlatformEnum _platform;
    protected SearchAbstract(PlatformEnum platform)
    {
        _platform = platform;
    }
    public void SetNextHandler(SearchAbstract nextHandler)
    {
        _nextHandler = nextHandler;
    }

    public async Task<List<Music>> SearchAsync(PlatformEnum platform, string keyword)
    {
        return await SearchAsync(platform, keyword, new List<Music>());
    }

    protected async Task<List<Music>> SearchAsync(PlatformEnum platform, string keyword, List<Music> allResult)
    {
        if ((platform & _platform) == _platform)
        {
            await DoSearchAsync(keyword, allResult);
        }
        if (_nextHandler != null)
        {
            await _nextHandler.SearchAsync(platform, keyword, allResult);
        }
        return allResult;
    }
    public abstract Task<List<Music>> DoSearchAsync(string keyword, List<Music> allResult);
}
