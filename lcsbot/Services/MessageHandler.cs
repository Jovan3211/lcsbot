using Discord;
using Discord.Commands;

namespace lcsbot.Services
{
    public class MessageHandler : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Builds an embed to be used in sending messages to discord.
        /// </summary>
        /// <param name="title">Message title.</param>
        /// <param name="description">Message description.</param>
        /// <returns>Embed to be used in async calls</returns>
        public EmbedBuilder BuildEmbed(string title, string description)
        {
            Color color = Palette.Pink;

            EmbedBuilder message = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color);

            return message;
        }

        /// <summary>
        /// Builds an embed to be used in sending messages to discord.
        /// </summary>
        /// <param name="title">Message title.</param>
        /// <param name="description">Message description.</param>
        /// <param name="color">Message color.</param>
        /// <returns>Embed to be used in async calls</returns>
        public EmbedBuilder BuildEmbed(string title, string description, Color color)
        {
            EmbedBuilder message = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color);

            return message;
        }

    }
}
