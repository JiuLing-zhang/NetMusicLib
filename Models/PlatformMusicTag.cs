namespace NetMusicLib.Models;
public class PlatformMusicTag
{
    public List<MusicTag> HotTags { get; set; }

    public List<MusicTypeTag> AllTypes { get; set; }

    public PlatformMusicTag(List<MusicTag> hotTags, List<MusicTypeTag> allTypes)
    {
        HotTags = hotTags;
        AllTypes = allTypes;
    }
}

/// <summary>
/// 音乐类型标签
/// </summary>
public class MusicTypeTag
{
    public string TypeName { get; set; } = null!;
    public List<MusicTag> Tags { get; set; } = null!;
}

/// <summary>
/// 音乐标签
/// </summary>
public class MusicTag
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}