using Microsoft.Extensions.DependencyInjection;
using NetMusicLib.MusicProvider;
using NetMusicLib.SearchProvider;

namespace NetMusicLib;
public static class MusicNetPlatformExtensions
{
    public static void AddMusicNetPlatform(this IServiceCollection services)
    {
        services.AddSingleton<IMusicProvider, NetEaseMusicProvider>();
        services.AddSingleton<IMusicProvider, KuGouMusicProvider>();
        services.AddSingleton<IMusicProvider, MiGuMusicProvider>();
        services.AddSingleton<IMusicProvider, KuWoMusicProvider>();

        services.AddSingleton<ISearch, MiGuSearch>();
        services.AddSingleton<ISearch, KuWoSearch>();
        services.AddSingleton<ISearch, NetEaseSearch>();
        services.AddSingleton<ISearch, KuGouSearch>();
        services.AddSingleton<SearchManager>(provider =>
        {
            var searches = provider.GetServices<ISearch>().ToArray();
            return new SearchManager(searches);
        });

        services.AddSingleton<MusicNetPlatform>();         
    }
}