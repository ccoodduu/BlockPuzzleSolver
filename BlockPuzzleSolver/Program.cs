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
		public static List<long> hashes = new List<long>();
		public static StringBuilder allSolutions = new StringBuilder();
		static void Main(string[] args)
		{

			foreach (var piece in Piece.pieces)
			{
				piece.GenerateAllPositionedPieces();
			}
			Console.WriteLine(Piece.pieces.Sum(p => p.positionedPieces.Length));

			foreach (var piece in Piece.pieces)
			{
				piece.RemovePositionedPieceDuplicates();
			}
			Console.WriteLine(Piece.pieces.Sum(p => p.positionedPieces.Length));
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

			var countsForStartingPieces = new int[11];
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

			for (int i = 0; i < startingPiece.positionedPieces.Length; i++)
			{
				PositionedPiece positionedPiece = startingPiece.positionedPieces[i];

				Console.WriteLine(countsForStartingPieces[i]);
				positionedPiece.Log();
			}

				using (StreamWriter writer = new StreamWriter("C:\\Users\\willi\\Git Projects\\C#\\BlockPuzzleSolver\\BlockPuzzleSolver\\solutions.txt"))
			{
				writer.Write(allSolutions.ToString());
			}

			Console.WriteLine("Press esc key to exit");
			while (true)
			{
				var key = Console.ReadKey();
				if (key.Key == ConsoleKey.Escape) break;
			}
		}

		static void Solve(Board board)
		{
			if (board.piecesAdded == 0b1111111111)
			{
				var hash = board.Hash();

				// if (Program.hashes.Contains(hash)) return;

				Program.count++;
				Console.WriteLine(count);

				allSolutions.Append(Program.count + "\n" + board.Log() + "\n");

				Program.hashes.Add(hash);

				return;
			}

			var i = board.pieces.Count;
			Piece piece = Piece.pieces[i];

			foreach (var positionedPiece in piece.positionedPieces)
			{
				if (!board.CanAddPiece(positionedPiece.bits)) continue;

				var bits = board.bits;
				board.AddPiece(positionedPiece);
				if (!board.ContainsUnreachableAreas()) Solve(board);
				board.RemovePiece(positionedPiece);
				if (bits != board.bits) Console.WriteLine("Board has changed!");
			}

		}
	}
}
