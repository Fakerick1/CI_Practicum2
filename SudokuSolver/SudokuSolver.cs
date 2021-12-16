using System.Text;

namespace SudokuSolver
{
    public class SudokuSolver
    {
        // Change these variables to determine how the output is printed to the console,
        // they are ordered in ascending order of impact on running time (no impact on the amount of cycles)

        // Prints a colored version of the sudoku at the start and at the finish, also shows how many steps it took
        public const bool PrintStartAndFinish = true;

        // Print heuristic values at every step
        public const bool PrintHeuristics = false;

        // Print the sudoku at every step
        public const bool PrettyPrint = false;
        // Black number = non-fixed node
        // Blue number = fixed node
        // Red background = row incorrect, column incorrect
        // Yellow background  = row or column correct
        // Green background = both row and column correct

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

            // Configuration values used in experiments/testing
            int amountOfRuns = 10;
            int[] sValues = new int[] { 3, 4, 5 };
            int[] plateauValues = new int[] { 8, 10, 12 };

            // Set to true if sValues & plateauValues should be used, if false the sudokus added to the inputs list will be solved once with the given parameters
            bool runTests = false;

            Solver solver = (runTests) ? new Solver(inputs, amountOfRuns, sValues, plateauValues) : new Solver(inputs);

            solver.Execute();
        }
    }
}