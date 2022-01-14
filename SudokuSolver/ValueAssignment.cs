using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class ValueAssignment
    {
        private int i;
        private int j;
        private int value;

        public ValueAssignment(int i, int j, int value)
        {
            this.i = i;
            this.j = j;
            this.value = value;
        }

        public int I()
        {
            return i;
        }

        public int J()
        {
            return j;
        }

        public int Value()
        {
            return value;
        }
    }
}
