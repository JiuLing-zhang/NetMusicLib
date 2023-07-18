using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchMusic;
public class KuGouSearcher : SearchAbstract
{
    private readonly IMusicProvider _myMusicProvider;
    public KuGouSearcher() : base(PlatformEnum.KuGou)
    {
        _myMusicProvider = MusicProviderFactory.Create(PlatformEnum.KuGou);
    }

    public override async Task<List<MusicResultShow>> DoSearchAsync(string keyword, List<MusicResultShow> allResult)
    {
        var musics = await _myMusicProvider.SearchAsync(keyword);
        allResult.AddRange(musics);
        return allResult;
    }
}