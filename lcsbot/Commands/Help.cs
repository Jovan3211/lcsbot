using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using lcsbot.Services;
using Discord;

namespace lcsbot.Commands
{
    [Group("help")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        private string helpImageName = "confusedlulu";

        [Command("")]
        public async Task HelpCommand()
        {
            MessageHandler messageHandler = new MessageHandler();

            List<string> commandTitles = new List<string>
            {
                "team", "!battle"
            };
            List<string> commandDescriptions = new List<string>
            {
                "Team creation and management command.", "Battling, idk tene write this"
            };

            string imageUrl = ImageHandler.GetImageUrl(helpImageName);
            string description = "I can create teams of league summoners with selected champions and put two teams against each other.\n" +
                                 "There are commands called from a server I'm in and commands sent directly to my PMs. Team creations are done from PMs, while simulating matches can be done in a server channel.\n" +
                                 "Below are my main commands, call `help command` to get more info on how to use it and see its subcommands.";
            EmbedBuilder message = messageHandler.BuildEmbed("How to use me properly: ", $"{description}", Palette.Pink, commandTitles, commandDescriptions, imageUrl);

            await ReplyAsync("", false, message.Build());
        }

        [Command("team"), RequireContext(ContextType.Guild)]
        public async Task TeamHelpCommandGuild()
        {
            MessageHandler messageHandler = new MessageHandler();

            string imageUrl = ImageHandler.GetImageUrl(helpImageName);
            string description = "PM me the the same command to view help on it.";
            EmbedBuilder message = messageHandler.BuildEmbed("This command is used in private messages ", $"{description}", Palette.Pink, imageUrl);

            await ReplyAsync("", false, message.Build());
        }

        [Command("team"), RequireContext(ContextType.DM)]
        public async Task TeamHelpCommand()
        {
            MessageHandler messageHandler = new MessageHandler();

            List<string> commandTitles = new List<string>
            {
                "team view", "team save", "team addplayer 'Summoner name/id' 'champ name/id' `role` `region`", "team removeplayer `ask tene about this`"
            };
            List<string> commandDescriptions = new List<string>
            {
                "Displays your current team.", "Saves your team settings.", "Adds a summoner with a champion to your team.", "Removes a summoner and his champ from your team."
            };

            string imageUrl = ImageHandler.GetImageUrl(helpImageName);
            string description = "a description";
            EmbedBuilder message = messageHandler.BuildEmbed("Team creation and management help ", $"{description}", Palette.Pink, commandTitles, commandDescriptions, imageUrl);

            await ReplyAsync("", false, message.Build());
        }
    }
}
