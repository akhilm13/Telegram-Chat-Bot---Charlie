using Charlie_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Telegram.Bot;

namespace Telegram_Chat_Bot___Charlie
{
    class Program
    {
        //Creating a BotClient object using Bot API Key obtained from the Telegram BotFather
        private static TelegramBotClient botClient = new TelegramBotClient("Insert your Telegram Bot API Key here.");
        //List of all chat Ids that have already staretd a chat with the bot
        private static List<Telegram.Bot.Types.ChatId> chatIdsList = new List<Telegram.Bot.Types.ChatId>();
        //List of bot instances for each chat Id in the corresponding indices of the chat Ids list 
        private static List<BotInstance> botInstancesList = new List<BotInstance>();

        static void Main(string[] args)
        {
            //Add event handler for function to handle incoming messages to the bot
            botClient.OnMessage += BotClient_OnMessage;

            //start receiving input 
            botClient.StartReceiving();

            //ReadLine function to contine receiving 
            Console.ReadLine();

            botClient.StopReceiving();

        }

        //Callback function to handle the event OnMessage 
        private static void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            //If chat ID already exists in the list of chat Ids
            if (chatIdsList.Contains(e.Message.Chat.Id))
            {
                //Get the corresponding BotInstance object from the list of instances 
                BotInstance botInstance = botInstancesList.ElementAt(chatIdsList.IndexOf(e.Message.Chat.Id));
                //Talk with the bot on new thread
                new Thread(botInstance.startRecieveMessages).Start();
            }
            else
            {
                //Add the new chat Id to the list
                chatIdsList.Add(e.Message.Chat.Id);
                //Create a new BotInstance object and add it to the list
                BotInstance botInstance = new BotInstance(botClient, e);
                botInstancesList.Add(botInstance);
                //Talk to the bot using a new thread
                new Thread(botInstance.startRecieveMessages).Start();
            }


        }


    }

    //Each user ID is assigned an instance of this class in order to have a unque AIML bot for each user.
    public class BotInstance
    {
        TelegramBotClient botClient;
        Telegram.Bot.Args.MessageEventArgs eventVariable;
        Charlie botI;
        

        public BotInstance(TelegramBotClient client, Telegram.Bot.Args.MessageEventArgs e)
        {
            botClient = client;
            this.eventVariable = e;
            botI = new Charlie(e.Message.Chat.Id.ToString());
        }

        public void startRecieveMessages()
        {
            string input = eventVariable.Message.Text; //Recieved message
            string response = "";
            Console.Write("Message recieved: " + input);

            //Ignore when a new user subscribes to the bot
            if (input.Equals("/start"))
            {
                return;
            }
            //remove the leading slash('/') to avoid when message sent from groups
            else if (input.StartsWith("/"))
                input = input.Remove(0, 1);


            try
            {
                response = botI.talkToBot(eventVariable.Message.Text);
            }
            catch (Exception e)
            {
                //if FileNotFoundException is thrown, please check the location of the AIML template files.
                Console.WriteLine("The type init failed" + e.Message);
            }

            //Sending a reply to the user from the bot
            botClient.SendTextMessageAsync(eventVariable.Message.Chat.Id, response);


            //Logging the incoming messages. Modify and uncomment if required
            /*
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("Log.txt", true))
            {
                //Logs First name and message (only incoming)
                 file.WriteLine(e.Message.Chat.FirstName + " : " + input);
            }
            */

        }

    }

}
