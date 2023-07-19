using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchMusic;
internal class NetEaseSearcher : SearchAbstract
{
    private readonly IMusicProvider _myMusicProvider;
    public NetEaseSearcher() : base(PlatformEnum.NetEase)
    {
        _myMusicProvider = MusicProviderFactory.Create(PlatformEnum.NetEase);
    }

    public override async Task<List<Music>> DoSearchAsync(string keyword, List<Music> allResult)
    {
        var musics = await _myMusicProvider.SearchAsync(keyword);
        allResult.AddRange(musics);
        return allResult;
    }
}
