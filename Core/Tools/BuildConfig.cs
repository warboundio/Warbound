namespace Core.Tools;

public static class BuildConfig
{
#if DEBUG
    public static bool IsDebug => true;
    public static bool IsRelease => false;
#else
    public static bool IsDebug => false;
    public static bool IsRelease => true;
#endif
}
