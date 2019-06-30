using System;

namespace Solvers
{
    public interface IAssignmentSolver
    {
        int[] Solve(int[,] costMatrix);
    }
}