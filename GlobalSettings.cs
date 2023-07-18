using NetMusicLib.Enums;
using Microsoft.Extensions.Logging;

namespace NetMusicLib;
public class GlobalSettings
{
    private GlobalSettings()
    {

    }

    public static MusicFormatTypeEnum MusicFormatType { get; set; } = MusicFormatTypeEnum.PQ;

    public static ILoggerFactory? LoggerFactory { get; set; }
}