using lcsbot.Services;
using System;
using System.Collections.Generic;

namespace lcsbot.Classes
{
    public class User : ILCSBOTClass
    {
        private string userId;
        private string username;
        private Team team;
        private bool saved;

        public string UserId { get => userId; set => userId = value; }
        public string Username { get => username; set => username = value; }
        public Team Team { get => team; }
        public bool Saved { get => saved; set => saved = value; }

        public User(string userId, string username)
        {
            this.userId = userId;
            this.username = username;
            team = new Team(userId);
            saved = false;
        }

        public bool CheckTeamReady() => team.CheckReady();

        public bool AddSummonerToTeam(Summoner summoner) => team.AddSummoner(summoner);

        public bool RemoveSummonerFromTeam(Summoner summoner) => team.RemoveSummoner(summoner);

        public List<Summoner> ReturnSummonersInTeam() => team.Summoners;

        public void SetTeam(Team team) => this.team = team;

        public bool SaveTeam()
        {
            if (team.AddToDatabase())
                saved = true;

            return saved;
        }

        public bool AddToDatabase()
        {
            try
            {
                SqlHandler.Insert("Users(UserId, Username)", $"'{userId}', '{username}'");

                Debugging.Log("Add user to database", $"Added {username} to users");
                return true;
            }
            catch (Exception e)
            {
                Debugging.Log("Add user to database", $"Error: {e.Message}");
                return false;
            }
        }
    }
}
