﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICanCode.Api
{
	public class Board : AbstractLayeredBoard {
		public Board(string boardString) : base(boardString) {
		}

		/**
        * @return Checks if your robot is alive.
        */
		public bool IsMeAlive() {
			return Get(Layers.LAYER2, Element.ROBO_FALLING, Element.ROBO_LASER).Count == 0;
		}

		public Direction? GetOppositeDirection(Direction? direction) {
			switch (direction) {
				case Direction.Down:
					return Direction.Up;
				case Direction.Up:
					return Direction.Down;
				case Direction.Left:
					return Direction.Right;
				case Direction.Right:
					return Direction.Left;
				default:
					return null;
			}
		}

		public int GetLinearDistance(Point p1, Point p2) {
			return Convert.ToInt32(Math.Floor(Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2))));
		}

		/// <param name="from">Source point.</param>
		/// <param name="direction">Direction to fire.</param>
		/// <returns></returns>
		public int GetNearestEnemyDistanceByDirection(Point from, Direction direction) {
			var otherRobot = new[] {
				Element.ROBO_OTHER,
				Element.ROBO_OTHER_FALLING,
				Element.FEMALE_ZOMBIE,
				Element.MALE_ZOMBIE
			};
			var myPos = GetMe();
			if (from.IsOutOf(Size) || !myPos.HasValue) {
				return 0;
			}
			switch (direction) {
				case Direction.Down:
					var target = from.ShiftBottom();
					if (!CanGoAt(target)) {
						break;
					}
					if (IsAt(target, otherRobot)) {
						return GetLinearDistance(myPos.Value, target);
					}
					return GetNearestEnemyDistanceByDirection(target, direction);
				case Direction.Up:
					target = from.ShiftTop();
					if (!CanGoAt(target)) {
						break;
					}
					if (IsAt(target, otherRobot)) {
						return GetLinearDistance(myPos.Value, target);
					}
					return GetNearestEnemyDistanceByDirection(target, direction);
				case Direction.Left:
					target = from.ShiftLeft();
					if (!CanGoAt(target)) {
						break;
					}
					if (IsAt(target, otherRobot)) {
						return GetLinearDistance(myPos.Value, target);
					}
					return GetNearestEnemyDistanceByDirection(target, direction);
				case Direction.Right:
					target = from.ShiftRight();
					if (!CanGoAt(target)) {
						break;
					}
					if (IsAt(target, otherRobot)) {
						return GetLinearDistance(myPos.Value, target);
					}
					return GetNearestEnemyDistanceByDirection(target, direction);
			}
			return 0;
		}

		/**
         * @param x X coordinate.
         * @param y Y coordinate.
         * @return Is it possible to go through the cell with {x,y} coordinates.
         */
		public bool IsBarrierAt(int x, int y) {
			return !IsAt(Layers.LAYER1, x, y, Element.FLOOR, Element.START, Element.EXIT, Element.GOLD, Element.HOLE) ||
					!IsAt(Layers.LAYER2, x, y,
					Element.GOLD,
					Element.LASER_DOWN,
					Element.LASER_UP,
					Element.LASER_LEFT,
					Element.LASER_RIGHT,
					Element.ROBO_OTHER,
					Element.ROBO_OTHER_FLYING,
					Element.ROBO_OTHER_FALLING,
					Element.ROBO_OTHER_LASER,
					Element.ROBO,
					Element.ROBO_FLYING,
					Element.ROBO_FALLING,
					Element.ROBO_LASER);
		}


		public bool IsBarrierAt(Point p) => IsBarrierAt(p.X, p.Y);

		public bool CanGoAt(Point p) {
			return (!IsAt(p,
				Element.BOX,
				Element.FEMALE_ZOMBIE,
				Element.MALE_ZOMBIE,
				Element.LASER_MACHINE_READY_LEFT,
				Element.LASER_MACHINE_READY_RIGHT,
				Element.LASER_MACHINE_READY_UP,
				Element.LASER_MACHINE_READY_DOWN,
				Element.LASER_MACHINE_CHARGING_LEFT,
				Element.LASER_MACHINE_CHARGING_RIGHT,
				Element.LASER_MACHINE_CHARGING_UP,
				Element.LASER_MACHINE_CHARGING_DOWN,
				Element.ANGLE_IN_LEFT,
				Element.WALL_FRONT,
				Element.ANGLE_IN_RIGHT,
				Element.WALL_RIGHT,
				Element.ANGLE_BACK_RIGHT,
				Element.WALL_BACK,
				Element.ANGLE_BACK_LEFT,
				Element.WALL_LEFT,
				Element.WALL_BACK_ANGLE_LEFT,
				Element.WALL_BACK_ANGLE_RIGHT,
				Element.ANGLE_OUT_RIGHT,
				Element.ANGLE_OUT_LEFT,
				Element.ZOMBIE_START
				)) || p.IsOutOf(Size);
		}

		public bool CanMoveTo(Point pos) {
			if (!CanGoAt(pos)) {
				return false;
			}
			if (IsDangerAt(pos, Direction.Left)
				&& IsDangerAt(pos, Direction.Right)
				&& IsDangerAt(pos, Direction.Down)
				&& IsDangerAt(pos, Direction.Up)) {
				return false;
			}
			return true;
		}

		public bool IsMyPointSafe() {
			var myPos = GetMe();
			if (!myPos.HasValue) {
				return true;
			} 
			return (!IsDangerAt(myPos.Value.ShiftLeft(), Direction.Left))
				&& (!IsDangerAt(myPos.Value.ShiftRight(), Direction.Right))
				&& (!IsDangerAt(myPos.Value.ShiftTop(), Direction.Up))
				&& (!IsDangerAt(myPos.Value.ShiftBottom(), Direction.Down));
		}

		public bool IsEnemyAtDirection(Direction direction, int steps = 1) {
			var enemies = new[] {
				Element.ROBO_OTHER,
				Element.ROBO_OTHER_FALLING,
				Element.FEMALE_ZOMBIE,
				Element.MALE_ZOMBIE
			};
			var pos = GetMe();
			if (!pos.HasValue) {
				return true;
			}
			var targetPos = pos.Value.Shift(direction, steps);
			return IsAt(targetPos, enemies);
		}

		public bool IsDangerAt(Point pos, Direction direction) {
			var dangerElements = new[] {
				Element.ROBO_OTHER_LASER,
				Element.ROBO_LASER,
				Element.FEMALE_ZOMBIE,
				Element.MALE_ZOMBIE
			};
			switch (direction) {
				case Direction.Left:
					return IsAt(pos, dangerElements.Concat(new[] { Element.LASER_RIGHT }).ToArray())
						|| IsAt(pos.ShiftLeft(), dangerElements.Concat(new[] { Element.LASER_RIGHT, Element.LASER_MACHINE_READY_RIGHT }).ToArray())
						|| IsAt(pos.ShiftBottom(), dangerElements.Concat(new[] { Element.LASER_UP, Element.LASER_MACHINE_READY_UP }).ToArray())
						|| IsAt(pos.ShiftTop(), dangerElements.Concat(new[] { Element.LASER_DOWN, Element.LASER_MACHINE_READY_DOWN }).ToArray());
				case Direction.Right:
					return IsAt(pos, dangerElements.Concat(new[] { Element.LASER_LEFT }).ToArray())
						|| IsAt(pos.ShiftRight(), dangerElements.Concat(new[] { Element.LASER_LEFT, Element.LASER_MACHINE_READY_LEFT }).ToArray())
						|| IsAt(pos.ShiftBottom(), dangerElements.Concat(new[] { Element.LASER_UP, Element.LASER_MACHINE_READY_UP }).ToArray())
						|| IsAt(pos.ShiftTop(), dangerElements.Concat(new[] { Element.LASER_DOWN, Element.LASER_MACHINE_READY_DOWN }).ToArray());
				case Direction.Down:
					return IsAt(pos, dangerElements.Concat(new[] { Element.LASER_UP }).ToArray())
						|| IsAt(pos.ShiftBottom(), dangerElements.Concat(new[] { Element.LASER_UP, Element.LASER_MACHINE_READY_UP }).ToArray())
						|| IsAt(pos.ShiftRight(), dangerElements.Concat(new[] { Element.LASER_LEFT, Element.LASER_MACHINE_READY_LEFT }).ToArray())
						|| IsAt(pos.ShiftLeft(), dangerElements.Concat(new[] { Element.LASER_RIGHT, Element.LASER_MACHINE_READY_RIGHT }).ToArray());
				case Direction.Up:
					return IsAt(pos, dangerElements.Concat(new[] { Element.LASER_DOWN }).ToArray())
						|| IsAt(pos.ShiftTop(), dangerElements.Concat(new[] { Element.LASER_DOWN, Element.LASER_MACHINE_READY_DOWN }).ToArray())
						|| IsAt(pos.ShiftRight(), dangerElements.Concat(new[] { Element.LASER_LEFT, Element.LASER_MACHINE_READY_LEFT }).ToArray())
						|| IsAt(pos.ShiftLeft(), dangerElements.Concat(new[] { Element.LASER_RIGHT, Element.LASER_MACHINE_READY_RIGHT }).ToArray());
				default:
					return false;
			}
		}

		/// <summary>
		/// L1: Element.FLOOR, Element.START, Element.EXIT, Element.GOLD, Element.HOLE
		/// L2: Element.EMPTY,
		//Element.GOLD,
		//Element.LASER_DOWN,
		//Element.LASER_UP,
		//Element.LASER_LEFT,
		//Element.LASER_RIGHT,
		//Element.ROBO_OTHER,
		//Element.ROBO_OTHER_FLYING,
		//Element.ROBO_OTHER_FALLING,
		//Element.ROBO_OTHER_LASER,
		//Element.ROBO,
		//Element.ROBO_FLYING,
		//Element.ROBO_FALLING,
		//Element.ROBO_LASER);
		/// </summary>
		public bool IsAt(int x, int y, params Element[] elements) {
			return IsAt(Layers.LAYER1, x, y, elements) ||
					IsAt(Layers.LAYER2, x, y, elements) ||
					IsAt(Layers.LAYER3, x, y, elements);
		}
		public bool IsAt(Point point, params Element[] elements) =>
			IsAt(point.X, point.Y, elements);

		public bool IsAllAt(Point point, params Element[] elements) => elements.All(el => IsAt(point, el));

		public Point? GetMe() {
			List<Point> points = new List<Point>();
			points.AddRange(Get(Layers.LAYER2,
					Element.ROBO_FALLING,
					Element.ROBO_LASER,
					Element.ROBO));
			points.AddRange(Get(Layers.LAYER3,
				   Element.ROBO_FLYING));
			
			return points.Count < 1
				? default(Point?)
				: points[0];
		}

		public List<Point> GetOtherHeroes() {
			List<Point> points = new List<Point>();
			points.AddRange(Get(Layers.LAYER2,
				   Element.ROBO_OTHER_FALLING,
				   Element.ROBO_OTHER_LASER,
				   Element.ROBO_OTHER));
			points.AddRange(Get(Layers.LAYER3,
				   Element.ROBO_OTHER_FLYING));
			return points;
		}

		/**
        * @return Returns list of coordinates for all visible Exit points.
        */
		public List<Point> GetExits() {
			return Get(Layers.LAYER1, Element.EXIT);
		}

		/**
         * @return Returns list of coordinates for all visible Start points.
         */
		public List<Point> GetStarts() {
			return Get(Layers.LAYER1, Element.START);
		}

		/**
         * @return Returns list of coordinates for all visible Gold.
         */
		public List<Point> GetGold() {
			return Get(Layers.LAYER1, Element.GOLD);
		}

		/**
         * @return Returns list of coordinates for all Zombies (even die).
         */
		public List<Point> GetZombies() {
			return Get(Layers.LAYER2,
					Element.FEMALE_ZOMBIE,
					Element.MALE_ZOMBIE,
					Element.ZOMBIE_DIE);
		}

		/**
        * @return Returns list of coordinates for all visible Boxes.
        */
		public List<Point> GetBoxes() {
			return Get(Layers.LAYER2,
					Element.BOX);
		}

		/**
         * @return Returns list of coordinates for all visible Holes.
         */
		public List<Point> GetHoles() {
			return Get(Layers.LAYER1,
					Element.HOLE,
					Element.ROBO_FALLING,
					Element.ROBO_OTHER_FALLING);
		}

		/**
        * @return Returns list of coordinates for all visible LaserMachines.
        */
		public List<Point> GetLaserMachines() {
			return Get(Layers.LAYER1,
					Element.LASER_MACHINE_CHARGING_LEFT,
					Element.LASER_MACHINE_CHARGING_RIGHT,
					Element.LASER_MACHINE_CHARGING_UP,
					Element.LASER_MACHINE_CHARGING_DOWN,

					Element.LASER_MACHINE_READY_LEFT,
					Element.LASER_MACHINE_READY_RIGHT,
					Element.LASER_MACHINE_READY_UP,
					Element.LASER_MACHINE_READY_DOWN);
		}

		/**
         * @return Returns list of coordinates for all visible Lasers.
         */
		public List<Point> GetLasers() {
			return Get(Layers.LAYER2,
					Element.LASER_LEFT,
					Element.LASER_RIGHT,
					Element.LASER_UP,
					Element.LASER_DOWN);
		}

		/**
         * @return Returns list of coordinates for all perks.
         */
		public List<Point> GetPerks() {
			return Get(Layers.LAYER1,
					Element.UNSTOPPABLE_LASER_PERK,
					Element.DEATH_RAY_PERK,
					Element.UNLIMITED_FIRE_PERK);
		}

		public override string ToString() {
			string temp = "0123456789012345678901234567890";

			StringBuilder builder = new StringBuilder();
			string[] layer1 = boardAsString(Layers.LAYER1).Split('\n');
			string[] layer2 = boardAsString(Layers.LAYER2).Split('\n');
			string[] layer3 = boardAsString(Layers.LAYER3).Split('\n');

			string numbers = temp.Substring(0, layer1.Length);
			string space = "".PadLeft(layer1.Length - 4);
			string numbersLine = numbers + "   " + numbers + "   " + numbers;
			string firstPart = " Layer1 " + space + " Layer2" + space + " Layer3" + "\n  " + numbersLine;

			for (int i = 0; i < layer1.Length; ++i) {
				int ii = Size - 1 - i;
				string index = (ii < 10 ? " " : "") + ii;
				builder.Append(index + layer1[i]
						+ " " + index + MaskOverlay(layer2[i], layer1[i])
						+ " " + index + MaskOverlay(layer3[i], layer1[i]));

				switch (i) {
					case 0:
						builder.Append(" Robots: " + GetMe() + "," + ListToString(GetOtherHeroes()));
						break;
					case 1:
						builder.Append(" Gold: " + ListToString(GetGold()));
						break;
					case 2:
						builder.Append(" Starts: " + ListToString(GetStarts()));
						break;
					case 3:
						builder.Append(" Exits: " + ListToString(GetExits()));
						break;
					case 4:
						builder.Append(" Boxes: " + ListToString(GetBoxes()));
						break;
					case 5:
						builder.Append(" Holes: " + ListToString(GetHoles()));
						break;
					case 6:
						builder.Append(" LaserMachine: " + ListToString(GetLaserMachines()));
						break;
					case 7:
						builder.Append(" Lasers: " + ListToString(GetLasers()));
						break;
					case 8:
						builder.Append(" Zombies: " + ListToString(GetZombies()));
						break;
					case 9:
						builder.Append(" Perks: " + ListToString(GetPerks()));
						break;
				}

				if (i != layer1.Length - 1) {
					builder.Append("\n");
				}
			}

			return firstPart + "\n" + builder.ToString() + "\n  " + numbersLine;
		}


		public string MaskOverlay(string source, string mask) {
			StringBuilder result = new StringBuilder(source);
			for (int i = 0; i < result.Length; ++i) {
				Element el = (Element)mask[i];
				if (IsWall(el)) {
					result[i] = (char)el;
				}
			}

			return result.ToString();
		}

		private string ListToString(List<Point> list) {
			return string.Join(",", list.ToArray());
		}

		public static bool IsWall(Element el) {
			Element[] map = new Element[] {
					Element.ANGLE_IN_LEFT,
					Element.WALL_FRONT,
					Element.ANGLE_IN_RIGHT,
					Element.WALL_RIGHT,
					Element.ANGLE_BACK_RIGHT,
					Element.WALL_BACK,
					Element.ANGLE_BACK_LEFT,
					Element.WALL_LEFT,
					Element.WALL_BACK_ANGLE_LEFT,
					Element.WALL_BACK_ANGLE_RIGHT,
					Element.ANGLE_OUT_RIGHT,
					Element.ANGLE_OUT_LEFT,
					Element.SPACE };

			return map.Contains(el);
		}
	}
}
