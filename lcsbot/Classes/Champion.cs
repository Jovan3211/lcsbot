using lcsbot.Services;
using System;

namespace lcsbot
{
    class Champion : ILCSBOTClass
    {
        private string playerId;
        private string championId;

        public string PlayerId { get => playerId; set => playerId = value; }
        public string ChampionId { get => championId; set => championId = value; }

        public Champion(string userId, string championId)
        {
            this.playerId = userId;
            this.championId = championId;
        }

        /// <summary>
        /// Adds champion to database.
        /// </summary>
        /// <returns>Success</returns>
        public bool AddToDatabase()
        {
            try
            {
                SqlHandler.Insert("Champions(ChampionId, PlayerId)", $"'{championId}', '{playerId}'");

                Debugging.Log("Add champion to player", $"Added champion:{ChampionId} to player:{playerId}");
                return true;
            }
            catch (Exception e)
            {
                Debugging.Log("Add champion to playere", $"Error: {e.Message}");
                return false;
            }
        }
    }
}
