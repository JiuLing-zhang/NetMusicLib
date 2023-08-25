using NetMusicLib.Enums;
using NetMusicLib.Models;

namespace NetMusicLib.SearchProvider;
public interface ISearch
{
    PlatformEnum Platform { get; }
    Task<List<Music>> DoSearchAsync(string keyword);
}