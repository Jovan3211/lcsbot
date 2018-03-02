using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using RiotSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using lcsbot.Services;
using Discord;

namespace lcsbot.Commands
{
    [Group("team"), RequireContext(ContextType.DM)]
    public class TeamCommand : ModuleBase<SocketCommandContext>
    {
        private List<string> teamPlayerIds = new List<string>();
        private bool saved = false;

        [Command("")]
        public async Task HelpCommand()
        {
            var champions = StaticRiotApi.GetInstance(Settings.RiotAPIKey).GetChampions(RiotSharp.Misc.Region.euw).Champions;
            var champ = champions.First(c => c.Key == "Aatrox").Value;

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
            EmbedBuilder message = messageHandler.BuildEmbed("Your current team setup: ", savedMessage, Palette.Pink, teamPlayerIds, GetNamesForPlayers());

            await ReplyAsync("", false, message.Build());
        }

        private List<string> GetNamesForPlayers()
        {
            List<string> playerNames = new List<string>();

            foreach (string player in teamPlayerIds)
            {
                //get username from riotapi by summoner id
                playerNames.Add(player);
            }

            return playerNames;
        }
    }
}
