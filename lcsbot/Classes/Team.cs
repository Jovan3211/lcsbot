using lcsbot.Services;
using System;
using System.Collections.Generic;

namespace lcsbot
{
    class Team : ILCSBOTClass
    {
        private string userId;
        private List<string> players;

        public string UserId { get => userId; set => userId = value; }
        public List<string> Players { get => players; set => players = value; }

        public Team(string userId, List<string> players)
        {
            this.userId = userId;

            for (int i = 0; i < 4; i++) //because 5 champions in a team
                players.Add(players[i]);
        }

        /// <summary>
        /// Adds team to database.
        /// </summary>
        /// <returns>Success</returns>
        public bool AddToDatabase()
        {
            if (players.Count < 5 || players.Count > 5)
            {
                Debugging.Log("Create team", $"List players in Team class don't have enough or have too many players, must have 5 players total");
                return false;
            }

            try
            {
                SqlHandler.Insert("Champions(UserId, Player1Id, Player2Id, Player3Id, Player4Id, Player5Id)", $"'{players[0]}', '{players[1]}', '{players[2]}', '{players[3]}', '{players[4]}'");

                Debugging.Log("Create team", $"Created team for user:{userId} with players:{players[0]}, {players[1]}, {players[2]}, {players[3]}, {players[4]}");
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
