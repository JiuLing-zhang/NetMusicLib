using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchMusic;
public class MiGuSearcher : SearchAbstract
{
    private readonly IMusicProvider _myMusicProvider;
    public MiGuSearcher() : base(PlatformEnum.MiGu)
    {
        _myMusicProvider = MusicProviderFactory.Create(PlatformEnum.MiGu);
    }

    public override async Task<List<MusicResultShow>> DoSearchAsync(string keyword, List<MusicResultShow> allResult)
    {
        var musics = await _myMusicProvider.SearchAsync(keyword);
        allResult.AddRange(musics);
        return allResult;
    }
}
