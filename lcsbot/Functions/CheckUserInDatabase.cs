using System.Collections.Generic;
using lcsbot.Services;

namespace lcsbot.Functions
{
    public static class CheckUserInDatabase
    {
        /// <summary>
        /// Checks if a user is in the database by id.
        /// </summary>
        /// <param name="userId">Discord UserId.</param>
        /// <returns>If user is in the database or not.</returns>
        public static bool Check(string userId)
        {
            List<string> selection = SqlHandler.Select("Users", "UserId", $"UserId = '{userId}'");

            if (selection.Count < 1)
                return false;
            else
                return true;
        }
    }
}
