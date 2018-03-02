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
            MessageHandler handler = new MessageHandler();

            Lane LANE = new Lane();
            Role ROLE = new Role();
            switch (role.ToUpper())
            {
                case ("ADC"):
                    LANE = Lane.Bot;
                    ROLE = Role.DuoCarry;
                    break;
                case ("JUNGLE"):
                    LANE = Lane.Jungle;
                    ROLE = Role.None;
                    break;
                case ("MID"):
                    LANE = Lane.Mid;
                    ROLE = Role.Solo;
                    break;
                case ("TOP"):
                    LANE = Lane.Top;
                    ROLE = Role.Solo;
                    break;
                case ("SUPPORT"):
                    LANE = Lane.Bot;
                    ROLE = Role.DuoSupport;
                    break;
                default:
                    await ReplyAsync("", false, handler.BuildEmbed("Hmm... Doesn't look like that's a proper role!", "Try one of these instead!: `ADC`  `SUPPORT`  `MID`  `TOP`  `JUNGLE`"));
                    break;
                


            }

            
            string savedMessage = "";

            Summoner newSummoner = new Summoner(RiotAPIClass.api.GetSummonerByName(RiotSharp.Misc.Region.euw, summoner).Id.ToString(), GetChampionByName(champion).Id.ToString(), LANE, ROLE);

            MessageHandler messageHandler = new MessageHandler();
            EmbedBuilder message = messageHandler.BuildEmbed("Added player to your team", "", Palette.Pink);

            await ReplyAsync("", false, message.Build());
        }


        /*private List<string> GetNamesForSummoners()
        {
            List<string> playerNames = new List<string>();

            foreach (string playerid in teamPlayerIds)
            {
                try
                {
                    playerNames.Add(RiotAPIClass.api.GetSummonerByAccountId(RiotSharp.Misc.Region.euw, long.Parse(playerid)).Name);
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
        }*/

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
