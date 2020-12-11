using ICanCode.Api;

namespace ICanCode.Client.Strategies
{
	public class GoToExitStrategy : BaseActionStrategy
	{
		public GoToExitStrategy(Bot bot) : base(bot) { }
		public override Command GetCommand(Board board) => Command.GoToExit();
	}
}