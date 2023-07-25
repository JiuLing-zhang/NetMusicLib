using NetMusicLib.Enums;
using Microsoft.Extensions.Logging;

namespace NetMusicLib;
public class GlobalSettings
{
    private GlobalSettings()
    {

    }

    public static MusicFormatTypeEnum MusicFormatType { get; set; } = MusicFormatTypeEnum.PQ;

    internal static ILoggerFactory? LoggerFactory { get; set; }
}