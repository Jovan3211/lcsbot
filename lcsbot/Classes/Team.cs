using lcsbot.Services;
using System;
using System.Collections.Generic;

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
            this.userId = userId;
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
                int counter = 0;
                foreach (Summoner summoner in summoners)
                {
                    summonerIds[counter] = summoner.AddToDatabase();
                    counter++;
                }

                SqlHandler.Insert("Champions(UserId, Summoner1Id, Summoner2Id, Summoner3Id, Summoner4Id, Summoner5Id)", $"'{summonerIds[0]}', '{summonerIds[1]}', '{summonerIds[2]}', '{summonerIds[3]}', '{summonerIds[4]}'");

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
