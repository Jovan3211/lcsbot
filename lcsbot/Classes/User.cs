using lcsbot.Services;
using System;

namespace lcsbot.Classes
{
    public class User : ILCSBOTClass
    {
        private string userId;
        private string username;
        private Team team;

        public string UserId { get => userId; set => userId = value; }
        public string Username { get => username; set => username = value; }
        public Team Team { get => team; }

        public User(string userId, string username)
        {
            this.userId = userId;
            this.username = username;
            team = new Team(userId);
        }

        private bool CheckTeamReady() => team.CheckReady();

        public bool AddSummonerToTeam(Summoner summoner) => team.AddSummoner(summoner);

        public bool RemoveSummonerFromTeam(Summoner summoner) => team.RemoveSummoner(summoner);

        public bool SaveTeam() => team.AddToDatabase();

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
