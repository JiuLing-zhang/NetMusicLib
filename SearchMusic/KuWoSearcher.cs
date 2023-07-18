using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchMusic;
internal class KuWoSearcher : SearchAbstract
{
    private readonly IMusicProvider _myMusicProvider;
    public KuWoSearcher() : base(PlatformEnum.KuWo)
    {
        _myMusicProvider = MusicProviderFactory.Create(PlatformEnum.KuWo);
    }

    public override async Task<List<MusicResultShow>> DoSearchAsync(string keyword, List<MusicResultShow> allResult)
    {
        var musics = await _myMusicProvider.SearchAsync(keyword);
        allResult.AddRange(musics);
        return allResult;
    }
}
