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
        private List<string> teamRoleIds = new List<string>();

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
            EmbedBuilder message = messageHandler.BuildEmbed("Your current team setup: ", savedMessage, Palette.Pink, GetNamesForSummoners(), GetNamesForSummoners());

            await ReplyAsync("", false, message.Build());
        }

        [Command("addplayer")]
        public async Task AddPlayer(string summoner, string champion)
        {
            string savedMessage = "";



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
            List<string> playerNames = new List<string>();

            foreach (string playerid in teamPlayerIds)
            {
                try
                {
                    playerNames.Add(GetChampionByName("asd"));
                }
                catch (Exception e)
                {
                    Debugging.Log("GetNamesForChampions", $"Error getting champion and/or adding to list: {e.Message}");
                }
            }

            return playerNames;
        }

        private RiotSharp.StaticDataEndpoint.Champion.ChampionStatic GetChampionByName(string name)
        {
            var champions = RiotAPI.staticapi.GetChampions(RiotSharp.Misc.Region.global).Champions;

            foreach (RiotSharp.StaticDataEndpoint.Champion.ChampionStatic champ in champions)
            {
                if (champ.Name == name)
                    return champ;
            }
        }
    }
}
