using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockPuzzleSolver
{
	public class Piece
	{
		public PieceType pieceType;
		public Vector2Int[] blocks;
		public PositionedPiece[] positionedPieces;


		public static Piece[] pieces = new Piece[] {
			new Piece(PieceType.A,
				"###.. " +
				"..#.. " +
				"..### "
			), new Piece(PieceType.B,
				"### " +
				"### " +
				"##. "
			), new Piece(PieceType.C,
				"### " +
				"#.. " +
				"#.. "
			), new Piece(PieceType.D,
				"### " +
				"#.. " +
				"#.. " +
				"#.. "
			), new Piece(PieceType.E,
				"## " +
				"## " +
				"#. " +
				"#. "
			), new Piece(PieceType.F,
				"## " +
				"## " +
				"## " +
				"## "
			), new Piece(PieceType.G,
				"## " +
				"## " +
				"#. "
			), new Piece(PieceType.H,
				"#.. " +
				"#.. " +
				"### " +
				"### "
			), new Piece(PieceType.I,
				"#. " +
				"#. " +
				"#. " +
				"## " +
				"## "
			), new Piece(PieceType.J,
				"## " +
				"#. " +
				"#. "
			)
		};

		public Piece(PieceType pieceType, string pieceString)
		{
			var blocks = new List<Vector2Int>();

			var rows = pieceString.Split(' ');
			for (int y = 0; y < rows.Length; y++)
				for (int x = 0; x < rows[y].Length; x++)
					if (rows[y][x] == '#') blocks.Add(new Vector2Int(x, y));

			this.blocks = blocks.ToArray();
			this.pieceType = pieceType;
		}

		public bool RemoveBlockingPositionedPieces()
		{
			var newPositionedPieces = new List<PositionedPiece>();
			var madeImprovement = false;

			foreach (PositionedPiece piece in positionedPieces)
			{
				var board = new Board();
				board.AddPiece(piece);

				if (!board.ContainsUnreachableAreas()) newPositionedPieces.Add(piece);
				else madeImprovement = true;
			}

			positionedPieces = newPositionedPieces.ToArray();
			return madeImprovement;
		}

		public void RemovePositionedPieceDuplicates()
		{
			var newPositionedPieces = new List<PositionedPiece>();
			var pieceBits = new List<long>();

			foreach (PositionedPiece piece in positionedPieces)
			{
				if (!pieceBits.Contains(piece.bits))
				{
					pieceBits.Add(piece.bits);
					newPositionedPieces.Add(piece);
				}
			}

			positionedPieces = newPositionedPieces.ToArray();
		}

		public void GenerateAllPositionedPieces()
		{
			GeneratePositionedPiecesForBlocks(blocks);
			GeneratePositionedPiecesForBlocks(BlockTransformation.RotateBlocks(blocks, false));
			GeneratePositionedPiecesForBlocks(BlockTransformation.RotateBlocks(blocks, true));
			GeneratePositionedPiecesForBlocks(BlockTransformation.RotateBlocks(BlockTransformation.RotateBlocks(blocks, true), true));

			GeneratePositionedPiecesForBlocks(BlockTransformation.FlipBlocks(blocks, false));
			GeneratePositionedPiecesForBlocks(BlockTransformation.FlipBlocks(BlockTransformation.RotateBlocks(blocks, false), false));
			GeneratePositionedPiecesForBlocks(BlockTransformation.FlipBlocks(BlockTransformation.RotateBlocks(blocks, true), false));
			GeneratePositionedPiecesForBlocks(BlockTransformation.FlipBlocks(BlockTransformation.RotateBlocks(BlockTransformation.RotateBlocks(blocks, true), true), false));
		}

		private void GeneratePositionedPiecesForBlocks(Vector2Int[] blocks)
		{
			var newPositionedPieces = new List<PositionedPiece>();
			newPositionedPieces.AddRange(positionedPieces ?? new PositionedPiece[0]);

			for (int y = 0; y < Board.boardSize.y; y++)
				for (int x = 0; x < Board.boardSize.x; x++)
				{
					var newBlocks = BlockTransformation.MoveBlocks(blocks, new Vector2Int(x, y));
					if (newBlocks.Any(b => b.IsOutOfBounds(Board.boardSize))) continue;
					newPositionedPieces.Add(new PositionedPiece(this, newBlocks));
				}
			positionedPieces = newPositionedPieces.ToArray();
		}

		public void Log()
		{
			Console.WriteLine("Piece Type " + pieceType.ToString() + ":");

			var width = blocks.Max(v => v.x);
			var height = blocks.Max(v => v.y);

			var s = "";
			for (int y = 0; y <= height; y++)
			{
				for (int x = 0; x <= width; x++)
					s += blocks.Contains(new Vector2Int(x, y)) ? "#" : ".";
				Console.WriteLine(s);
				s = "";
			}
		}
	}

	public enum PieceType
	{
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
	}

	public static class BlockTransformation
	{
		public static Vector2Int[] MoveBlocks(Vector2Int[] blocks, Vector2Int move)
		{
			return blocks.Select(v => v + move).ToArray();
		}
		public static Vector2Int[] RotateBlocks(Vector2Int[] blocks, bool ccw)
		{
			var newBlocks = new List<Vector2Int>();

			foreach (var block in blocks)
			{
				newBlocks.Add(ccw ? block.RotateCW() : block.RotateCCW()); // the rotation is flipped because of y being down
			}

			var minX = newBlocks.Min(v => v.x);
			var minY = newBlocks.Min(v => v.y);

			newBlocks = newBlocks.Select(v => new Vector2Int(v.x - minX, v.y - minY)).ToList();

			return newBlocks.ToArray();
		}

		public static Vector2Int[] FlipBlocks(Vector2Int[] blocks, bool horizontal)
		{
			var newBlocks = new List<Vector2Int>();

			foreach (var block in blocks)
			{
				newBlocks.Add(horizontal ? block.FlipHorizontally() : block.FlipVertically());
			}

			var minX = newBlocks.Min(v => v.x);
			var minY = newBlocks.Min(v => v.y);

			newBlocks = newBlocks.Select(v => new Vector2Int(v.x - minX, v.y - minY)).ToList();

			return newBlocks.ToArray();
		}
	}
}