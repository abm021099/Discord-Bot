using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneBot
{
    class MyBot
    {
        DiscordClient client;
        CommandService commands;

        Random randy;

        string[] dankestMemes;

        public MyBot()
        {
            randy = new Random();
            dankestMemes = new string[]
            {
                "memes/mem1.jpg",
                "memes/mem2.png",
                "memes/mem3.jpeg",
                "memes/mem4.png",
                "memes/mem5.jpg",
                "memes/mem6.jpg",
                "memes/mem7.jpg",
                "memes/mem8.png",
                "memes/mem9.jpg",
                "memes/mem10.gif",
                "memes/mem11.jpg",
                "memes/mem12.jpg",
                "memes/mem13.jpg",
                "memes/mem14.png",
                "memes/mem15.png",
                "memes/mem16.png",
                "memes/mem17.gif",
                "memes/mem18.png"
            };


            client = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });
            
            client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Private; //This is for the !help command. Private = Help menu PM'd to user, public = help menu displayed in channel.
            });

            commands = client.GetService<CommandService>();



            //Announcements
            client.UserJoined += async (s, e) =>                                    //Welcomes new users
            {
                var channel = e.Server.FindChannels(Convert.ToString(e.Server.DefaultChannel), ChannelType.Text).FirstOrDefault();
                var user = e.User;
                await channel.SendMessage(string.Format("{0} has joined the server!", user.Mention));
            };

            client.ChannelCreated += async (s, e) =>                               //Announces when a new chanel has been created
            {
               await e.Channel.SendMessage("A new channel has been created.");
            };

            client.JoinedServer += async (s, e) =>                                //Announces when CloneBot joins a new server
            {
                await e.Server.DefaultChannel.SendMessage("Hello @everyone! I'm CloneBot. To learn more, type !help");
            };

            client.UserBanned += async (s, e) =>                                 //Announces when a user has been banned from the server.   
            {
                var channel = e.Server.DefaultChannel;
                var user = e.User;
                await channel.SendMessage(string.Format("{0} has been banned from the server.", user.Mention));
            };
            client.UserLeft += async (s, e) =>                                  //Announces when a user has permanently left the server (not just disconnect)
            {
                var channel = e.Server.DefaultChannel;
                var user = e.User;
                await channel.SendMessage(string.Format("{0} has left the server.", user.Mention));
            };
            

            //Call Functions
            RegisterMemeCommand();
            RegisterPurgeCommand();
            InstantInvite();

            client.ExecuteAndWait(async () =>
            {
                await client.Connect("Put Your Token Here", TokenType.Bot);
            });
        }



//*****************
//*Command Section*
//*****************


        //Meme Command
        private void RegisterMemeCommand()
        {
            commands.CreateCommand("meme")
                .Do(async (e) =>
                {
                    int randomMemeIndex = randy.Next(dankestMemes.Length);
                    string memeToPost = dankestMemes[randomMemeIndex];
                   await e.Channel.SendFile(memeToPost);
                });
        }

        //Purge command
        private void RegisterPurgeCommand()
        {
            commands.CreateCommand("purge")
                .Description("Deletes up to 100 messages. Use: !purge")
                .Do(async (e) =>
                {
                    if (e.User.ServerPermissions.Administrator == true)
                    {
                        Message[] messagesToDelete;
                        messagesToDelete = await e.Channel.DownloadMessages(100);

                        await e.Channel.DeleteMessages(messagesToDelete);
                    }else
                    {
                        await e.Channel.SendMessage("Sorry, you're not an admin!");
                        
                    }
                });
        }
        
        //InstantInvite link
        private void InstantInvite()
        {
            commands.CreateCommand("invite")
                .Description("Creates an instant invite link. Use: !invite")
                .Do(async (e) =>
                {
                    if (e.User.ServerPermissions.CreateInstantInvite == true)
                    {
                        var invite = await e.Server.CreateInvite(maxAge: null, maxUses: null, tempMembership: false, withXkcd: true);
                        await e.Channel.SendMessage(Convert.ToString(e.User.Mention +" Here is your instant invite link: https://discord.gg/" + invite));
                    }
                });
        }

        







       
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
