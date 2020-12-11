using ICanCode.Api;

namespace ICanCode.Client.Strategies
{
	public class StartPointStrategy : BaseActionStrategy
	{
		public StartPointStrategy(Bot bot) : base(bot) { }

		/// <summary>
		/// 0.!safe point Reset
		/// 1.Safe move from StartPoint with others
		/// 2.Fire
		/// 3.Reset
		/// </summary>
		/// <param name="board"></param>
		/// <returns></returns>
		public override Command GetCommand(Board board) {
			var strategyStep = TryIncrementStrategyStep();
			if (!board.IsMyPointSafe()) {
				ResetStrategyStep();
				return Command.Reset();
			}
			switch (strategyStep) {
				case 1:
					var enemyDirection = GetEnemyDirection(board, 1);
					if (enemyDirection.HasValue) {
						return Command.Fire(enemyDirection.Value);
					}
					break;
				case 2:
					var next = GetSafePointToMove(board);
					if (next == default) {
						TryDecrementStrategyStep();
						var fireDirection = GetFireDirection(board);
						if (fireDirection.HasValue) {
							return Command.Fire(fireDirection.Value);
						}
						break;
					}
					Bot.PreviousBotDirection = next.direction;
					return Command.Go(next.direction);
				case 3:
					var direction = board.GetOppositeDirection(Bot.PreviousBotDirection);
					if (!direction.HasValue) {
						break;
					}
					var myPos = board.GetMe();
					if (!myPos.HasValue) {
						return Command.Fire(direction.Value);
					}
					var target = myPos.Value.Shift(direction);
					var isEnemyStillAtStart = board.IsAllAt(target, Element.ROBO_OTHER, Element.START);
					if (isEnemyStillAtStart) {
						return Command.Fire(direction.GetValueOrDefault());
					} else {
						var fireDirection = GetFireDirection(board);
						if (fireDirection.HasValue) {
							return Command.Fire(fireDirection.Value);
						}
					}
					break;
				default:
					ResetStrategyStep();
					return Command.Reset();
			}
			return GetCommand(board);
		}

		private Direction? GetEnemyDirection(Board board, int steps) {
			var directions = new[] {
				Direction.Left,
				Direction.Up,
				Direction.Right,
				Direction.Down
			};
			foreach(var direction in directions) {
				if (board.IsEnemyAtDirection(direction, steps)) {
					return direction;
				}
			}
			return null;
		}

		private Direction? GetFireDirection(Board board) {
			var directions = new[] {
				Direction.Left,
				Direction.Up,
				Direction.Right,
				Direction.Down
			};
			var pos = board.GetMe();
			if (!pos.HasValue) {
				return Bot.PreviousBotDirection;
			}
			var minDist = 9999;
			Direction? fireDirection = null;
			foreach(var direction in directions) {
				var distance = board.GetNearestEnemyDistanceByDirection(pos.Value, direction);
				if (distance == 0) {
					continue;
				}
				if (distance < minDist) {
					minDist = distance;
					fireDirection = direction;
				}
			}
			return fireDirection.HasValue
				? fireDirection
				: null;
		}

		/// <returns>Null when no safe points to move.</returns>
		private (Point point, Direction direction) GetSafePointToMove(Board board) {
			var myPos = board.GetMe();
			if (!myPos.HasValue) {
				return default;
			}
			var leftPos = myPos.Value.ShiftLeft();
			if (board.CanMoveTo(leftPos)) {
				return (leftPos, Direction.Left);
			}
			var rightPos = myPos.Value.ShiftRight();
			if (board.CanMoveTo(rightPos)) {
				return (rightPos, Direction.Right);
			}
			var bottomPos = myPos.Value.ShiftBottom();
			if (board.CanMoveTo(bottomPos)) {
				return (bottomPos, Direction.Down);
			}
			var topPos = myPos.Value.ShiftTop();
			if (board.CanMoveTo(topPos)) {
				return (topPos, Direction.Up);
			}
			return default;
		}
	}
}