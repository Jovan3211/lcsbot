using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using lcsbot.Classes;
using RiotSharp;
using lcsbot.Services;

namespace lcsbot.Commands
{
    public class SimulateMatch : ModuleBase<SocketCommandContext>
    {
        RiotApi api = RiotApi.GetDevelopmentInstance(Settings.RiotAPIKey);
        #region command
        [Command("battle")]
        public async Task battle(SocketUser user2)
        { 
            var user1 = Context.User;

            Team team1 = new Team(user1.Id.ToString());
            Team team2 = new Team(user2.Id.ToString());
            team1.GetExistingTeamFromUser();
            team2.GetExistingTeamFromUser();

            MessageHandler handler = new MessageHandler();
            await ReplyAsync("", false, handler.GetDebugEmbed().Build());
        }
        #endregion

        public async Task CompareTeamsCompoundKDA(Team team1, Team team2)
        {
            foreach (var summoner1 in team1.Summoners)
            {
                long id = long.Parse(summoner1.SummonerId);
                var summonerForId = api.GetSummonerBySummonerId(RiotSharp.Misc.Region.euw, id);
                var recentMatches = api.GetRecentMatches(RiotSharp.Misc.Region.euw, summonerForId.AccountId);
                int team1TotalledKills = 0;
                int team1TotalledDeaths;
                int team1TotalledAssists;
                

                foreach (var match in recentMatches)
                {
                    Console.Write("hi guys its ME");
                    var indepthMatch = api.GetMatch(RiotSharp.Misc.Region.euw, match.GameId);
                    var participant = indepthMatch.Participants.Where(c => c.ParticipantId == id);
                    Int64 kills = participant.First().Stats.Kills;
                    team1TotalledKills += (int)kills;
                    await ReplyAsync("current teams kills: " + team1TotalledKills);
                }
            }
        }
        
    }

}
