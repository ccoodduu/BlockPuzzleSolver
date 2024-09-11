using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockPuzzleSolver
{
	public class Board
	{
		public static Vector2Int boardSize = new Vector2Int(8, 8);
		public long bits; // 64 bit number, where each bit is a square on the board, 1 is a piece, 0 is empty.
		public int piecesAdded; // Each bit is a quick way of checking weather this piece-type is already added to the board.
		public List<PositionedPiece> pieces; // This is only used for printing the board, so you know which piece is which.

		public Board()
		{
			bits = 0;
			pieces = new List<PositionedPiece>();
			piecesAdded = 0;
		}

		public void AddPiece(PositionedPiece positionedPiece)
		{
			// using this bit manipulation we can quickly add a piece to the board
			bits |= positionedPiece.bits;
			piecesAdded += 1 << (int)positionedPiece.pieceType;

			pieces.Add(positionedPiece);
		}

		public void RemovePiece(PieceType pieceType)
		{
			RemovePiece(pieces.First(p => p.pieceType == pieceType));
		}

		public void RemovePiece(PositionedPiece positionedPiece)
		{
			// using this bit manipulation we can quickly remove a piece from the board
			bits &= ~positionedPiece.bits;
			piecesAdded -= 1 << (int)positionedPiece.pieceType;

			pieces.Remove(positionedPiece);
		}

		public bool CanAddPiece(long bits)
		{
			// using this bitwise and we can quickly check if the piece overlaps with something already on the board.
			return (bits & this.bits) == 0;
		}

		public bool ContainsUnreachableAreas()
		{
			long filledBits = bits;

			for (int i = 0; i < Piece.pieces.Length; i++)
			{
				if ((piecesAdded & (1 << i)) != 0) continue;
				var piece = Piece.pieces[i];
				var fits = false;
				foreach (var positionedPiece in piece.positionedPieces)
				{
					if (CanAddPiece(positionedPiece.bits))
					{
						filledBits |= positionedPiece.bits;
						fits = true;
					}

					if (filledBits == ~0) return false;
				}

				if (!fits) return true;
			}

			return filledBits != ~0;
		}

		public string Log()
		{
			var str = "";
			var s = "";
			for (int y = 0; y < boardSize.y; y++)
			{
				for (int x = 0; x < boardSize.x; x++)
				{
					var i = y * boardSize.x + x;

					foreach (var piece in pieces)
					{
						if ((((long)1 << i) & piece.bits) != 0)
						{
							s += piece.pieceType.ToString();
							break;
						}
					}
					if (s.Length == x) s += ".";
				}

				Console.WriteLine(s);
				str += s + "\n";
				s = "";
			}

			return str;
		}
	}
}
