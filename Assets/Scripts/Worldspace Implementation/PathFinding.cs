using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    [SerializeField]
    private AStar.HEURISTIC_METHODS astarMethodType = AStar.HEURISTIC_METHODS.MANHATTAN;
    private Player player;


    // Use this for initialization
    void Start()
    {
        player = GridManager.instance.playerReference;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PathFound(List<Node> path)
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

    public void FindPath(NodeIndex start, NodeIndex end)
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

        for (int x = 0; x < GridManager.instance.tiles.GetLength(0); x++)
        {
            List<bool> boolList = new List<bool>();
            
            for (int y = 0; y < GridManager.instance.tiles.GetLength(1); y++)
            {
                TileUnit unit = GridManager.instance.tiles[x, y];
                if (unit.occupied)
                {
                    boolList.Insert(y, false);
                }
                else
                {
                    boolList.Insert(y, true);
                }
            }
            AStar.gridMap.Insert(x, boolList);
        }

        //timeToFindPath = System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000;

        AStar.heuristicType = astarMethodType;
        bool pathfound = AStar.FindPath();
        Debug.Log(pathfound);

        //timeToFindPath = System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000 - timeToFindPath;
        //		Debug.Log("TimeToFinPath: " + timeToFindPath);
    }

    private IEnumerator MovePlayerAlongPath(List<Node> path)
    {
        if (player)
        {
            player.isMoving = true;

            if (path[0].nodeIndex.x == player.desiredEndNodeIndex.x && path[0].nodeIndex.y == player.desiredEndNodeIndex.y)
            {
                path.Reverse();     // DH: Make sure the path nodes go from start to end properly
            }

            int pathCount = path.Count;
            for (int i = 0; i < pathCount; i++)
            {
                TileUnit nextTile = GridManager.instance.GetTile(path[i].nodeIndex);

                //yield return StartCoroutine(player.MoveWait(path[i].nodeIndex, nextTile.transform));

                if (player)
                {
                    if (nextTile.conatinsSplill)
                    {
                        if (player.playerWillStopToClean)
                        {
                            yield return StartCoroutine(player.CleanWait());

                            GridManager.instance.Score++;

                            nextTile.ClearSpill();

                            GridManager.instance.SpawnRandomSpill();
                        }
                        else
                        {
                            player.normalSpeed = false;
                        }
                    }
                }
            }

            ClearAStarGridMap();
            player.isMoving = false;
        }
    }
}
