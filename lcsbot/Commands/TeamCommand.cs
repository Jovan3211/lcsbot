using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using lcsbot.Services;
using Discord;
using RiotSharp;
using lcsbot.Riot;
using System;
using System.Linq;

namespace lcsbot.Commands
{
    [Group("team"), RequireContext(ContextType.DM)]
    public class TeamCommand : ModuleBase<SocketCommandContext>
    {
        private List<string> teamPlayerIds = new List<string>();
        private List<string> teamChampionIds = new List<string>();
        private List<string> teamRoles = new List<string>();

        private bool saved = false;

        [Command("")]
        public async Task HelpCommand()
        {
            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Team creation and management command", $"Use `help team` to see how to use it.", Palette.Pink);

            await ReplyAsync("", false, message.Build());
        }

        [Command("view")]
        public async Task View()
        {
            string savedMessage = "";
            if (saved)
                savedMessage = "Team is saved.";
            else
                savedMessage = "Team is not saved, use `team save` when done.";

            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Your current team setup: ", savedMessage, Palette.Pink, GetNamesForSummoners(), GetNamesForChampions());

            await ReplyAsync("", false, message.Build());
        }

        [Command("addplayer")]
        public async Task AddPlayer(string summoner, string champion, string role)
        {
            string savedMessage = "";

            teamPlayerIds.Add(RiotAPI.api.GetSummonerByName(RiotSharp.Misc.Region.global, summoner).Id.ToString());
            teamChampionIds.Add(GetChampionByName(champion).Id.ToString());
            teamRoles.Add(role);

            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Added player to your team", "", Palette.Pink);

            await ReplyAsync("", false, message.Build());
        }

        private List<string> GetNamesForSummoners()
        {
            List<string> playerNames = new List<string>();

            foreach (string playerid in teamPlayerIds)
            {
                try
                {
                    playerNames.Add(RiotAPI.api.GetSummonerByAccountId(RiotSharp.Misc.Region.global, long.Parse(playerid)).Name);
                }
                catch (Exception e)
                {
                    Debugging.Log("GetNamesForSummoners", $"Error getting summoner and/or adding to list: {e.Message}");
                }
            }

            return playerNames;
        }

        private List<string> GetNamesForChampions()
        {
            List<string> champNames = new List<string>();

            foreach (string champId in teamChampionIds)
            {
                try
                {
                    champNames.Add(GetChampionNameById("asd").Name);
                }
                catch (Exception e)
                {
                    Debugging.Log("GetNamesForChampions", $"Error getting champion and/or adding to list: {e.Message}");
                }
            }

            return champNames;
        }

        private RiotSharp.StaticDataEndpoint.Champion.ChampionStatic GetChampionByName(string name)
        {
            var champions = StaticRiotApi.GetInstance(Settings.RiotAPIKey).GetChampions(RiotSharp.Misc.Region.euw).Champions;
            return champions.First(c => c.Key == name).Value;
        }

        // todo: try catch for shit
        private RiotSharp.StaticDataEndpoint.Champion.ChampionStatic GetChampionNameById(string id)
        {
            var champions = StaticRiotApi.GetInstance(Settings.RiotAPIKey).GetChampions(RiotSharp.Misc.Region.euw).Champions;
            return champions.First(c => c.Value.ToString() == id).Value;
        }
    }
}
