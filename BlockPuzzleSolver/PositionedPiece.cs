using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzleSolver
{
	public class PositionedPiece
	{
		public long bits;
		public PieceType pieceType;

		public PositionedPiece(Piece piece, Vector2Int[] blocks)
		{
			bits = 0;
			if (blocks.Length > 5) 
			{
				;
			}
			foreach (var block in blocks)
			{
				var n = block.x + block.y * Board.boardSize.x;
				bits += (long)1 << n;
			}

			pieceType = piece.pieceType;
		}

		public static PositionedPiece GenerateFromPiece(Piece piece, Vector2Int pos)
		{
			return new PositionedPiece(piece, BlockTransformation.MoveBlocks(piece.blocks, pos));
		}

		public void Log()
		{
			Console.WriteLine("Piece Type " + pieceType.ToString() + ":");

			var s = "";
			for (int i = 0; i < Board.boardSize.x * Board.boardSize.y; i++)
			{
				s += (((long)1 << i) & bits) != 0 ? "#" : ".";

				if (s.Length == Board.boardSize.x)
				{
					Console.WriteLine(s);
					s = "";
				}
			}
		}
	}
}
