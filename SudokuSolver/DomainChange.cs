using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class DomainChange
    {
        private int i;
        private int j;
        private int value;

        public DomainChange(int i, int j, int value)
        {
            this.i = i;
            this.j = j;
            this.value = value;
        }

        public int Row()
        {
            return i;
        }

        public int Column()
        {
            return j;
        }

        public int Value()
        {
            return value;
        }
    }
}
