namespace NetMusicLib.Models.KuGou;
internal class HttpResultBase<T>
{
    public int status { get; set; }
    public int error_code { get; set; }
    public T data { get; set; } = default(T)!;
}