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

            string imageUrl = ImageHandler.GetImageUrl("confusedlulu");

            string description = "I can create teams of league summoners with selected champions and put two teams againts each other.\n" +
                                 "There are commands called from a server I'm in and commands sent directly to my PMs. Team creations are done from PMs, while battling teams can be done in a server channel.\n" +
                                 "Below are main commands, call `help command` to get more info on how to use it and see its subcommands.";
            EmbedBuilder message = messageHandler.BuildEmbed("How to use me properly: ", $"{description}\n ", Palette.Pink, commandTitles, commandDescriptions, imageUrl);

            await ReplyAsync("", false, message.Build());
        }
    }
}
