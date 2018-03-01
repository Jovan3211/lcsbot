using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using lcsbot.Services;
using lcsbot.Functions;

namespace lcsbot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private Random rand = new Random();

        public async Task RunBotAsync()
        {
            Settings._client = new DiscordSocketClient(Settings._config);
            Settings._commands = new CommandService();

            Settings._services = new ServiceCollection()
                .AddSingleton(Settings._client)
                .AddSingleton(Settings._commands)
                .BuildServiceProvider();

            Settings.LoadJson();
            Settings.SqlServerSetup();

            //subs
            Settings._client.Log += Debugging.Log;
            Settings._commands.Log += Debugging.Log;
            Settings._client.MessageUpdated += MessageUpdated;
            Settings._client.UserJoined += SqlHandler.TaskInsertUser;

            await RegisterCommandAsync();
            await Settings._client.LoginAsync(TokenType.Bot, Settings.BotToken);
            await Settings._client.StartAsync();
            await Task.Delay(-1);
        }

        public async Task RegisterCommandAsync()
        {
            Settings._client.MessageReceived += HandleCommandAsync;

            await Settings._commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            MessageHandler handler = new MessageHandler();

            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;

            int argPos = 0;

            if ((message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(Settings._client.CurrentUser, ref argPos)) && !CheckPrivate(message))
            {
                var context = new SocketCommandContext(Settings._client, message);
                Debugging.Log("Command Handler", $"{context.User.Username} called {message}");

                var result = await Settings._commands.ExecuteAsync(context, argPos, Settings._services);
                if (!result.IsSuccess && result.Error != CommandError.ObjectNotFound || result.Error != CommandError.Exception || result.ErrorReason != "The server responded with error 400: BadRequest")
                {
                    Debugging.Log("Command Handler", $"Error with command {message}: {result.ErrorReason.Replace(".", "")}", LogSeverity.Warning);
                    
                    if (result.ErrorReason == "Invalid context for command; accepted contexts: DM")
                        await arg.Channel.SendMessageAsync("", false, handler.BuildEmbed(":no_entry_sign:  Error!", "That command is used in private messages.").Build());
                    else
                        await arg.Channel.SendMessageAsync("", false, handler.BuildEmbed(":no_entry_sign:  Error!", result.ErrorReason).Build());
                }
            }
            else if (CheckPrivate(message)) //direct message
            {
                var context = new SocketCommandContext(Settings._client, message);

                if (!CheckUserInDatabase.Check(context.User.Id.ToString())) // checks if user is in database, if not, add
                {
                    await arg.Channel.SendMessageAsync("", false, handler.BuildEmbed("Hi there!", "First time meeting you, let me just add you to my database quickly.", ImageHandler.GetImageUrl("ahriwave")).Build());

                    User newUser = new User(context.User.Id.ToString(), context.User.Username);
                    if (newUser.AddToDatabase())
                        await arg.Channel.SendMessageAsync("", false, handler.BuildEmbed("Done!", "You're added, use `help` to see how to use me."));
                    else
                        await arg.Channel.SendMessageAsync("", false, handler.BuildEmbed("Something went wrong", "When attempting to add to database."));
                }

                Debugging.Log("Command Handler, DM", $"{context.User.Username} sent {message}");

                var result = await Settings._commands.ExecuteAsync(context, argPos, Settings._services);
                if (!result.IsSuccess && result.Error != CommandError.ObjectNotFound || result.Error != CommandError.Exception)
                {
                    Debugging.Log("Command Handler, DM", $"Error with command {message}: {result.ErrorReason.Replace(".", "")}", LogSeverity.Warning);
                    await arg.Channel.SendMessageAsync("", false, handler.BuildEmbed(":no_entry_sign:  DM Error!", result.ErrorReason).Build());
                }
            }
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
        }

        private bool CheckPrivate(SocketUserMessage message) => message.Channel.ToString().Contains(message.Author.ToString());
    }
}
