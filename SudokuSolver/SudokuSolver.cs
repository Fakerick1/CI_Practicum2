using System.Text;

namespace SudokuSolver
{
    public class SudokuSolver
    {
        // Change these variables to determine how the output is printed to the console,
        // They are ordered in ascending order of impact on running time (no impact on the amount of cycles)

        // Prints a colored version of the sudoku at the start and at the finish, also shows how many steps it took
        public const bool PrintStartAndFinish = false;

        // Print the sudoku at every step
        public const bool PrettyPrint = false;
        // Black number = non-fixed node
        // Blue number = fixed node
        // Red background = value currently incorrect
        // Green background = value currently believed to be correct

        // This boolean determines whether forward checking is used in the algorithm or not.
        public const bool ForwardChecking = true;

        static void Main(string[] args)
        {
            List<String> inputs = new List<String>();

            // Change these lines if a new sudoku should be used or added, all listed sudokus will be solved.
            // Comment out sudokus if they should not be solved/attempted
            inputs.Add(" 0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0");
            inputs.Add(" 2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3");
            inputs.Add(" 0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0");
            inputs.Add(" 0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0");
            inputs.Add(" 0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0");

            int amountOfRuns = 100;

            Solver solver = new Solver(inputs, amountOfRuns);

            solver.Execute();
        }
    }
}