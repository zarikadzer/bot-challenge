using System;
using System.Threading;

namespace ICanCode.Client
{
	class Program
	{
		// you can get this code after registration on the server with your email
		static string ServerUrl = "https://epam-botchallenge.com/codenjoy-contest/board/player/xd5ictl4ease9ned25m3?code=8202341988748313934";
		static BotMode Mode = BotMode.DoNothing;

		static void Main(string[] args) {
			Console.SetWindowSize(Console.LargestWindowWidth - 3, Console.LargestWindowHeight - 3);
			var bot = new Bot(ServerUrl) {
				Mode = Mode
			};
			(new Thread(bot.Play)).Start();
			Console.ReadKey();
			bot.InitiateExit();
		}
	}
}
