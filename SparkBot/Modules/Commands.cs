using Discord;
using Discord.Commands;
using SparkBot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SparkBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public MessageService MessageService { get; set; }


        [Command("help")]
        public async Task HelpAsync([Optional] string command)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.AddField("Commands", "ping, roll, tag, addtag, tags, rps")                
                .WithColor(Color.DarkMagenta);

            if (command == null)
                await ReplyAsync($"Hello, I am {Context.Client.CurrentUser.Username}.\nTyping `s!help <command name>` will give you more information on how to use the specific command.\nHere is a list of my commands:", false, builder.Build());
            else
            {
                command = command.ToLower();
                if (command == "ping")
                    await ReplyAsync("Calculates your latency to discord servers. Correct usage: `s!ping`");
                else if (command == "roll")
                    await ReplyAsync("Rolls an n-sided die. Correct usage: `s!roll <number of faces on die>`");
                else if (command == "tag")
                    await ReplyAsync("Tags the relevant memes, videos, copypastas, etc. A full list of tags are available with `s!tags`. Correct usage: `s!tag <tag>`");
                else if (command == "rps")
                    await ReplyAsync("Play a game of rock paper scissors. Correct usage: `s!rps <choice>`");
                else await ReplyAsync("This command does not exist.");
            }
        }

        #region Commands

        [Command("ping")]
        public async Task PingAsync([Optional] [Remainder] string useless)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var msg = await ReplyAsync("Calculating...");
            watch.Stop();
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(":ping_pong: Pong!")
                .WithDescription($"Time elapsed: {watch.ElapsedMilliseconds} ms")
                .WithColor(Color.DarkMagenta);
            await msg.DeleteAsync();
            await ReplyAsync("", false, builder.Build());
        }

        [Command("roll")]
        public async Task RollAsync([Optional] int x, [Optional] [Remainder] string useless)
        {
            if (x != 0)
            {
                Random rnd = new Random();
                int roll = rnd.Next(1, x + 1);
                await ReplyAsync($":game_die: {Context.User.Mention}, you rolled {roll}.");
            }
            else await ReplyAsync("How many sides should the dice have? Please use the command like: s!roll number");
        }

        [Command("say"), RequireOwner]
        public async Task SayAsync([Remainder] string phrase)
        {
            if (phrase != null)
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync(phrase);
            }
        }

        [Command("addtag")]
        public async Task AddTagAsync(string tagName, [Remainder] string tag)
        {
            bool tagExists = false;
            string[] tagArray = File.ReadAllLines("Tags.txt");
            for (int i = 0; i < tagArray.Length; i++)
            {
                if (tagArray[i].Split(' ').First() == tagName)
                {
                    tagExists = true;
                    break;
                }
            }

            if (!tagExists)
            {
                File.AppendAllText("Tags.txt", $"{tagName} {tag}\n");
                await ReplyAsync($"The tag {tagName} has been successfully created.");
            }
            else await ReplyAsync($"The tag {tagName} already exists.");
        }

        [Command("tag")]
        public async Task TagAsync(string tag, [Optional] string useless)
        {
            string[] tagArray = File.ReadAllLines("Tags.txt");
            bool tagExists = false;
            for (int i = 0; i < tagArray.Length; i++)
            {
                if (tagArray[i].Split(' ').First() == tag)
                {
                    await ReplyAsync(tagArray[i].Substring(tagArray[i].Split(' ').First().Length));
                    tagExists = true;
                }
            }
            if (!tagExists)
                await ReplyAsync($"The tag {tag} does not exist.");
        }

        [Command("tags")]
        public async Task TagsAsync([Optional] [Remainder] string useless)
        {
            string tags = "";

            string[] tagArray = File.ReadAllLines("Tags.txt");

            for (int i = 0; i < tagArray.Length; i++)
            {
                tags += tagArray[i].Split(' ').First() + ", ";
            }

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"Tags - {tagArray.Length} tags")
                .WithDescription(tags)
                .WithColor(Color.DarkMagenta);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("rps")]
        public async Task RPSAsync(string choice, [Optional] [Remainder] string useless)
        {
            Random random = new Random();
            int bot = random.Next(3);
                        
            if (choice == "r" && bot == 0)
                await ReplyAsync($"{Context.User.Username} picked rock and I picked rock. Draw.");
            else if (choice == "r" && bot == 1)
                await ReplyAsync($"{Context.User.Username} picked rock and I picked paper. Hold this L.");
            else if (choice == "r" && bot == 2)
                await ReplyAsync($"{Context.User.Username} picked rock and I picked scissors. {Context.User.Username} wins.");
            else if (choice == "p" && bot == 0)
                await ReplyAsync($"{Context.User.Username} picked paper and I picked rock. {Context.User.Username} wins.");
            else if (choice == "p" && bot == 1)
                await ReplyAsync($"{Context.User.Username} picked paper and I picked paper. Draw.");
            else if (choice == "p" && bot == 2)
                await ReplyAsync($"{Context.User.Username} picked paper and I picked scissors. Hold this L.");
            else if (choice == "s" && bot == 0)
                await ReplyAsync($"{Context.User.Username} picked scissors and I picked rock. Hold this L.");
            else if (choice == "s" && bot == 1)
                await ReplyAsync($"{Context.User.Username} picked scissors and I picked paper. {Context.User.Username} wins.");
            else if (choice == "s" && bot == 2)
                await ReplyAsync($"{Context.User.Username} picked scissors and I picked scissors. Draw.");
            else
                await ReplyAsync("Please enter your choice as `r`, `p` or `s`.");
        }

        [Command("spam")]
        public async Task SpamAsync(IGuildUser user, int n)
        {
            if (n > 25) n = 25;
            for (int i = 1; i <= n; i++)
                await ReplyAsync(user.Mention);
        }
        [Command("spam")]
        public async Task SpamAsync([Remainder]string phrase)
        {
            bool success = int.TryParse(phrase.Split(' ').Last(), out int spamCount);
            if (success)
            {
                if (spamCount > 25)
                    spamCount = 25;
                else if (spamCount <= 0)
                    spamCount = 5;

                if (1 <= spamCount && spamCount <= 9)
                    phrase = phrase.Substring(0, phrase.Length - 1);
                else if (10 <= spamCount && spamCount <= 25)
                    phrase = phrase.Substring(0, phrase.Length - 2);

                for (int i = 1; i <= spamCount; i++)
                    await ReplyAsync(phrase, true);
            }
            else await ReplyAsync("If you want me to spam something do `s!spam <message> <number of spams>`.");
        }

        #endregion

    }
}
