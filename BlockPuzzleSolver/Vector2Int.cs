using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzleSolver
{
	public struct Vector2Int
	{
		public int x;
		public int y;

		public Vector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2Int(int n)
		{
			this.x = n;
			this.y = n;
		}

		public static Vector2Int Zero => new Vector2Int(0, 0);
		public Vector2Int RotateCW() => new Vector2Int(y, -x);
		public Vector2Int RotateCCW() => new Vector2Int(-y, x);
		public Vector2Int Rotate180() => new Vector2Int(-x, -y);
		public Vector2Int FlipHorizontally() => new Vector2Int(-x, y);
		public Vector2Int FlipVertically() => new Vector2Int(x, -y);
		public bool IsOutOfBounds(Vector2Int mapSize) => x >= mapSize.x || y >= mapSize.y || x < 0 || y < 0;

		public override bool Equals(object obj) => obj is Vector2Int @int && x == @int.x && y == @int.y;
		public override int GetHashCode()
		{
			int hashCode = 1502939027;
			hashCode = hashCode * -1521134295 + x.GetHashCode();
			hashCode = hashCode * -1521134295 + y.GetHashCode();
			return hashCode;
		}

		public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new Vector2Int(a.x + b.x, a.y + b.y);
		public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new Vector2Int(a.x - b.x, a.y - b.y);
		public static Vector2Int operator -(Vector2Int a) => new Vector2Int(-a.x, -a.y);
		public static Vector2Int operator *(Vector2Int a, int scalar) => new Vector2Int(a.x * scalar, a.y * scalar);
		public static Vector2Int operator /(Vector2Int a, int scalar) => new Vector2Int(a.x / scalar, a.y / scalar);
		public static bool operator ==(Vector2Int a, Vector2Int b) => a.x == b.x && a.y == b.y;
		public static bool operator !=(Vector2Int a, Vector2Int b) => a.x != b.x || a.y != b.y;


	}
}
