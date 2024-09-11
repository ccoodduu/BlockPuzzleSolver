using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockPuzzleSolver
{
	public class Program
	{
		public static int count = 0;
		public static StringBuilder allSolutions = new StringBuilder();
		static void Main(string[] args)
		{
			Console.WriteLine("Full path to output file (leave empty if no file is wanted): ");
			var path = Console.ReadLine();

			using (StreamWriter writer = new StreamWriter(path))
			{
				writer.Write("");
			}

			// Generate all orientations and positions of all pieces.
			foreach (var piece in Piece.pieces)
			{
				piece.GenerateAllPositionedPieces();
			}
			Console.WriteLine(Piece.pieces.Sum(p => p.positionedPieces.Length));

			// Remove all duplicates, this is to ensure pieces with rotational or mirror symmetry don't have duplicates.
			foreach (var piece in Piece.pieces)
			{
				piece.RemovePositionedPieceDuplicates();
			}
			Console.WriteLine(Piece.pieces.Sum(p => p.positionedPieces.Length));

			// Iteratively remove all piece positions that immediately cause blank spaces that can't be filled.
			while (true)
			{
				var madeImprovement = false;

				foreach (var piece in Piece.pieces)
				{
					if (piece.RemoveBlockingPositionedPieces()) madeImprovement = true;
				}

				if (!madeImprovement) break;
			}
			Console.WriteLine(Piece.pieces.Sum(p => p.positionedPieces.Length));


			// Manually make all position for the "Z"-piece. This is so that we automatically don't get mirror or rotational board symmetry.
			var startingPiece = Piece.pieces.First();

			startingPiece.positionedPieces = new PositionedPiece[]{
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(3, 5)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(2, 5)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(1, 5)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(3, 4)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(2, 4)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(1, 4)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(0, 4)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(3, 3)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(2, 3)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(1, 3)),
				PositionedPiece.GenerateFromPiece(startingPiece, new Vector2Int(0, 3)),
			};


			var countsForStartingPieces = new int[11]; // This is just for fun, it's to see how many solutions exist for each starting position.

			// Solve!
			for (int i = 0; i < startingPiece.positionedPieces.Length; i++)
			{
				PositionedPiece positionedPiece = startingPiece.positionedPieces[i];
				var board = new Board();

				var countBefore = Program.count;
				board.AddPiece(positionedPiece);
				Solve(board);
				board.RemovePiece(positionedPiece);
				countsForStartingPieces[i] = Program.count - countBefore;
			}

			// Log results
			for (int i = 0; i < startingPiece.positionedPieces.Length; i++)
			{
				PositionedPiece positionedPiece = startingPiece.positionedPieces[i];

				Console.WriteLine(countsForStartingPieces[i]);
				positionedPiece.Log();
			}

			// Save results
			if (path != "")
			{
				using (StreamWriter writer = new StreamWriter(path))
				{
					writer.Write(allSolutions.ToString());
				}
			} 
			
			// Keep console open
			Console.WriteLine("Press esc key to exit");
			while (true)
			{
				var key = Console.ReadKey();
				if (key.Key == ConsoleKey.Escape) break;
			}
		}

		static void Solve(Board board)
		{
			// No more empty space, solution found
			if (board.piecesAdded == 0b1111111111)
			{
				Program.count++;
				Console.WriteLine(count);

				allSolutions.Append(Program.count + "\n" + board.Log() + "\n");

				return;
			}

			int pieceIndex = board.pieces.Count;
			Piece piece = Piece.pieces[pieceIndex];

			foreach (var positionedPiece in piece.positionedPieces)
			{
				if (!board.CanAddPiece(positionedPiece.bits)) continue;

				board.AddPiece(positionedPiece);

				// Before begin trying new pieces, for faster execution, check if the board has unreachable areas
				if (!board.ContainsUnreachableAreas()) 
					Solve(board);

				board.RemovePiece(positionedPiece);
			}

		}
	}
}
