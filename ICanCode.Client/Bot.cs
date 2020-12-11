using ICanCode.Api;
using ICanCode.Client.Strategies;
using System.Collections.Generic;

namespace ICanCode.Client
{
	public enum BotMode
	{
		DoNothing = 0,
		TryUseOnlyStartPoint = 1,
		GoToExit = 2
	}

	public class Bot : AbstractSolver
	{
		private Dictionary<BotMode, IActionStrategy> Strategies;

		public Bot(string server)
			: base(server) {
			Strategies = new Dictionary<BotMode, IActionStrategy> {
				{BotMode.DoNothing, new DoNothingStrategy(this) },
				{BotMode.TryUseOnlyStartPoint, new StartPointStrategy(this) },
				{BotMode.GoToExit, new GoToExitStrategy(this) },
			};
		}

		public BotMode Mode { get; set; } = BotMode.DoNothing;
		public Dictionary<string, int> Memory { get; } = new Dictionary<string, int>();
		public Direction? PreviousBotDirection { get; set; } = null;
		public override Command WhatToDo(Board board) => Strategies[Mode].GetCommand(board);
	}
}
