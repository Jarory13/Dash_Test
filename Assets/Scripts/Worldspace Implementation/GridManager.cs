using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/**
 * Creates and manages the grid in world space. Prefabs should be set in editor
 * */
public class GridManager : MonoBehaviour
{

    public static GridManager instance;

    public TileUnit[,] tiles;
    public Player playerReference;
    public int Score;
    public int gridWidth = 10;
    public int gridHeight = 5;

    [SerializeField]
    private GameObject tileUnitPrefab;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject tablePrefab;

    [SerializeField]
    private NodeIndex[] tablePositions;

    [SerializeField]
    private float GridYOffset = 2.0f;

    private NodeIndex playerstart = new NodeIndex(1, 1);
    private string tileNameStart = "Tile(";


    private void Awake()
    {
        //Normally we'd also add a don't destroy on load here but since we've only scene we don't need to
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        GenerateGrid();
        SpawnTables();
        SpawnPlayer();
        RepositionCamera();
    }


    // Use this for initialization
    void Start()
    {
      
       
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void GenerateGrid()
    {
        Vector3 currentPosition = Vector3.zero;
        GameObject tile;

        tiles = new TileUnit[gridWidth, gridHeight];

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                tile = Instantiate(tileUnitPrefab, currentPosition, Quaternion.identity, transform) as GameObject;
                tile.name = $"{tileNameStart}{i},{j})";

                TileUnit unit = tile.GetComponent<TileUnit>();

                if (unit)
                {
                    tiles[i, j] = unit;
                    unit.myIndex.x = i;
                    unit.myIndex.y = j;
                }

                currentPosition.Set(i, j + 1, 0.0f);

            }
            currentPosition.Set(i +1, 0.0f, 0.0f);
        }
    }

    private void SpawnTables()
    {
        for (int i = 0; i < tablePositions.Length; i++)
        {
            NodeIndex index = tablePositions[i];
            TileUnit parentTile = tiles[index.x, index.y];
            Instantiate(tablePrefab, new Vector3(index.x, index.y, 1.0f), Quaternion.identity, parentTile.gameObject.transform);

            parentTile.occupied = true;
        }
    }

    private void RepositionCamera()
    {
        Camera.main.transform.position = new Vector3(gridWidth / 2, (gridHeight / 2) + GridYOffset, -10);
    }

    private void SpawnPlayer()
    {
        if (playerPrefab)
        {
            GameObject playerObject = Instantiate(playerPrefab, GetTile(playerstart).gameObject.transform.position, Quaternion.identity);
            playerReference = playerObject.GetComponent<Player>();
            playerReference.gridTransform = transform;
            playerReference.currentNodeIndex = playerstart;
        }
    }

    public void SpawnRandomSpill()
    {
        Debug.Log("Spawning spill");
    }

    public TileUnit GetTile(NodeIndex index)
    {
        return tiles[index.x, index.y];
    }
}
