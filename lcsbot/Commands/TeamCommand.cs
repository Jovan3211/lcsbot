using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using RiotSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using lcsbot.Services;
using Discord;
using RiotSharp;
using lcsbot.Riot;
using System;
using System.Linq;
using lcsbot.Classes;

namespace lcsbot.Commands
{
    [Group("team"), RequireContext(ContextType.DM)]
    public class TeamCommand : ModuleBase<SocketCommandContext>
    {
        public static List<User> userList = new List<User>();
        private string[] haventStartedMessage = { "Whoops, looks like you haven't started team building.", "Type `team start` to begin and `help team` for assistance." };

        [Command("")]
        public async Task HelpCommand()
        {
            var champions = StaticRiotApi.GetInstance(Settings.RiotAPIKey).GetChampions(RiotSharp.Misc.Region.euw).Champions;
            var champ = champions.First(c => c.Key == "Aatrox").Value;

            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Team creation and management command", $"Use `help team` to see how to use it.", Palette.Pink);

            await ReplyAsync("", false, message.Build());
        }

        [Command("start")]
        public async Task Start()
        {
            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Team creation and management", $"Started creating and editing team. Write `team end` when done.", Palette.Pink);

            if (CheckStarted(Context.User.Id.ToString()))
                message = messageHandler.BuildEmbed("Team creation and management", $"You've already started.", Palette.Pink);
            else
            {
                User user = new User(Context.User.Id.ToString(), Context.User.Username);
                userList.Add(user);
            }

            await ReplyAsync("", false, message.Build());
        }

        [Command("end")]
        public async Task End()
        {
            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Team creation and management", $"Finished team creation and editing.", Palette.Pink);

            if (!CheckStarted(Context.User.Id.ToString()))
                message = messageHandler.BuildEmbed("Team creation and management", $"You haven't started.", Palette.Pink);
            else
                userList.RemoveAll(x => x.UserId == Context.User.Id.ToString());

            await ReplyAsync("", false, message.Build());
        }

