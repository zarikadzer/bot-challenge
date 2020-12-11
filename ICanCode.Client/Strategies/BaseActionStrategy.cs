using ICanCode.Api;

namespace ICanCode.Client.Strategies
{
	public abstract class BaseActionStrategy : IActionStrategy
	{
		public Bot Bot { get; set; }

		public BaseActionStrategy(Bot bot) => Bot = bot;

		public int TryIncrementStrategyStep() {
			var type = GetType().Name;
			if (!Bot.Memory.ContainsKey(type)) {
				Bot.Memory.Add(type, 0);
			}
			return ++Bot.Memory[type];
		}

		public int TryDecrementStrategyStep() {
			var type = GetType().Name;
			if (!Bot.Memory.ContainsKey(type)) {
				Bot.Memory.Add(type, 1);
			}
			return --Bot.Memory[type];
		}

		public void ResetStrategyStep() {
			Bot.Memory[GetType().Name] = 0;
		}

		public abstract Command GetCommand(Board board);
	}
}
