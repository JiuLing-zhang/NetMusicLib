using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchMusic;
internal class MiGuSearcher : SearchAbstract
{
    private readonly IMusicProvider _myMusicProvider;
    public MiGuSearcher() : base(PlatformEnum.MiGu)
    {
        _myMusicProvider = MusicProviderFactory.Create(PlatformEnum.MiGu);
    }

    public override async Task<List<Music>> DoSearchAsync(string keyword, List<Music> allResult)
    {
        var musics = await _myMusicProvider.SearchAsync(keyword);
        allResult.AddRange(musics);
        return allResult;
    }
}
