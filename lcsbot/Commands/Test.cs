using Discord;
using Discord.Commands;
using lcsbot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using RestSharp;

namespace lcsbot.Commands
{
    public class Test : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task TestCommand()
        {
            //use this to test whatever

            string requestString = @"http://api.champion.gg/champion/ahri/summaaoners/mostWins?api_key=9d30e87ffd52401b8387aa20cf2c701e";

            IRestResponse data = HTTPHandler.GET(requestString);

            MessageHandler messageHandler = new MessageHandler();
            await ReplyAsync("", false, messageHandler.BuildEmbed("testing", $"{data.Content}; {data.StatusCode}; {data.StatusDescription}; {data.ResponseStatus}; {data.IsSuccessful}", Palette.Pink).Build());
        }
    }
}