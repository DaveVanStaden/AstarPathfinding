using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    ///private Vector2Int startPos = new Vector2Int(0, 0);
    ///private Vector2Int endPos = new Vector2Int(8, 8);
    private MazeGeneration m_Maze;
    private int gridSizeX, gridSizeY;
    private Vector2 gridWorldSize;
    private float nodeRadius = 0.5f;
    private float nodeDiameter;
    private Node[,] nodeGrid;
    public List<Vector2Int> path;
    /// <param name="endPos"></param>
    ///public List<Vector2Int> grid = new List<Vector2Int>();
    /// <param name="grid"></param>
    /// <returns></returns>

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Node startNode = new Node();
        startNode.position = startPos;
        startNode.HScore = GetDistance(startPos, endPos, grid);
        startNode.GScore = 0;

        List<Node> openSetNodes = new List<Node>();
        HashSet<Node> closedSetNodes = new HashSet<Node>();
        openSetNodes.Add(startNode);

        nodeGrid = new Node[grid.GetLength(0), grid.GetLength(1)];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Node node = new Node();
                nodeGrid[x, y] = node;
                nodeGrid[x, y].position = grid[x, y].gridPosition;
                nodeGrid[x, y].walls = grid[x, y].walls;
                nodeGrid[x, y].HScore = GetDistance(nodeGrid[x,y].position, endPos, grid);
                nodeGrid[x, y].GScore = int.MaxValue;
            }
        }
        while (openSetNodes.Count > 0)
        {
            Node currentNode = openSetNodes[0];
            for (int i = 1; i < openSetNodes.Count; i++)
            {
                if (openSetNodes[i].FScore < currentNode.FScore || openSetNodes[i].FScore == currentNode.FScore)
                {
                    if (openSetNodes[i].HScore < currentNode.HScore)
                        currentNode = openSetNodes[i];
                }
            }

            openSetNodes.Remove(currentNode);
            closedSetNodes.Add(currentNode);

            if (currentNode.position == endPos)
            {
                RetracePath(startNode, currentNode);
                return path;
            }
            foreach (Node neighbour in GetNeighbours(currentNode, nodeGrid))
            {
                int dstX = Mathf.Abs(currentNode.position.x - neighbour.position.x);
                int dstY = Mathf.Abs(currentNode.position.y - neighbour.position.y);

                if (closedSetNodes.Contains(neighbour))
                    continue;
                int newMovementCostToNeighbour = currentNode.GScore + GetDistance(currentNode.position, neighbour.position, grid);
                if (newMovementCostToNeighbour < neighbour.GScore || !openSetNodes.Contains(neighbour))
                {
                    //grid[neighbour.position.x, neighbour.position.y]
                    neighbour.GScore = newMovementCostToNeighbour;
                    neighbour.parent = currentNode;
                    if (!openSetNodes.Contains(neighbour))
                        openSetNodes.Add((neighbour));
                }
            }
        }

        return null;
    }

    public void RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode.position != startNode.position)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Add(startNode.position);
        path.Reverse();

        this.path = path;
    }
    public List<Node> GetNeighbours(Node node, Node[,] grid)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {

                if (x == 0 && y == 0 || Mathf.Abs(x) == Mathf.Abs(y))
                    continue;
                int checkX = node.position.x + x;
                int checkY = node.position.y + y;
                //|| (nodeGrid[node.position.x, node.position.y-y].walls & Wall.DOWN) != 0 && y == -1

                if (checkX >= 0 && checkX < nodeGrid.GetLength(0) && checkY >= 0 && checkY < nodeGrid.GetLength(1))
                {
                    Node tempNode = nodeGrid[checkX, checkY];
                    if ((node.walls & Wall.LEFT) != 0 && x == -1 || (tempNode.walls & Wall.RIGHT) != 0 && x == -1)
                    {
                        continue;
                    }            
                    if ((node.walls & Wall.RIGHT) != 0 && x == 1 || (tempNode.walls & Wall.LEFT) != 0 && x == 1)
                    {
                        continue;
                    }            
                    if ((node.walls & Wall.UP) != 0 && y == 1 || (tempNode.walls & Wall.DOWN) != 0 && y == 1)
                    {
                        continue;
                    }            
                    if ((node.walls & Wall.DOWN) != 0 && y == -1 || (tempNode.walls & Wall.UP) != 0 && y == -1)
                    {
                        continue;
                    }
                    neighbours.Add(tempNode);
                }
            }
        }

        return neighbours;
    }

    /*int GetDistance(Vector2Int posOne, Vector2Int posTwo)
    {
        return Mathf.RoundToInt(Vector2Int.Distance(posOne, posTwo));
    }*/

    int GetDistance(Vector2Int cellA, Vector2Int cellB, Cell[,] grid)
    {
        int dstX = Mathf.Abs(cellA.x - cellB.x);
        int dstY = Mathf.Abs(cellA.y - cellB.y);
        int walls = grid[cellA.x, cellA.y].GetNumWalls();
        if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }



    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node
        public int gridX, gridY;
        public Wall walls;
        public int FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public int GScore; //Current Travelled Distance
        public int HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore, int _gridX, int _gridY)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
            gridX = _gridX;
            gridY = _gridY;
        }
    }