using ICanCode.Api;

namespace ICanCode.Client.Strategies
{
	public class DoNothingStrategy : BaseActionStrategy
	{
		public DoNothingStrategy(Bot bot) : base(bot) { }
		public override Command GetCommand(Board board) => Command.DoNothing();
	}
}