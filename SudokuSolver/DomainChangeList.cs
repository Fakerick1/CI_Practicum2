using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class DomainChangeList
    {
        private (int i, int j) source;
        private List<DomainChange> domainChangeList;

        public DomainChangeList ((int i, int j) source)
        {
            this.source = source;
            this.domainChangeList = new List<DomainChange>();
        }

        public void AddDomainChange(DomainChange domainChange)
        {
            domainChangeList.Add(domainChange);
        }

        public (int i, int j) Source()
        {
            return source;
        }

        public List<DomainChange> DomainChanges()
        {
            return domainChangeList;
        }
    }
}
