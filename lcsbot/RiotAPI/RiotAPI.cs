using RiotSharp;
using lcsbot.Services;

namespace lcsbot.Riot
{
    public static class RiotAPI
    {
        /// <summary>
        /// RiotApi Instance.
        /// </summary>
        public static RiotApi api = RiotApi.GetDevelopmentInstance(Settings.RiotAPIKey);
        public static StaticRiotApi staticapi = StaticRiotApi.GetInstance(Settings.RiotAPIKey);
    }
}
