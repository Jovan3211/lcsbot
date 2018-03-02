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

        public Summoner(string summonerId, string championId, Role role, Lane lane)
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
                SqlHandler.Insert("Summoners(SummonerId, ChampionId, Role, Lane)", $"'{summonerId}', '{championId}', '{(int)role}', '{(int)lane}'");
                var selection = SqlHandler.Select("Summoners", "Id", $"SummonerId='{summonerId}' AND ChampionId='{championId}' AND Role='{(int)role}' AND Lane='{(int)lane}'");

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