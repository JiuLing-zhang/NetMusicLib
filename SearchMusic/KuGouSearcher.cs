using NetMusicLib.Enums;
using NetMusicLib.Models;
using NetMusicLib.MusicProvider;

namespace NetMusicLib.SearchMusic;
internal class KuGouSearcher : SearchAbstract
{
    private readonly IMusicProvider _myMusicProvider;
    public KuGouSearcher() : base(PlatformEnum.KuGou)
    {
        _myMusicProvider = MusicProviderFactory.Create(PlatformEnum.KuGou);
    }

    public override async Task<List<Music>> DoSearchAsync(string keyword, List<Music> allResult)
    {
        var musics = await _myMusicProvider.SearchAsync(keyword);
        allResult.AddRange(musics);
        return allResult;
    }
}