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

                while (priorityQueue.Count > 0)
                {
                    var currentColumn = priorityQueue.Dequeue();

                    if (assignedRow[currentColumn] == -1)
                    {
                        for (int column = 0; column < size; column++)
                        {
                            if (!scanned[column]) continue;
                            duals[column] += weights[column] - weights[currentColumn];
                        }

                        var augmentingPathColumn = currentColumn;

                        while (true)
                        {
                            if (previousColumn[augmentingPathColumn] == -1)
                            {
                                assignedRow[augmentingPathColumn] = row;
                                assignedColumn[row] = augmentingPathColumn;
                                break;
                            }

                            assignedRow[augmentingPathColumn] = assignedRow[previousColumn[augmentingPathColumn]];
                            assignedColumn[assignedRow[previousColumn[augmentingPathColumn]]] = augmentingPathColumn;
                            augmentingPathColumn = previousColumn[augmentingPathColumn];
                        }

                        break;
                    }

                    for (int nextColumn = 0; nextColumn < size; nextColumn++)
                    {
                        if (scanned[nextColumn] || nextColumn == currentColumn) continue;

                        int weight = costs[assignedRow[currentColumn], nextColumn] - duals[nextColumn] - 
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