using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchMusic;
public class NetEaseSearcher : SearchAbstract
{
    private readonly IMusicProvider _myMusicProvider;
    public NetEaseSearcher() : base(PlatformEnum.NetEase)
    {
        _myMusicProvider = MusicProviderFactory.Create(PlatformEnum.NetEase);
    }

    public override async Task<List<MusicResultShow>> DoSearchAsync(string keyword, List<MusicResultShow> allResult)
    {
        var musics = await _myMusicProvider.SearchAsync(keyword);
        allResult.AddRange(musics);
        return allResult;
    }
}