        [Command("view")]
        public async Task View()
        {
            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message;
            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(a => a.WithIconUrl(Context.User.GetAvatarUrl()).WithName(Context.User.Username + "'s team"))
                .WithTitle("Here's your lineup!")
                .WithColor(Palette.Pink);

            if (CheckStarted(Context.User.Id.ToString()))
            {
                string savedMessage = "";
                var user = GetUserByIdFromList(Context.User.Id.ToString());

                if (user.Saved)
                    savedMessage = "Your team is currently saved! Don't worry!";
                else
                    savedMessage = "Team isn't currently saved! Use `team save` when you're done!";

                var summonersInUserTeam = GetNamesForSummonersInUserTeam(GetUserByIdFromList(Context.User.Id.ToString()));
                var namesForChampionsInUserTeam = GetNamesForChampionsInUserTeam(GetUserByIdFromList(Context.User.Id.ToString()));

                if (summonersInUserTeam.Count > 1)
                    message = messageHandler.BuildEmbed("Your current team setup: ", savedMessage, Palette.Pink, summonersInUserTeam, namesForChampionsInUserTeam);
                else
                    message = messageHandler.BuildEmbed("Your current team setup: ", savedMessage, Palette.Pink, new List<string>{ "It's empty!" }, new List<string> { "See `help` to see how to add" });
            }
            else
            {
                try
                {
                    string userid = Context.User.Id.ToString();

                    User user = new User(userid, Context.User.Username);

                    Team team = new Team(userid);
                    team.GetExistingTeamFromUser();
                    

                    user.SetTeam(team);

                    int counter = 0;
                    foreach(var summoner in team.Summoners)
                    {
                        counter++;
                        string role;
                        string incrementor;
                        switch (summoner.Lane)
                        {
                            case (Lane.Bot): role = "ADC"; break;
                            case (Lane.Jungle): role = "Jungler"; break;
                            case (Lane.Mid): role = "Midlaner"; break;
                            case (Lane.Top): role = "Top"; break;
                            case (Lane.Support): role = "Support"; break;
                            default: return;
                        }                   
                        switch(counter)
                        {
                            case 1: incrementor = "Alright! First up we've got: "; break;
                            case 2: incrementor = "Secondly, it's: "; break;
                            case 3: incrementor = "Next up!: "; break;
                            case 4: incrementor = "Let's not forget about "; break;
                            case 5: incrementor = "And last, but not least! It's "; break;
                            default: return;
                        }
                        long id = int.Parse(summoner.SummonerId);
                        embed.AddField(incrementor + RiotApi.GetDevelopmentInstance(Settings.RiotAPIKey).GetSummonerBySummonerId(summoner.Region, id).Name + " as SK" + Context.User.Username.ToUpperInvariant() + "'s " + role + "!", "Looks like they'll be playing... " + GetChampionNameById(summoner.ChampionId).Name + "!");
                    }

                    
                }
                catch (Exception e)
                {
                    message = messageHandler.BuildEmbed("Whoops! There's been an error getting your team. Make sure you have one!", $"A message for my creators: {e.Message}", Palette.Pink);
                    Debugging.Log("View command CHECKSTARTED ELSE", $"Error trying to get existing team from user: {e.Message}", LogSeverity.Error);
                }
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("save")]
        public async Task Save()
        {
            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message;

            if (CheckStarted(Context.User.Id.ToString()))
            {
                string savedMessage = "Alright! I've taken a note of your team! You can now simulate matches against other users using it!";
                var user = GetUserByIdFromList(Context.User.Id.ToString());

                if (user.CheckTeamReady())
                {
                    if (user.Saved)
                        savedMessage = "Team is already saved.";
                    else
                        user.SaveTeam();
                }
                else
                    savedMessage = "Hmm, looks like your loadout isn't quite ready yet. You gotta have five people in your team!";

                message = messageHandler.BuildEmbed("Save team", savedMessage, Palette.Pink, GetNamesForSummonersInUserTeam(GetUserByIdFromList(Context.User.Id.ToString())), GetNamesForChampionsInUserTeam(GetUserByIdFromList(Context.User.Id.ToString())));
            }
            else
            {
                message = messageHandler.BuildEmbed(haventStartedMessage[0], haventStartedMessage[1], Palette.Pink);
            }

            await ReplyAsync("", false, message.Build());
        }

        [Command("addplayer")]
        public async Task AddPlayer(string summoner, string champion, string roleInput, string region)
        {
            MessageHandler handler = new MessageHandler();

            if (CheckStarted(Context.User.Id.ToString()))
            {
                if (GetUserByIdFromList(Context.User.Id.ToString()).Team.Summoners.Count == 5)
                {
                    await ReplyAsync("", false, handler.BuildEmbed("Looks like your team is full!", "You can't have more than 5 summoners in your team. To remove one do `removesummoner idk need to talk to tene`"));
                    return;
                }

                bool correctLaneRole = true;
                Lane lane = new Lane();
                Role role = new Role();
                switch (roleInput.ToUpper())
                {
                    case ("ADC"):
                        lane = Lane.Bot;
                        role = Role.DuoCarry;
                        break;
                    case ("JUNGLE"):
                        lane = Lane.Jungle;
                        role = Role.None;
                        break;
                    case ("MID"):
                        lane = Lane.Mid;
                        role = Role.Solo;
                        break;
                    case ("TOP"):
                        lane = Lane.Top;
                        role = Role.Solo;
                        break;
                    case ("SUPPORT"):
                        lane = Lane.Bot;
                        role = Role.DuoSupport;
                        break;
                    default:
                        correctLaneRole = false;
                        await ReplyAsync("", false, handler.BuildEmbed("Hmm... Doesn't look like that's a proper role!", "Try one of these instead!: `ADC`  `SUPPORT`  `MID`  `TOP`  `JUNGLE`"));
                        break;
                }

                if (correctLaneRole)
                {
                    champion = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(champion);

                    RiotSharp.Misc.Region parsedRegion;
                    Enum.TryParse(region, out parsedRegion);

                    Summoner newSummoner = new Summoner(RiotAPIClass.api.GetSummonerByName(parsedRegion, summoner).Id.ToString(), GetChampionByName(champion).Id.ToString(), role, lane, parsedRegion);

                    var obj = GetUserByIdFromList(Context.User.Id.ToString());
                    if (obj != null) obj.AddSummonerToTeam(newSummoner);

                    await ReplyAsync("", false, handler.BuildEmbed("Added player to your team", "", Palette.Pink).Build());
                }
            }
            else
            {
                await ReplyAsync("", false, handler.BuildEmbed(haventStartedMessage[0], haventStartedMessage[1], Palette.Pink).Build());
            }
        }

        [Command("removeplayer")]
        public async Task RemovePlayer()
        {
            MessageHandler messageHandler = new MessageHandler();
            await ReplyAsync("", false, messageHandler.BuildEmbed("Looks like this isn't quite ready yet. Give me a bit more time, no need to rush ;)", "Not yet implemented").Build());
        }

        private User GetUserByIdFromList(string userId) => userList.FirstOrDefault(x => x.UserId == userId);

        private List<string> GetNamesForSummonersInUserTeam(User user)
        {
            List<string> summonerNames = new List<string>();

            foreach (string summonerId in user.Team.GetSummonersIds())
            {
                try
                {
                    summonerNames.Add(RiotAPIClass.api.GetSummonerBySummonerId(RiotSharp.Misc.Region.euw, long.Parse(summonerId)).Name);
                }
                catch (Exception e)
                {
                    Debugging.Log("GetNamesForSummoners", $"Error getting summoner and/or adding to list: {e.Message}");
                }
            }

            return summonerNames;
        }

        private List<string> GetNamesForChampionsInUserTeam(User user)
        {
            List<string> champNames = new List<string>();

            foreach (string champId in user.Team.GetSummonersChampionIds())
            {
                try
                {
                    champNames.Add(GetChampionNameById(champId).Name);
                }
                catch (Exception e)
                {
                    Debugging.Log("GetNamesForChampions", $"Error getting champion and/or adding to list: {e.Message}", LogSeverity.Error);
                }
            }

            return champNames;
        }

        private bool CheckStarted(string userId)
        {
            foreach (User user in userList)
            {
                if (user.UserId == userId)
                    return true;
            }

            return false;
        }

        private RiotSharp.StaticDataEndpoint.Champion.ChampionStatic GetChampionByName(string name)
        {
            var champions = StaticRiotApi.GetInstance(Settings.RiotAPIKey).GetChampions(RiotSharp.Misc.Region.euw).Champions;
            return champions.First(c => c.Key == name).Value;
        }

        private RiotSharp.StaticDataEndpoint.Champion.ChampionStatic GetChampionNameById(string id)
        {
            try
            {
                var champions = StaticRiotApi.GetInstance(Settings.RiotAPIKey).GetChampions(RiotSharp.Misc.Region.euw).Champions;
                return champions.First(c => c.Value.Id.ToString() == id).Value;
            }
            catch (Exception e)
            {
                Debugging.Log("GetChampionNameById", $"Error getting champion name by id: {e.Message}", LogSeverity.Error);
                return null;
            }
        }
    }
}
