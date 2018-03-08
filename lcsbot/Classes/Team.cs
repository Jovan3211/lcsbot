using Discord.WebSocket;
using lcsbot.Riot;
using Discord;
using lcsbot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lcsbot.Classes
{
    public class Team : ILCSBOTClass
    {
        private string userId;
        private List<Summoner> summoners;

        public string UserId { get => userId; set => userId = value; }
        public List<Summoner> Summoners { get => summoners; }

        public Team(string userId)
        {
            summoners = new List<Summoner>();
            this.userId = userId;
        }

        public bool GetExistingTeamFromUser()
        {
            try
            {
                List<string> selectionSummonerIds = SqlHandler.Select("Teams", "Summoner1Id,Summoner2Id,Summoner3Id,Summoner4Id,Summoner5Id", $"UserId='{userId}'");
                selectionSummonerIds = selectionSummonerIds[0].Split(',').ToList();

                summoners.Clear();
                foreach (string summonerId in selectionSummonerIds)
                {
                    List<string> summonerSelection = SqlHandler.Select("Summoners", "SummonerId,ChampionId,Role,Lane,Region", $"Id='{summonerId}'");
                    summonerSelection = summonerSelection[0].Split(',').ToList();


                    Role role;
                    Enum.TryParse(summonerSelection[2], out role);
                    Lane lane;
                    Enum.TryParse(summonerSelection[3], out lane);
                    RiotSharp.Misc.Region region;
                    Enum.TryParse(summonerSelection[4], out region);

                    summoners.Add(new Summoner(summonerSelection[0], summonerSelection[1], role, lane, region));
                }

                return true;
            }
            catch (Exception e)
            {
                Debugging.Log("GetExistingTeamFromUser", $"Error getting existing team from user: {e.Message}", Discord.LogSeverity.Error);
                
                return false;
            }
            
        }

        public bool AddSummoner(Summoner summoner)
        {
            if (summoners.Count < 5)
            {
                summoners.Add(summoner);
                return true;
            }
            else
                return false;
        }

        public bool RemoveSummoner(Summoner summoner)
        {
            if (summoners.Count > 0)
            {
                summoners.Remove(summoner);
                return true;
            }
            else
                return false;
        }

        public bool CheckReady()
        {
            if (summoners.Count == 5)
                return true;

            return false;
        }

        public List<string> GetSummonersChampionIds()
        {
            List<string> returnList = new List<string>();

            foreach (Summoner summoner in summoners)
            {
                returnList.Add(summoner.ChampionId);
            }

            return returnList;
        }

        public List<string> GetSummonersIds()
        {
            List<string> returnList = new List<string>();

            foreach (Summoner summoner in summoners)
            {
                returnList.Add(summoner.SummonerId);
            }

            return returnList;
        }

        public bool AddToDatabase()
        {
            List<int> summonerIds = new List<int>();

            if (!CheckReady())
            {
                Debugging.Log("Create team", $"Ready check, team is not ready, not enough or too many players.");
                return false;
            }

            try
            {
                foreach (Summoner summoner in summoners)
                {
                    summonerIds.Add(summoner.AddToDatabase());
                }

                SqlHandler.Insert("Teams(UserId, Summoner1Id, Summoner2Id, Summoner3Id, Summoner4Id, Summoner5Id)", $"'{userId}', '{summonerIds[0]}', '{summonerIds[1]}', '{summonerIds[2]}', '{summonerIds[3]}', '{summonerIds[4]}'");

                Debugging.Log("Create team", $"Created team for user: {userId} with summoners: {summonerIds[0]}, {summonerIds[1]}, {summonerIds[2]}, {summonerIds[3]}, {summonerIds[4]}");
                return true;
            }
            catch (Exception e)
            {
                Debugging.Log("Create team", $"Error: {e.Message}");
                return false;
            }
        }

    }

}
