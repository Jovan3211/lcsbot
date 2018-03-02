using RiotSharp;
using lcsbot.Services;

namespace lcsbot.Riot
{
    public enum Role { Duo, DuoCarry, DuoSupport, None, Solo }
    public enum Lane { Bot, Jungle, Mid, Top, Support }

    public static class RiotAPIClass
    {
        public static RiotApi api = RiotApi.GetDevelopmentInstance(Settings.RiotAPIKey);
        public static StaticRiotApi staticapi = StaticRiotApi.GetInstance(Settings.RiotAPIKey);
    }
}
