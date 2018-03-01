using lcsbot.Services;
using System;

namespace lcsbot
{
    class User : ILCSBOTClass
    {
        private string userId;
        private string username;

        public string UserId { get => userId; set => userId = value; }
        public string Username { get => username; set => username = value; }

        public User(string userId, string username)
        {
            this.userId = userId;
            this.username = username;
        }

        /// <summary>
        /// Adds user to database.
        /// </summary>
        /// <returns>Success</returns>
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
