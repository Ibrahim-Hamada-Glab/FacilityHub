using UAParser;

namespace FacilityHub.helper;

// Helpers/UserAgentHelper.cs
public static class UserAgentHelper
{
    public static string GetShortUserAgent(string rawUserAgent)
    {
        if (string.IsNullOrEmpty(rawUserAgent))
            return "Unknown";

        var parser = Parser.GetDefault();
        var clientInfo = parser.Parse(rawUserAgent);

        return $"{clientInfo.UA.Family} {clientInfo.UA.Major} - {clientInfo.OS.Family} {clientInfo.OS.Major}";
    }
}