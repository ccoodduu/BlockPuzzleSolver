using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzleSolver
{
	public class Board
	{
		public static Vector2Int boardSize = new Vector2Int(8);
		public long bits;
		public int piecesAdded;
		public List<PositionedPiece> pieces;

		public Board()
		{
			bits = 0;
			pieces = new List<PositionedPiece>();
			piecesAdded = 0;
		}

		public void AddPiece(PositionedPiece positionedPiece)
		{
			bits ^= positionedPiece.bits;
			piecesAdded += 1 << (int)positionedPiece.pieceType;
			pieces.Add(positionedPiece);
		}

		public void RemovePiece(PieceType pieceType)
		{
			RemovePiece(pieces.First(p => p.pieceType == pieceType));
		}

		public void RemovePiece(PositionedPiece positionedPiece)
		{
			bits &= ~positionedPiece.bits;
			piecesAdded -= 1 << (int)positionedPiece.pieceType;
			pieces.Remove(positionedPiece);
		}

		public bool CanAddPiece(long bits) => (bits & this.bits) == 0;

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

		public long Hash()
		{
			long hash = 4721923745439584;

			foreach (var piece in pieces)
			{
				hash ^= piece.bits * piece.bits * 1014335340734895793;
				hash += piece.bits * 1145631435709204824;
				hash ^= piece.bits * piece.bits * 6907834879879146858;
			}
			return hash;
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
