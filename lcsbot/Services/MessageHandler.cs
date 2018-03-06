using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;

namespace lcsbot.Services
{
    public class MessageHandler : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Builds a ready made embed for debug sending a premade message to discord.
        /// </summary>
        /// <returns>Embed to be used in async calls</returns>
        public EmbedBuilder GetDebugEmbed()
        {
            Color color = Palette.Pink;

            EmbedBuilder message = new EmbedBuilder()
                .WithTitle("Horry Shiieet")
                .WithDescription("OH MAI GOD! Whatever you were testing worked, finally.")
                .WithColor(color)
                .WithThumbnailUrl(ImageHandler.GetImageUrl("joseph"));

            return message;
        }

        /// <summary>
        /// Builds an embed to be used in sending messages to discord.
        /// </summary>
        /// <param name="title">Message title.</param>
        /// <param name="description">Message description.</param>
        /// <param name="thumbnailUrl">Optional thumbnail picture to be added to the message.</param>
        /// <returns>Embed to be used in async calls</returns>
        public EmbedBuilder BuildEmbed(string title, string description, string thumbnailUrl = "")
        {
            Color color = Palette.Pink;

            EmbedBuilder message = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color);

            if (thumbnailUrl != "")
                message.WithThumbnailUrl(thumbnailUrl);

            return message;
        }

        /// <summary>
        /// Builds an embed to be used in sending messages to discord.
        /// </summary>
        /// <param name="title">Message title.</param>
        /// <param name="description">Message description.</param>
        /// <param name="color">Message color.</param>
        /// <param name="thumbnailUrl">Optional thumbnail picture to be added to the message.</param>
        /// <returns>Embed to be used in async calls</returns>
        public EmbedBuilder BuildEmbed(string title, string description, Color color, string thumbnailUrl = "")
        {
            EmbedBuilder message = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color);

            if (thumbnailUrl != "")
                message.WithThumbnailUrl(thumbnailUrl);

            return message;
        }

        /// <summary>
        /// Builds an embed to be used in sending messages to discord.
        /// </summary>
        /// <param name="title">Message title.</param>
        /// <param name="description">Message description.</param>
        /// <param name="color">Message color.</param>
        /// <param name="fieldTitles">Additional field titles.</param>
        /// <param name="fieldValue">Addidional field values.</param>
        /// <param name="thumbnailUrl">Optional thumbnail picture to be added to the message.</param>
        /// <returns>Embed to be used in async calls</returns>
        public EmbedBuilder BuildEmbed(string title, string description, Color color, List<string> fieldTitles, List<string> fieldValue, string thumbnailUrl = "")
        {
            EmbedBuilder message = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription($"{description}\n────────────")
                .WithColor(color);

            if (thumbnailUrl != "")
                message.WithThumbnailUrl(thumbnailUrl);

            if (fieldTitles.Count < 1)
                throw new Exception("fieldTitles list cannot be empty.");
            if (fieldValue.Count < 1)
                throw new Exception("fieldValue list cannot be empty.");

            int counter = 0;
            foreach (string fieldTitle in fieldTitles)
            {
                if (string.IsNullOrEmpty(fieldValue[counter]))
                {
                    Debugging.Log("BuildEmbed", "No fieldValue provided for fieldTitle, field text cannot be empty", LogSeverity.Warning);
                    message.AddField(fieldTitle, "No value provided");
                }
                else
                    message.AddField(fieldTitle, $"{fieldValue[counter]}\n───");
                counter++;
            }

            return message;
        }
    }
}
