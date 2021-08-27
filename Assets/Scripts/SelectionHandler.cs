using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager grid;
    [SerializeField] private TileHandler tileHandler;
    [SerializeField] private NeighborFinder neighborFinder;
    [SerializeField] private Score score;

    [Header("Controls")]
    public KeyCode confirm = KeyCode.Space;
    public KeyCode select = KeyCode.Mouse0;

    private readonly Queue<Vector2> frontier = new Queue<Vector2>();
    private readonly List<Vector2> reached = new List<Vector2>();
    private Vector2 currentLocation;
    private GameObject tileToPath;
    private bool frontierCleared;
    private Color chosenColor = Color.clear;

    public List<GameObject> markedTilesList;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ConfirmSelection();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) { // Left Mouse
            SelectTile();
        }
    }

    private void SelectTile() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (grid.GOLoc.TryGetValue(hit.transform.gameObject, out currentLocation)) {
                TryToLink(reached, frontier, currentLocation);
                frontierCleared = false;
            }
        }
    }

    private void TryToLink(List<Vector2> reached, Queue<Vector2> frontier, Vector2 target) {
        foreach (Dictionary<Vector2, GameObject> locGo in grid.LocGO) {
            if (locGo.TryGetValue(target, out tileToPath)) {
                if (tileToPath == null)
                    break;

                SpriteRenderer sprite = tileToPath.GetComponent<SpriteRenderer>();

                if (reached.Count < 1) {
                    StartLink(reached, frontier);
                }
                else if (locGo.ContainsKey(reached[reached.Count - 1])) {
                    BacktrackLink(reached, frontier, target);
                }
                else {
                    ContinueLink(reached, frontier, target, sprite);
                }
            }
        }
    }

    private void StartLink(List<Vector2> reached, Queue<Vector2> frontier) {
        MarkSelectedTile(currentLocation);
        frontier.Enqueue(currentLocation);
        reached.Add(currentLocation);

        frontierCleared = true;
        neighborFinder.FindNeighbors(grid, frontier, reached, reached[reached.Count - 1], frontierCleared);

        if (chosenColor == Color.clear) {
            grid.GOColor.TryGetValue(tileToPath, out Color color);
            chosenColor = color;
        }
    }

    private void ContinueLink(List<Vector2> reached, Queue<Vector2> frontier, Vector2 target, SpriteRenderer sprite) {
        if (frontier.Contains(target)) {
            if (!reached.Contains(target)) { // If you haven't already pathed here
                if (sprite.color == chosenColor) {
                    reached.Add(target);
                    neighborFinder.FindNeighbors(grid, frontier, reached, target, frontierCleared);
                    MarkSelectedTile(target);
                    score.UpdateMultiplier(reached.Count);
                }
            }
        }
    } 

    private void BacktrackLink(List<Vector2> reached, Queue<Vector2> frontier, Vector2 target) {
        if (reached[reached.Count - 1] == target) {
            if(reached.Remove(target)) {
                Destroy(markedTilesList[markedTilesList.Count - 1]);
                markedTilesList.RemoveAt(markedTilesList.Count - 1);

                Vector2 newTarget = reached[reached.Count - 1];
                neighborFinder.FindNeighbors(grid, frontier, reached, newTarget, frontierCleared);

                markedTilesList[markedTilesList.Count - 1].GetComponent<SpriteRenderer>().color = grid.currentTileColor;
                score.UpdateMultiplier(reached.Count);
            }
        }
    }

    private void ConfirmSelection() {
        // Add score
        score.UpdateScore(reached.Count);

        // Set up temporary lists for clean up with less risk 
        // TODO: Is it really necessary to create new lists for removal?
        List<Vector2> locToRemove = new List<Vector2>();
        List<Dictionary<Vector2, GameObject>> tempLocGO = new List<Dictionary<Vector2, GameObject>>();

        foreach (Vector2 loc in reached) {
            foreach (Dictionary<Vector2, GameObject> locGo in grid.LocGO) {
                if (locGo.ContainsKey(loc)) { 
                    locToRemove.Add(loc);
                    tempLocGO.Add(locGo);
                }
            }
        }

        tileHandler.HandleTiles(locToRemove);

        foreach (GameObject gameObject in markedTilesList) {
            Destroy(gameObject);
        }

        markedTilesList.Clear();
        chosenColor = Color.clear;
        tempLocGO.Clear();
        locToRemove.Clear();
        frontier.Clear();
        reached.Clear();
    }

    private bool ReachedLocation(List<Vector2> reached, Vector2 target) {
        bool reachedLocation = false;

        if (reached.Count <= 0)
            return reachedLocation;

        foreach (Vector2 loc in reached) {
            if (loc == target) {
                reachedLocation = true;
                break;
            }
        }

        return reachedLocation;
    }

    private void MarkSelectedTile(Vector2 selectedTile) {
        GameObject tile = new GameObject("Selection Marker");
        tile.transform.localScale = new Vector2(0.5f, 0.5f);

        foreach (Dictionary<Vector2, GameObject> locGo in grid.LocGO) {
            if(locGo.TryGetValue(selectedTile, out GameObject SelectedTileGO)) {
                tile.transform.position = SelectedTileGO.transform.position;
            }
        }

        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = grid.sprite;

        if(markedTilesList.Count > 0)
            markedTilesList[markedTilesList.Count - 1].GetComponent<SpriteRenderer>().color = grid.pathedTileColor;

        markedTilesList.Add(tile);

        markedTilesList[markedTilesList.Count - 1].GetComponent<SpriteRenderer>().color = grid.currentTileColor;
    }
}
