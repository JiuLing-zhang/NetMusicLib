using NetMusicLib.Enums;

namespace NetMusicLib.MusicProvider;
internal class MusicProviderFactory
{
    public static IMusicProvider Create(PlatformEnum platform)
    {
        return platform switch
        {
            PlatformEnum.NetEase => NetEaseMusicProvider.GetInstance(),
            PlatformEnum.KuGou => KuGouMusicProvider.GetInstance(),
            PlatformEnum.MiGu => MiGuMusicProvider.GetInstance(),
            PlatformEnum.KuWo => KuWoMusicProvider.GetInstance(),
            _ => throw new ArgumentException("歌曲构建器生成失败：不支持的平台")
        };
    }
}