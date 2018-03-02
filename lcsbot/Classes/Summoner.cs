using System;
using lcsbot.Services;
using lcsbot.Riot;
using RiotSharp;
using RiotSharp.MatchEndpoint;

namespace lcsbot.Classes
{
    public class Summoner
    {
        private string summonerId;
        private string championId;
        private Role role;
        private Lane lane;
            
        public string SummonerId { get => summonerId; }
        public string ChampionId { get => championId; }
        public Role Role { get => role; }
        public Lane Lane { get => lane; }

        public Summoner(string summonerId, string championId, Lane lane, Role role)
        {
            this.summonerId = summonerId;
            this.championId = championId;
            this.lane = lane;
            this.role = role;
        }

        public int AddToDatabase()
        {
            try
            {
                SqlHandler.Insert("Summoners(SummonerId, ChampionId, Role)", $"'{summonerId}', '{championId}', '{(int)role}'");
                var selection = SqlHandler.Select("Summoners", "Id", $"SummonerId='{summonerId}' AND ChampionId='{championId}' AND Role='{(int)role}'");

                Debugging.Log("Create summoner", $"Created new summoner and added to database, id={selection[0]}");
                return int.Parse(selection[0]);
            }
            catch (Exception e)
            {
                Debugging.Log("Create summoner", $"Error: {e.Message}");
                return -1;
            }
        }
    }
}