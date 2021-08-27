using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Directions for pathfinder
    public static readonly Vector2[] DIRS = new[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(1, -1),
        new Vector2(-1, -1),
        new Vector2(0, -1),
        new Vector2(-1, 0),
        new Vector2(-1, 1),
        new Vector2(0, 1)
    };

    [Header("Colors")]
    public Color[] tileColors = { 
        Color.red,
        Color.blue,
        Color.green 
    };

    public Color pathedTileColor = Color.white;
    public Color currentTileColor = Color.black;

    // Lists for keeping track of every tile
    [HideInInspector] public List<Dictionary<Vector2, GameObject>> LocGO;
    [HideInInspector] public Dictionary<GameObject, Vector2> GOLoc;
    [HideInInspector] public Dictionary<GameObject, Color> GOColor;
    [HideInInspector] public List<GameObject> GO;
    [HideInInspector] public float[,] grid;

    [Header("Grid Settings")]
    [SerializeField] private bool autofill;
    public int columns, rows;

    [Header("Tile Settings")]
    public Sprite sprite;
    [HideInInspector] public int vertical, horizontal, autoColumns, autoRows;

    void Start() {
        vertical = (int)Camera.main.orthographicSize;
        horizontal = vertical * (Screen.width / Screen.height);

        if(autofill) {
            columns = horizontal * 2;
            rows = vertical * 2;
        }

        grid = new float[columns, rows];
        GOLoc = new Dictionary<GameObject, Vector2>();
        LocGO = new List<Dictionary<Vector2, GameObject>>();
        GOColor = new Dictionary<GameObject, Color>();

        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                Color colorToAdd;
                colorToAdd = tileColors[UnityEngine.Random.Range(0, tileColors.Length)];
                SpawnTile(i, j, colorToAdd);
            }
        }
    }

    private void SpawnTile(float x, float y, Color color) {
        GameObject tile = new GameObject("x: " + x + " y: " + y);
        tile.transform.position = new Vector2(x - (horizontal - 0.5f), y - (vertical - 0.5f));
        tile.transform.localScale = new Vector2(0.9f, 0.9f);
        tile.AddComponent<BoxCollider>();

        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;

        Vector2 loc = new Vector2(x, y);

        AddTileToLists(loc, tile, color);
    }

    public void AddTileToLists(Vector2 location, GameObject tile, Color color) {
        GOLoc.Add(tile, location);
        LocGO.Add(new Dictionary<Vector2, GameObject>() { { location, tile } });
        GOColor.Add(tile, color);
        GO.Add(tile);
    }

    public void RemoveTileFromLists(Dictionary<Vector2, GameObject> locgo, KeyValuePair<Vector2, GameObject> kvp) {

        GOLoc.Remove(kvp.Value);
        LocGO.Remove(locgo);
        GOColor.Remove(kvp.Value);
        GO.Remove(kvp.Value);

    }

    public IEnumerable<Vector2> Neighbors(Vector2 id) {
        foreach (var dir in DIRS) {
            yield return new Vector2(id.x + dir.x, id.y + dir.y);
        }
    }
}
