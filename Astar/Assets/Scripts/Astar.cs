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
    private MazeGeneration grid;
    /// <param name="endPos"></param>
    ///public List<Vector2Int> grid = new List<Vector2Int>();
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Cell startCell = grid[startPos.x,startPos.y];
        Node startNode = new Node();
        startNode.position = startCell.gridPosition;
        Cell endCell = grid[endPos.x,endPos.y];
        Node endNode = new Node();
        endNode.position = endCell.gridPosition;
        List<Vector2Int> finalPathInt = new List<Vector2Int>();
        List<Node> finalPathNode = new List<Node>();

        List<Node> OpenList = new List<Node>();
        HashSet<Node> ClosedList = new HashSet<Node>();

        OpenList.Add(startNode);
        while (OpenList.Count > 0)
        {
    
            Node CurrentNode = OpenList[0];
            for (int i = 0; i < OpenList.Count; i++)
            {
                //if (OpenList[i].FScore <= CurrentNode.FScore && OpenList[i].HScore <= CurrentNode.HScore)
                if (OpenList[i].FScore <= CurrentNode.FScore && OpenList[i].HScore <= CurrentNode.HScore)
                {
                    CurrentNode = OpenList[i];
                }
            }

            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode == endNode)
            {
                finalPathNode = GetFinalPath(startNode, endNode);
                for (int i = 0; i < finalPathNode.Count; i++)
                {
                    finalPathInt.Add(finalPathNode[i].position);
                }
        
            }
        }
        return finalPathInt;
    }

    public List<Node> GetFinalPath(Node startNode, Node endNode)
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = endNode;
        while (CurrentNode != startNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.parent;
        }

        FinalPath.Reverse();
        return FinalPath;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
