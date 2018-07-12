
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// C. Dakota C. Hurst - DH:
// GLU - Dash Demo
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelSetup : MonoBehaviour
{
    // DH: Note: BG Art and default is 900x640

    private const int kGridWidth = 10;
    private const int kGridHeight = 5;
    private const float kRoundTime = 180f;      // DH: seconds

    private Tile[,] tiles;
    private GameObject playerGO;
    private Player player;
    private GridLayoutGroup grid;
    private List<string> levels;
    private float tileWidth = -1f;
    private float tileHeight = -1f;
    private float roundTime = 0f;
    private bool playerMoving = false;
    private bool playerWillStopToClean = false;

    private AStar.HEURISTIC_METHODS astarMethodType = AStar.HEURISTIC_METHODS.MANHATTAN;
    private List<List<NodeState>> nodeStatesGrid;
    private List<NodeState> nodeStatesList;
    private int _tmpInt;
    private System.Int64 timeToFindPath; // Time for A* FindPath
    private bool randomized = false;

    // DH: Tweakable data from inspector
    //A scaler to determine how big a tile should be relative to the screen. We'll divide by this later.
    [SerializeField]
    private float GridUnitScaler = 10.0f;

    [SerializeField]
    private float GridXOffset = 100.0f;

    [SerializeField]
    private float GridYOffset = 150.0f;

    public string title = "";           // DH: What to display on the titleText
    public int gridWidth = 0;
    public int gridHeight = 0;
    public float playerSpeed = 3f;
    public NodeIndex playerStartPosition;
    public NodeIndex[] tablePositions;  // DH: Currently table positions are set to pull from the inspector

    // DH: UI Connections:
    public Text titleText;
    public Text timeText;
    public Text scoreText;
    public Button randomizeButton;
    public Button menuCloseButton;
    public GameObject canvasGO;

    // DH: Asset Connections:
    public Sprite tableSprite;
    public Sprite floorSprite;
    public Sprite spillSprite;
    public GameObject gridPrefab;
    public GameObject tileNodePrefab;
    public GameObject playerPrefab;

    private void Start()
    {
        //		Debug.Log("LevelSetup::Awake");

        timeToFindPath = 0;
        nodeStatesGrid = new List<List<NodeState>>();
        AStar.OnPathFinded += PathFound;

        // DH: NOTE: First level isn't in the randomizing.  Also any number of spills is possible, but I've only put 1 in each demo level

        SetupTitle();
        SetupGrid();
        SetupTables();
        SpawnSpill(new NodeIndex(1, 3));
        SpawnSpill(new NodeIndex(8, 1));
        StartCoroutine(SpawnPlayerWait());
        StartRound();

        SetupLevels();  // DH: string based levels for Randomizing
    }

    private void OnDestroy()
    {
        AStar.OnPathFinded -= PathFound;
    }

    private void SetupTitle()
    {
        if (titleText)
        {
            titleText.text = string.IsNullOrEmpty(title) ? "Hello World" : title;
        }
        else
        {
            Debug.LogError("No titleText !!!");
        }
        if (!timeText)
        {
            Debug.LogError("No timeText !!!");
        }
        if (!scoreText)
        {
            Debug.LogError("No scoreText !!!");
        }
        if (randomizeButton)
        {
            randomizeButton.onClick.AddListener(() => Randomize());
        }
        else
        {
            Debug.LogError("No menuCloseButton !!!");
        }
        if (menuCloseButton)
        {
            menuCloseButton.onClick.AddListener(() => MenuClose());
        }
        else
        {
            Debug.LogError("No menuCloseButton !!!");
        }
    }

    private void SetupGrid()
    {

        //we'll use this later to scal the grid accordingly;
        float CanvasWidth = 0;
        float CanvasHeight = 0;
        RectTransform gridRect = null;
        RectTransform canvasRect = null;
        if (!grid)
        {
            GameObject gridGO = Instantiate(gridPrefab, Vector3.zero, Quaternion.identity);
            grid = gridGO.GetComponent<GridLayoutGroup>();
            if (canvasGO)
            {

                canvasRect = canvasGO.GetComponent<RectTransform>();

                CanvasWidth = canvasRect.rect.width;
                CanvasHeight = canvasRect.rect.height;

                gridGO.transform.SetParent(canvasGO.transform, false);


                gridRect = gridGO.GetComponent<RectTransform>();

                ////Make sure the grid is at proper scale, sized right for the canvas and then positioned in a good location
                gridRect.localScale = Vector3.one;
                gridRect.sizeDelta = new Vector2(CanvasWidth, CanvasHeight) / GridUnitScaler;


                //Are we using the random button? If so we need to us a different calculation. I cannot figure out why this is necessary, but something is modifying the values after creation. This check stops that.
                //This may be realted to the canvas scalers but I would need more time to investigate
                gridRect.position = randomized ? (Vector3.zero -  (1.5f *Vector3.one) + new Vector3(-0.5f, 0.0f, 5.0f)) : new Vector3(canvasRect.rect.xMin - GridXOffset, canvasRect.rect.yMin - GridYOffset, 0.0f);
                //gridRect.position = canvasRect.rect.min;

            }
        }

        if (grid)
        {

            gridWidth = gridWidth > 0 ? gridWidth : kGridWidth;
            gridHeight = gridHeight > 0 ? gridHeight : kGridHeight;

            tileWidth = gridRect.rect.width / (float)gridWidth;
            tileHeight = gridRect.rect.height / (float)gridHeight;


            grid.cellSize = new Vector2(tileWidth, tileHeight);

            tiles = new Tile[gridWidth, gridHeight];

            //			Debug.Log ("LevelSetup::Setup Grid: gridWidth: " + gridWidth + " : gridHeight: " + gridHeight + " : tileWidth: " + tileWidth + " : tileHeight: " + tileHeight);

            foreach (List<NodeState> list in nodeStatesGrid)
            {
                list.Clear();
            }
            nodeStatesGrid.Clear();

            for (int y = 0; y < gridHeight; y++)
            {
                nodeStatesList = new List<NodeState>();

                for (int x = 0; x < gridWidth; x++)
                {
                    GameObject buttonGO = Instantiate(tileNodePrefab, new Vector3(gridWidth, gridHeight, 0.0f), Quaternion.identity);
                    buttonGO.name = "Tile(" + x + ", " + y + ")";
                    Button b = buttonGO.GetComponent<Button>();
                    if (b)
                    {
                        b.transform.SetParent(grid.transform, false);
                        NodeIndex nodeIndex = new NodeIndex();
                        nodeIndex.x = x;
                        nodeIndex.y = y;

                        b.onClick.AddListener(() => TileClicked(nodeIndex));

                        Tile tile = ScriptableObject.CreateInstance<Tile>();
                        tile.go = b.gameObject;
                        tile.nodeIndex = nodeIndex;

                        tiles[x, y] = tile;
                    }

                    nodeStatesList.Insert(x, buttonGO.GetComponent<NodeState>());
                }
                nodeStatesGrid.Insert(y, nodeStatesList);
            }
        }    
            
        else
        {
            Debug.LogError("No Grid !!!");
        }
    }

    private int score = -1;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            if (scoreText)
            {
                scoreText.text = "Cleaned: " + value.ToString();
            }
        }
    }

    private void SetupTables()
    {
        if (grid)
        {
            Score = 0;

            //			Debug.Log("LevelSetup::SetupTable: " + tablePositions.Length);

            int tablePositionsLength = tablePositions.Length;
            for (int i = 0; i < tablePositionsLength; i++)
            {
                Tile tile = null;
                NodeIndex nodeIndex = tablePositions[i];

                tile = tiles[nodeIndex.x, nodeIndex.y];
                if (tile)
                {
                    if (tile.go)
                    {
                        tile.type = TileType.TABLE;

                        NodeState ns = tile.go.GetComponent<NodeState>();
                        if (ns)
                        {
                            ns.state = NodeState.STATES.RED;
                        }

                        Button b = tile.go.GetComponent<Button>();
                        if (b)
                        {
                            if (tableSprite && b.image)
                            {
                                b.image.raycastTarget = false;
                                b.image.sprite = tableSprite;
                            }
                            b.onClick.RemoveAllListeners();
                            Destroy(b);
                        }
                    }
                }
            }
        }
    }

    private void SpawnPlayer()
    {
        //		Debug.Log("LevelSetup::SpawnPlayer");

        if (playerPrefab)
        {
            playerGO = Instantiate(playerPrefab);
            player = playerGO.AddComponent<Player>();
            player.speed = playerSpeed;

            RectTransform rt = playerPrefab.GetComponent<RectTransform>();
            if (rt)
            {
                rt.sizeDelta = new Vector2(tileWidth, tileHeight);
            }
            if (grid)
            {
                playerGO.transform.SetParent(canvasGO.transform, true);
                player.gridTransform = grid.transform;
            }
            playerGO.transform.SetAsLastSibling();

            player.SetPosition(playerStartPosition, GetTile(playerStartPosition).go.transform);
        }
        else
        {
            Debug.LogError("No playerPrefab !!!");
        }
    }

    private void SpawnSpill(NodeIndex ni)
    {
        Tile tile = GetTile(ni);

        if (tile)
        {
            if (tile.type != TileType.TABLE && tile.go) // DH: Don't ovveride Tables
            {
                Image tileImage = tile.go.GetComponent<Image>();

                if (tileImage && spillSprite)   // DH: ignore as spill even if content is missing, to eliminate confusion to the user.
                {
                    tileImage.sprite = spillSprite;
                    tile.type = TileType.SPILL;
                }
            }
        }
    }

    private void SpawnRandomSpill()
    {
        if (!player)
        {
            return;
        }

        int playerX = player.currentNodeIndex.x;
        int playerY = player.currentNodeIndex.y;

        List<Tile> availableTiles = new List<Tile>();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (tiles[x, y].type == TileType.FLOOR && !(tiles[x, y].nodeIndex.x == playerX && tiles[x, y].nodeIndex.y == playerY))
                {
                    availableTiles.Add(tiles[x, y]);
                }
            }
        }

        int availableTilesCount = availableTiles.Count;
        if (availableTilesCount > 0)
        {
            SpawnSpill(availableTiles[Random.Range(0, availableTilesCount)].nodeIndex);
        }
    }

    // DH: A little time is needed for the rest of the setup
    private IEnumerator SpawnPlayerWait()
    {
        yield return new WaitForSeconds(.1f);
        SpawnPlayer();
    }

    private Tile GetTile(NodeIndex ni)
    {
        return tiles[ni.x, ni.y];
    }

    private void TileClicked(NodeIndex nodeIndex)
    {
        if (playerMoving)
        {
            return;
        }

        if (player)
        {
            player.desiredEndNodeIndex = nodeIndex;
            NodeIndex playerPos = player.currentNodeIndex;

            NodeState ns = GetTile(player.currentNodeIndex).go.GetComponent<NodeState>();
            if (ns)
            {
                ns.state = NodeState.STATES.BLUE;
            }
            ns = null;

            Tile endTile = GetTile(nodeIndex);
            if (endTile)
            {
                playerWillStopToClean = endTile.type == TileType.SPILL;

                ns = endTile.go.GetComponent<NodeState>();
                if (ns)
                {
                    ns.state = NodeState.STATES.BLUE;
                }
            }
            ns = null;

            timeToFindPath = 0;
            FindPath();
        }
    }

    private void Randomize()
    {
        //		Debug.Log("LevelSetup::Randomize");

        // DH: Force to defaults in case first level created by inspector values is different
        gridWidth = kGridWidth;
        gridHeight = kGridHeight;

        StartCoroutine(RandomizeWait());

        CancelInvoke("UpdateRoundTimer");
        StartRound();
        if (titleText)
        {
            titleText.text = "Dash!!!";
        }
    }

    private IEnumerator RandomizeWait()
    {
        ClearGrid();
        ClearPlayer();

        yield return null;
        randomized = true;
        SetupGrid();

        int randLevel = Random.Range(0, levels.Count);

        //		Debug.Log("LevelSetup::RandomizeWait... levels: " + levels.Count + " : randLevel: " + randLevel);

        LoadLevel(levels[randLevel]);
    }

    void StartRound()
    {
        if (timeText != null)
        {
            roundTime = kRoundTime;
            timeText.text = "00:00";
            InvokeRepeating("UpdateRoundTimer", 0.0f, 0.01667f);
        }
    }

    void UpdateRoundTimer()
    {
        if (timeText != null)
        {
            roundTime -= Time.deltaTime;
            string minutes = Mathf.Floor(roundTime / 60).ToString("00");
            string seconds = (roundTime % 60).ToString("00");
            timeText.text = minutes + ":" + seconds;
            if (roundTime <= 0f)
            {
                if (titleText)
                {
                    titleText.text = "Round Over!";
                }
                CancelInvoke("UpdateRoundTimer");
            }
        }
    }

    private void MenuClose()
    {
        Debug.Log("LevelSetup:MenuClose");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    private void PathFound(List<Node> path)
    {
        // DH: TODO: Currently A* is flipping i & j... so needs to be flipped before moving along the path
        int pathCount = path.Count;
        for (int i = 0; i < pathCount; i++)
        {
            int tempI = path[i].nodeIndex.x;
            path[i].nodeIndex.x = path[i].nodeIndex.y;
            path[i].nodeIndex.y = tempI;
        }

        StartCoroutine(MovePlayerAlongPath(path));
    }

    private IEnumerator MovePlayerAlongPath(List<Node> path)
    {
        if (player)
        {
            playerMoving = true;

            if (path[0].nodeIndex.x == player.desiredEndNodeIndex.x && path[0].nodeIndex.y == player.desiredEndNodeIndex.y)
            {
                path.Reverse();     // DH: Make sure the path nodes go from start to end properly
            }

            int pathCount = path.Count;
            for (int i = 0; i < pathCount; i++)
            {
                Tile nextTile = GetTile(path[i].nodeIndex);

                //yield return StartCoroutine(player.MoveWait(path[i].nodeIndex, nextTile.go.transform));

                if (player)
                {
                    if (nextTile.type == TileType.SPILL)
                    {
                        if (playerWillStopToClean)
                        {
                            yield return StartCoroutine(player.CleanWait());

                            Score++;

                            Image tileImage = nextTile.go.GetComponent<Image>();

                            if (tileImage && floorSprite)
                            {
                                tileImage.sprite = floorSprite;
                                nextTile.type = TileType.FLOOR;
                            }

                            SpawnRandomSpill();
                        }
                        else
                        {
                            player.normalSpeed = false;
                        }
                    }
                }
            }
            ResetNodeStates();
            ClearAStarGridMap();
            playerMoving = false;
        }
    }

    private void ClearGrid()
    {
        if (grid && grid.gameObject)
        {
            Destroy(grid.gameObject);
        }
    }

    private void ClearPlayer()
    {
        if (player)
        {
            Destroy(player);
        }
        if (playerGO)
        {
            Destroy(playerGO);
        }
    }

    private void ResetNodeStates()
    {
        int nodeStatesGridCount = nodeStatesGrid.Count;
        for (int i = 0; i < nodeStatesGridCount; i++)
        {
            nodeStatesList = nodeStatesGrid[i];
            int nodeStatesListCount = nodeStatesList.Count;
            for (int j = 0; j < nodeStatesListCount; j++)
            {
                if (nodeStatesList[j].state != NodeState.STATES.RED)
                {
                    nodeStatesList[j].state = NodeState.STATES.WHITE;
                }
            }
        }
    }

    private void ClearAStarGridMap()
    {
        if (null != AStar.gridMap)
        {
            foreach (List<bool> list in AStar.gridMap)
            {
                list.Clear();
            }
            AStar.gridMap.Clear();
        }
        else
        {
            AStar.gridMap = new List<List<bool>>();
        }
    }

    private void FindPath()
    {
        ClearAStarGridMap();

        if (null == AStar.startIndex)
        {
            AStar.startIndex = new NodeIndex();
        }

        if (null == AStar.endIndex)
        {
            AStar.endIndex = new NodeIndex();
        }
        _tmpInt = 0;

        for (int x = 0; x < nodeStatesGrid.Count; x++)
        {
            List<bool> boolList = new List<bool>();
            for (int y = 0; y < nodeStatesGrid[0].Count; y++)
            {
                if (NodeState.STATES.BLUE == nodeStatesGrid[x][y].state)
                {
                    if (++_tmpInt > 2) return;

                    if (1 == _tmpInt)
                    {
                        AStar.startIndex.x = x;
                        AStar.startIndex.y = y;
                    }
                    else if (2 == _tmpInt)
                    {
                        AStar.endIndex.x = x;
                        AStar.endIndex.y = y;
                    }

                    boolList.Insert(y, true);
                }
                else if (NodeState.STATES.RED == nodeStatesGrid[x][y].state)
                {
                    boolList.Insert(y, false);
                }
                else// WHITE or GREEN
                {
                    boolList.Insert(y, true);
                }
            }
            AStar.gridMap.Insert(x, boolList);
        }

        if (_tmpInt != 2)
        {
            ClearAStarGridMap();
            return;
        }

        timeToFindPath = System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000;

        AStar.heuristicType = astarMethodType;
        AStar.FindPath();

        timeToFindPath = System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000 - timeToFindPath;
        //		Debug.Log("TimeToFinPath: " + timeToFindPath);
    }

    // DH: KEY: # = Empty Floor, $ = Table, P = Player, & = Spill
    // DH: NOTE: Multiple spills are allowed, I've only put 1 in each demo level
    private void LoadLevel(string level)
    {
        tablePositions = null;
        List<NodeIndex> tablePosList = new List<NodeIndex>();

        for (int y = gridHeight - 1; y > -1; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                char currentChar = level[x + ((gridHeight - y - 1) * gridWidth)];
                if (currentChar == 'P')
                {
                    playerStartPosition = new NodeIndex(x, y);
                }
                else if (currentChar == '$')
                {
                    tablePosList.Add(new NodeIndex(x, y));
                }
                else if (currentChar == '&')
                {
                    SpawnSpill(new NodeIndex(x, y));
                }
                // DH: else defaults to empty, currently no error handling if you use something other than '#'
            }
        }
        tablePositions = tablePosList.ToArray();

        SetupTables();

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        

        StartCoroutine(SpawnPlayerWait());
    }
    // DH: TODO: Expose visual level editor for designer, and save out levels/strings properly.
    private void SetupLevels()
    {
        if (levels == null)
        {
            levels = new List<string>();
        }
        else
        {
            int levelsCount = levels.Count;
            for (int i = 0; i < levelsCount; i++)
            {
                string s = levels[i];
                if (s != null)
                {
                    s = null;
                }
            }
            levels.Clear();
        }

        levels.Add(
            "###$$$$##P" +
            "##########" +
            "#$$$$$$$$#" +
            "##########" +
            "&##$$$$###");
        levels.Add(
            "##$$$$####" +
            "&########$" +
            "$$$$$##$$$" +
            "##$#######" +
            "P###$$$$##");
        levels.Add(
            "P#########" +
            "####$$####" +
            "#$$$$$$$$#" +
            "####$$####" +
            "#########&");
        levels.Add(
            "$$########" +
            "###&$$#$$$" +
            "#$$#$#####" +
            "#$##$###$#" +
            "########$P");
        levels.Add(
            "#$####$###" +
            "&$#$####$#" +
            "#$#$#$##$#" +
            "###$#$##P#" +
            "#####$####");
        levels.Add(
            "##$$$$####" +
            "#########$" +
            "P###$$$$##" +
            "##$#######" +
            "##$$$$##&#");
        levels.Add(
            "##$$######" +
            "##$$##$$##" +
            "##$$##$$P#" +
            "######$$##" +
            "#&####$$##");
        levels.Add(
            "##P#######" +
            "#$$$$$$$$#" +
            "######&###" +
            "#$$$$$$$$#" +
            "##########");
        levels.Add(
            "$#######&#" +
            "$$$$##$$$$" +
            "#########$" +
            "$$$##$$$$$" +
            "#######P##");
        levels.Add(
            "##########" +
            "##$$$$$$##" +
            "###$$$$###" +
            "#P#$$$$#&#" +
            "###$$$$###");

        Debug.Log("LevelSetup::SetupLevels... levels.Count: " + levels.Count);
    }
}
