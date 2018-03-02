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

            if (CheckStarted(Context.User.Id.ToString()))
                message = messageHandler.BuildEmbed("Team creation and management", $"You haven't started.", Palette.Pink);
            else
                userList.RemoveAll(x => x.UserId == Context.User.Id.ToString());

            await ReplyAsync("", false, message.Build());
        }

        [Command("view")]
        public async Task View()
        {
            string savedMessage = "";
            var user = GetUserByIdFromList(Context.User.Id.ToString());

            if (user.Saved)
                savedMessage = "Team is saved.";
            else
                savedMessage = "Team is not saved, use `team save` when done.";

            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Your current team setup: ", savedMessage, Palette.Pink, GetNamesForSummonersInUserTeam(GetUserByIdFromList(Context.User.Id.ToString())), GetNamesForChampionsInUserTeam(GetUserByIdFromList(Context.User.Id.ToString())));

            await ReplyAsync("", false, message.Build());
        }

        [Command("save")]
        public async Task Save()
        {
            string savedMessage = "Team is saved successfully, nothing to worry about I'll keep it safe.";
            var user = GetUserByIdFromList(Context.User.Id.ToString());

            if (user.Saved)
                savedMessage = "Team is already saved.";
            else
                user.SaveTeam();

            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Save team", savedMessage, Palette.Pink, GetNamesForSummonersInUserTeam(GetUserByIdFromList(Context.User.Id.ToString())), GetNamesForChampionsInUserTeam(GetUserByIdFromList(Context.User.Id.ToString())));

            await ReplyAsync("", false, message.Build());
        }

        [Command("addplayer")]
        public async Task AddPlayer(string summoner, string champion, string roleInput)
        {
            MessageHandler handler = new MessageHandler();

            if (CheckStarted(Context.User.Id.ToString()))
            {
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
                    Summoner newSummoner = new Summoner(RiotAPIClass.api.GetSummonerByName(RiotSharp.Misc.Region.euw, summoner).Id.ToString(), GetChampionByName(champion).Id.ToString(), role, lane);

                    var obj = GetUserByIdFromList(Context.User.Id.ToString());
                    if (obj != null) obj.AddSummonerToTeam(newSummoner);

                    await ReplyAsync("", false, handler.BuildEmbed("Added player to your team", "", Palette.Pink).Build());
                }
            }
            else
                await ReplyAsync("", false, handler.BuildEmbed("Start team creating and editing with 'team start'.", "", Palette.Pink).Build());
        }

        private User GetUserByIdFromList(string userId) => userList.FirstOrDefault(x => x.UserId == userId);

        private List<string> GetNamesForSummonersInUserTeam(User user)
        {
            List<string> summonerNames = new List<string>();

            foreach (string summonerId in user.Team.GetSummonersIds())
            {
                try
                {
                    summonerNames.Add(RiotAPIClass.api.GetSummonerByAccountId(RiotSharp.Misc.Region.euw, long.Parse(summonerId)).Name);
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
                    Debugging.Log("GetNamesForChampions", $"Error getting champion and/or adding to list: {e.Message}");
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

        // #TODO: try catch for shit
        private RiotSharp.StaticDataEndpoint.Champion.ChampionStatic GetChampionNameById(string id)
        {
            var champions = StaticRiotApi.GetInstance(Settings.RiotAPIKey).GetChampions(RiotSharp.Misc.Region.euw).Champions;
            return champions.First(c => c.Value.ToString() == id).Value;
        }
    }
}
