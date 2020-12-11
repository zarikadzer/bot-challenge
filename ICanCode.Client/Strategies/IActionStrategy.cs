using ICanCode.Api;

namespace ICanCode.Client.Strategies
{
	public interface IActionStrategy
	{
		Bot Bot { get; set; }
		Command GetCommand(Board board);
	}
}