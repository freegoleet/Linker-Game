using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    [SerializeField] private GridManager grid;

    public void HandleTiles(List<Vector2> removedLocList) {
        List<Dictionary<Vector2, GameObject>> tileToRemove = new List<Dictionary<Vector2, GameObject>>();
        List<Dictionary<Dictionary<Vector2, GameObject>, int>> locGoSteps = new List<Dictionary<Dictionary<Vector2, GameObject>, int>>();

        foreach (Dictionary<Vector2, GameObject> locGoDict in grid.LocGO) {
            locGoSteps.Add(new Dictionary<Dictionary<Vector2, GameObject>, int>() { { locGoDict, 0 } });
        }

        foreach (Vector2 location in removedLocList) {
            foreach (Dictionary<Dictionary<Vector2, GameObject>, int> locGoStep in locGoSteps.ToList()) {
                foreach (Dictionary<Vector2, GameObject> locgo in locGoStep.Keys.ToList()) {
                    foreach (KeyValuePair<Vector2, GameObject> locGoKvp in locgo) {
                        if (locGoKvp.Key == location) {
                            tileToRemove.Add(locgo);
                        }

                        StepsToMove(location, locGoStep, locGoKvp);
                    }
                }
            }
        }

        foreach (Dictionary<Vector2, GameObject> tileDict in tileToRemove) {
            foreach (KeyValuePair<Vector2, GameObject> keyValuePair in tileDict) {
                grid.RemoveTileFromLists(tileDict, keyValuePair);
                keyValuePair.Value.SetActive(false);
            }
        }

        if (locGoSteps.Count > 0)
            MoveTiles(locGoSteps);

        tileToRemove.Clear();
        locGoSteps.Clear();
    }

    private static void StepsToMove(Vector2 location, Dictionary<Dictionary<Vector2, GameObject>, int> locGoStep, KeyValuePair<Vector2, GameObject> locGoKvp) {
        if (locGoKvp.Key.x == location.x) {
            if (locGoKvp.Key.y > location.y) {
                foreach (KeyValuePair<Dictionary<Vector2, GameObject>, int> keyValuePair in locGoStep.ToList()) {
                    locGoStep[keyValuePair.Key]++;
                }
            }
        }
    }

    private void MoveTiles(List<Dictionary<Dictionary<Vector2, GameObject>, int>> locGoSteps) {
        foreach (Dictionary<Dictionary<Vector2, GameObject>, int> locGoStep in locGoSteps) {
            foreach (KeyValuePair<Dictionary<Vector2, GameObject>, int> keyValue in locGoStep) {
                foreach (KeyValuePair<Vector2, GameObject> locgo in keyValue.Key) {
                    if (keyValue.Value > 0) {
                        GameObject tile = locgo.Value;

                        Color color = tile.GetComponent<SpriteRenderer>().color;
                        grid.RemoveTileFromLists(keyValue.Key, locgo);

                        tile.transform.position = new Vector2(tile.transform.position.x, tile.transform.position.y - locGoStep[keyValue.Key]);
                        tile.name = ("x: " + tile.transform.position.x + " y: " + tile.transform.position.y).ToString();

                        // Correct location to fit grid
                        Vector2 loc = new Vector2(tile.transform.position.x + (grid.horizontal - 0.5f), tile.transform.position.y + (grid.vertical - 0.5f));

                        grid.AddTileToLists(loc, tile, color);
                    }
                }
            }
        }
    }

//    private List<Vector2> FindEmptySlotsInGrid(List<Dictionary<Vector2, GameObject>> locGo = null) {


//        foreach (Dictionary<Vector2, GameObject> locgo in locGo.ToList()) {
//            foreach (KeyValuePair<Vector2, GameObject> locGoKvp in locgo) {
//                if (locGoKvp.Key.x == location.x) {
//                    if (locGoKvp.Key.y > location.y) {
//                        foreach (KeyValuePair<Dictionary<Vector2, GameObject>> keyValuePair in locGoStep.ToList()) {
//                            return locGoStep;
//                        }
//                    }
//                }
//            }
//        }

//    }
}
