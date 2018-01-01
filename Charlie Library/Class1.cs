using AIMLbot;

namespace Charlie_Library
{

    class BotInit
    {
        //Please refer the AIML documentation
        private Bot AimlBot; //Bot object
        private User user; //username for the bot initalization

        public BotInit(string _userId)
        {
            AimlBot = new Bot();
            user = new User(_userId, AimlBot);
            Init();
        }

        public void Init()
        {

            //The AIML files are in the same folder as the dll file
            AimlBot.loadSettings();
            AimlBot.isAcceptingUserInput = false;
            AimlBot.loadAIMLFromFiles();
            AimlBot.isAcceptingUserInput = true;
        }

        //Return a response from the AIML bot for the input string
        public string GetOutput(string input)
        {
            Request request = new Request(input, user, AimlBot);
            Result result = AimlBot.Chat(request);
            return result.Output;
        }

    }

    //class that handles all the interactions with the bot. Currently only sending and recieving chats
    public class Charlie
    {
        static string response;
        private BotInit bot;

        //Creates a new AIML bot for each user ID
        public Charlie(string user)
        {
            bot = new BotInit(user);
        }

        //Sends and recieves messages to the bot. Accepts input as string parameter and returns response as string 
        public string talkToBot(string input)
        {
            response = bot.GetOutput(input);
            return response;
        }
    }
}
