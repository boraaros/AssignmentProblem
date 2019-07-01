using PriorityQueues;
using System;
using System.Linq;

namespace Solvers
{
    public class AssignmentSolver : IAssignmentSolver
    {
        public int[] Solve(int[,] costs)
        {
            if (costs.GetLength(0) != costs.GetLength(1)) throw new ArgumentException("must be squared");

            int size = costs.GetLength(0);
            int[] assignedColumn = Enumerable.Repeat(-1, size).ToArray();
            int[] assignedRow = Enumerable.Repeat(-1, size).ToArray();
            int[] duals = Enumerable.Repeat(0, size).ToArray();

            for (int row = 0; row < size; row++)
            {
                int[] weights = Enumerable.Range(0, size).Select(j => costs[row, j] - duals[j]).ToArray();
                int[] previousColumn = Enumerable.Repeat(-1, size).ToArray();
                bool[] scanned = Enumerable.Repeat(false, size).ToArray();

                BinaryHeap<int, int> priorityQueue = new BinaryHeap<int, int>(PriorityQueueType.Minimum);
                IPriorityQueueEntry<int>[] entries = Enumerable.Range(0, size).Select(t => priorityQueue.Enqueue(t, weights[t])).ToArray();

                while (priorityQueue.Count > 0) // dijkstra
                {
                    int currentColumn = priorityQueue.Dequeue();

                    if (assignedRow[currentColumn] == -1) // goal: free column found
                    {
                        for (int column = 0; column < size; column++) // update dual variables
                        {
                            if (!scanned[column]) continue;
                            duals[column] += weights[column] - weights[currentColumn];
                        }

                        while (previousColumn[currentColumn] != -1) // update assignments along the alternating path
                        {
                            assignedRow[currentColumn] = assignedRow[previousColumn[currentColumn]];
                            assignedColumn[assignedRow[previousColumn[currentColumn]]] = currentColumn;
                            currentColumn = previousColumn[currentColumn];
                        }

                        assignedRow[currentColumn] = row;
                        assignedColumn[row] = currentColumn;
                        break;
                    }

                    for (int nextColumn = 0; nextColumn < size; nextColumn++) // expand
                    {
                        if (scanned[nextColumn] || nextColumn == currentColumn) continue;

                        int weight = costs[assignedRow[currentColumn], nextColumn] - duals[nextColumn] - // weight >= 0
                                    (costs[assignedRow[currentColumn], currentColumn] - duals[currentColumn]);

                        if (weights[currentColumn] + weight < weights[nextColumn])
                        {
                            weights[nextColumn] = weights[currentColumn] + weight;
                            previousColumn[nextColumn] = currentColumn;
                            priorityQueue.UpdatePriority(entries[nextColumn], weights[nextColumn]);
                        }
                    }

                    scanned[currentColumn] = true;
                }   
            }

            return assignedColumn;
        }
    }
}