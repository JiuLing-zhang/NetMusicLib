using NetMusicLib.Enums;
using Microsoft.Extensions.Logging;

namespace NetMusicLib;
public class GlobalSettings
{
    private GlobalSettings()
    {

    }
    internal static ILoggerFactory? LoggerFactory { get; set; }
}