# TelegramCryptoBot
This telegram bot gets price updates from the Coin Market Cap API.

You will need to get a bot API key from BotFather on Telegram and insert at top of CryptoBot.cs.

This bot uses the C# wrapper for the Coin Market Cap API from this repository: https://github.com/binamonk/CoinMarketCapClient

To add additional assets just add the appropriate name for the asset from the Coin Market Cap API documentation here: https://coinmarketcap.com/api/.  The three arrays(_currency, _priceChangeLastHourCommand, _priceChangeLastDayCommand) must match indexes for it to select the proper command.

I have the program running in an App Service in Azure to handle requests, but you can also run it locally and it will work.
