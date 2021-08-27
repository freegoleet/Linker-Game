using System.Collections.Generic;
using UnityEngine;

public class NeighborFinder : MonoBehaviour
{
    public void FindNeighbors(GridManager grid, Queue<Vector2> frontier, List<Vector2> reached, Vector2 location, bool frontierCleared) { // Breadth First Search
        frontier.Clear();
        foreach (Vector2 next in grid.Neighbors(location)) {
            foreach (Dictionary<Vector2, GameObject> locGo in grid.LocGO) {
                if (locGo.TryGetValue(next, out GameObject checkNull)) {
                    if (checkNull != null) {
                        if (!reached.Contains(next)) {
                            if (frontierCleared == false) {
                                frontier.Clear();
                                frontierCleared = true;
                            }

                            if (!frontier.Contains(next)) {
                                frontier.Enqueue(next);
                            }
                        }
                    }
                }
            }
        }

    }
}
