using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Configs;

public enum EProgramType
{
    Client = 0,
    Scene = 1,
    Chat = 2,
}
public class Config
{
    public long ServerId;
    public List<int> ListenPort;
}

public static class CommandLine
{
    public static string getOpt(string[] args, int idx, string defaultValue)
    {
        if (args == null || args.Length <= 0)
            return defaultValue;

        if (idx >= args.Length)
            return defaultValue;

        return args[idx];
    }

    public static string getOpt(string[] args, string opt, string defaultValue)
    {
        if (args == null || args.Length <= 0 || string.IsNullOrEmpty(opt) == true)
            return defaultValue;
        int idx = Array.FindIndex(args, (p) => { return p == opt; });
        if (idx < 0)
            return defaultValue;
        else
            return getOpt(args, idx + 1, defaultValue);
    }
}