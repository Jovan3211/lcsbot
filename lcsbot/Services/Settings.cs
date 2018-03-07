using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Discord;
using System.IO;
using Discord.Commands;
using Discord.WebSocket;

namespace lcsbot.Services
{

    public class SettingsMiddleMan
    {
        public string ClientId { get; set; }
        public string BotToken { get; set; }
        public IList<string> OwnerIds { get; set; }
        public bool EnableDatabase { get; set; }
        public string BaseHostUrl { get; set; }
        public string SqlServerUrl { get; set; }
        public string DatabaseName { get; set; }
        public string RiotAPIKey { get; set; }
        public string ChampionGGAPIKey { get; set; }
    }

    public static class Settings
    {
        public static DiscordSocketClient _client;
        public static CommandService _commands;
        public static IServiceProvider _services;
        public static DiscordSocketConfig _config = new DiscordSocketConfig { MessageCacheSize = 100 };

        private static string clientId;
        private static string botToken;
        private static IList<string> ownerIds;
        private static bool enableDatabase;
        private static string baseHostUrl;
        private static string databaseName;
        private static string sqlServerUrl;
        private static string riotAPIKey;
        private static string championGGAPIKey;

        private static string configPath = "configuration.json";

        private static string ReadFile()
        {
            return File.ReadAllText(configPath);
        }

        /// <summary>
        /// Returns the client ID as string.
        /// </summary>
        public static string ClientId { get => clientId; }
        /// <summary>
        /// Returns the bot token as string.
        /// </summary>
        public static string BotToken { get => botToken; }
        /// <summary>
        /// Returns a list of owner ids as strings.
        /// </summary>
        public static IList<string> OwnerIds { get => ownerIds; }
        /// <summary>
        /// Returns the url for tenebots web host.
        /// </summary>
        public static string BaseHostUrl { get => baseHostUrl; set => baseHostUrl = value; }
        /// <summary>
        /// Returns the url for the sql server.
        /// </summary>
        public static string SqlServerUrl { get => sqlServerUrl; set => sqlServerUrl = value; }
        /// <summary>
        /// Name of the database.
        /// </summary>
        public static string DatabaseName { get => databaseName; set => databaseName = value; }
        /// <summary>
        /// Returns true or false depending on if the database is enabled in settings.
        /// </summary>
        public static bool EnableDatabase { get => enableDatabase; set => enableDatabase = value; }
        /// <summary>
        /// Returns the Riot API key.
        /// </summary>
        public static string RiotAPIKey { get => riotAPIKey; set => riotAPIKey = value; }
        /// <summary>
        /// Returns the Champion.gg API key.
        /// </summary>
        public static string ChampionGGAPIKey { get => championGGAPIKey; set => championGGAPIKey = value; }
        /// <summary>
        /// Loads the settings from a json file and stores it in the settings class variable.
        /// </summary>
        /// <returns>Returns bool if it loaded or failed to load.</returns>
        public static bool LoadJson()
        {
            Debugging.Log("Settings", $"Loading configuration.json");

            try
            {
                SettingsMiddleMan middleMan = JsonConvert.DeserializeObject<SettingsMiddleMan>(ReadFile());

                clientId = middleMan.ClientId;
                botToken = middleMan.BotToken;
                ownerIds = middleMan.OwnerIds;
                enableDatabase = middleMan.EnableDatabase;
                baseHostUrl = middleMan.BaseHostUrl;
                sqlServerUrl = middleMan.SqlServerUrl;
                databaseName = middleMan.DatabaseName;
                riotAPIKey = middleMan.RiotAPIKey;
                championGGAPIKey = middleMan.ChampionGGAPIKey;

                Debugging.Log("Settings", $"Loaded configuration.json successfully");
                return true;
            }
            catch (Exception e)
            {
                Debugging.Log(new LogMessage(LogSeverity.Critical, "Settings", $"Exception while trying to load settings from configuration", e));
                return false;
            }
        }

        /// <summary>
        /// Sets the connection string for logging in to the server.
        /// </summary>
        /// <returns>Returns bool if it successful or not.</returns>
        public static bool SqlServerSetup()
        {
            if (!enableDatabase)
            {
                Debugging.Log("SQL Server Setup", $"Database disabled in configuration. Some features which use a mysql database won't be available", Discord.LogSeverity.Warning);
                return false;
            }

            while (true)
            {
                Debugging.Log("SQL Server Setup", "Please enter login info");

                string port;
                while (true)
                {
                    port = Debugging.Read("Port");
                    if (int.TryParse(port, out int n))
                        break;
                    else
                        Debugging.Log("SQL Server Setup", "Port must be numeric", LogSeverity.Error);
                }

                string username = Debugging.Read("Username");
                string password = Debugging.Read("Password");
                Console.Clear();

                SqlHandler.SetConnectionString(sqlServerUrl, port, username, password);

                if (SqlHandler.TestDatabase())
                    return true;
                else
                {
                    if (Debugging.Read("Credentials incorrect or database unreachable, retry? (y/n)").ToLower() != "y")
                        break;

                }
            }

            Debugging.Log("TestDatabase", $"Some features which use a mysql database won't be available", Discord.LogSeverity.Warning);
            return false;
        }
    }
}
