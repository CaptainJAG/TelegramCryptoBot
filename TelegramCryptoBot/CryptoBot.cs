using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using CoinMarketCap;

namespace TelegramCryptoBot
{
    class CryptoBot
    {
        // Creates Bot instance to authenticate.
        private static readonly TelegramBotClient Bot = new TelegramBotClient("<BotAPIKey>");

        // List is currencies.
        private static readonly string[] _currency = { "BITCOIN", "LITECOIN", "ETHEREUM", "RIPPLE", "BITCOIN-CASH" };

        // List of commands for last hour.
        private static readonly string[] _priceChangeLastHourCommand = { "BCHANGE", "LCHANGE", "ECHANGE", "RCHANGE", "BCHCHANGE" };

        // List of commands for past day.
        private static readonly string[] _priceChangeLastDayCommand = { "BDAY", "LDAY", "EDAY", "RDAY", "BCHDAY" };

        // List of other commands.
        private static readonly string[] _otherCommands = { "MOON" };

        // Main method where execution happens.
        static void Main(string[] args)
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnMessageEdited += Bot_OnMessage;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        /// <summary>
        /// Method that controls what the bot says back to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message from users</param>
        public static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                // Loops through all arrays to find command.
                for (int i = 0; i < _currency.Length; i++)
                {
                    // Current price.
                    if (e.Message.Text.ToUpper().Trim() == "/" + _currency[i])
                    {
                        CurrentPrice(_currency[i].ToLower().ToString(), e);
                    }
                    // Price change last hour.
                    else if (e.Message.Text.ToUpper().Trim() == "/" + _priceChangeLastHourCommand[i])
                    {
                        PriceChangeLastHour(_currency[i].ToLower().ToString(), e);
                    }
                    // Price change last day.
                    else if (e.Message.Text.ToUpper().Trim() == "/" + _priceChangeLastDayCommand[i])
                    {
                        PriceChangeLastDay(_currency[i].ToLower().ToString(), e);
                    }
                }

                // Hard coded moon command.
                if (e.Message.Text.ToUpper().ToString() == "/MOON")
                {
                    Random random = new Random();
                    int randomNumber = random.Next(20000, 100000);
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Bitcoin will hit $" + randomNumber + " soon!");
                }

                // Loops through all arrays to send command list.
                else if (e.Message.Text.ToUpper().ToString() == "/COMMANDS")
                {
                    StringBuilder response = new StringBuilder();
                    response.Append(@"Current commands are:");
                    response.AppendLine();
                    for (int j = 0; j < _currency.Length; j++)
                    {
                        response.Append("/" + _currency[j]);
                        response.AppendLine();
                    }
                    for (int j = 0; j < _priceChangeLastHourCommand.Length; j++)
                    {
                        response.Append("/" + _priceChangeLastHourCommand[j]);
                        response.AppendLine();
                    }
                    for (int j = 0; j < _priceChangeLastDayCommand.Length; j++)
                    {
                        response.Append("/" + _priceChangeLastDayCommand[j]);
                        response.AppendLine();
                    }
                    for (int j = 0; j < _otherCommands.Length; j++)
                    {
                        response.Append("/" + _otherCommands[j]);
                        response.AppendLine();
                    }
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, response.ToString());
                }
            }
        }

        /// <summary>
        /// Gets the current price and sends a message back to user with price.
        /// </summary>
        /// <param name="currency">Cryptocurrency name</param>
        /// <param name="e">Message from users</param>
        public static void CurrentPrice(string currency, MessageEventArgs e)
        {
            var response = CoinMarketCapPrice(currency).Result;
            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Current price of " + currency.ToUpper() + ": $" + response);
        }

        /// <summary>
        /// Gets the price change the last hour and sends message back to user.
        /// </summary>
        /// <param name="currency">Cryptocurrency name</param>
        /// <param name="e">Message from users</param>
        public static void PriceChangeLastHour(string currency, MessageEventArgs e)
        {
            var response = CoinMarketCapPriceLastHour(currency).Result;
            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Price change in the last hour for " + currency.ToUpper() + ": " + response + "%");
        }

        /// <summary>
        /// Gets the price change the last day and sends message back to user.
        /// </summary>
        /// <param name="currency">Cryptocurrency name</param>
        /// <param name="e">Message from users</param>
        public static void PriceChangeLastDay(string currency, MessageEventArgs e)
        {
            var response = CoinMarketCapPriceLastDay(currency).Result;
            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Price change in the last day for " + currency.ToUpper() + ": " + response + "%");
        }

        /// <summary>
        /// Gets the price of the currency that you supply.
        /// </summary>
        /// <param name="currency">Cryptocurrency name</param>
        /// <returns></returns>
        public static async Task<string> CoinMarketCapPrice(string currency)
        {
            // Get instance
            var client = new CoinMarketCapClient();

            // Get current price.
            var ticker = await client.GetTickerAsync(currency);
            return ticker.PriceUsd.Value.ToString();
        }

        /// <summary>
        /// Gets price change last 1 hour.
        /// </summary>
        /// <param name="currency">Cryptocurrency name</param>
        /// <returns></returns>
        public static async Task<string> CoinMarketCapPriceLastHour(string currency)
        {
            // Get instance
            var client = new CoinMarketCapClient();

            // Get price change last hour.
            var ticker = await client.GetTickerAsync(currency);
            return ticker.PercentChange1h.Value.ToString();
        }

        /// <summary>
        /// Gets price change last 24 hour.
        /// </summary>
        /// <param name="currency">Cryptocurrency name</param>
        /// <returns></returns>
        public static async Task<string> CoinMarketCapPriceLastDay(string currency)
        {
            // Get instance
            var client = new CoinMarketCapClient();

            // Get price change last day.
            var ticker = await client.GetTickerAsync(currency);
            return ticker.PercentChange24h.Value.ToString();
        }
    }
}
