using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SparkBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparkBot.Services
{
    public class MessageService
    {
        private readonly DiscordSocketClient _client;

        public MessageService(DiscordSocketClient client)
        {
            _client = client;
            _client.MessageReceived += React;
        }

        private async Task React(SocketMessage msg)
        {
            //if (msg.Channel.GetType() == typeof(SocketDMChannel))
            //{
            //    await Commands;
            //}
            if (msg.Channel.Name.Contains("suggestion") && msg.Author.IsBot)
            {
                //var emote = (msg.Author as SocketGuildUser).Guild.Emotes.FirstOrDefault(x => x.Name.Contains("theslavsmark"));
                var checkemote = new Emoji("✅");
                var xemote = new Emoji("❌");

                await (msg as SocketUserMessage).AddReactionAsync(checkemote);
                await (msg as SocketUserMessage).AddReactionAsync(xemote);
            }
            else if (msg.Content.Contains(":giggle") || msg.Content.Contains("🤭"))
            {
                var emote = new Emoji("🤢");
                await (msg as SocketUserMessage).AddReactionAsync(emote);
            }
            else if (msg.Author.Id == 349756729396953089/* && message.Content.ToLower().Contains("now playing")*/)
            {
                var upemote = new Emoji("👍");
                var downemote = new Emoji("👎");

                await (msg as SocketUserMessage).AddReactionAsync(upemote);
                await (msg as SocketUserMessage).AddReactionAsync(downemote);
            }
        }
    }
}
