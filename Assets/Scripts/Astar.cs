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
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Node start = new Node(new Vector2Int(startPos.x, startPos.y), null, 9999, 9999);
        Node end = new Node(new Vector2Int(endPos.x, endPos.y), null, 9999, 9999);

        Stack<Node> path = new Stack<Node>();
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        List<Node> adjacencies;
        Node current = start;
        
        openList.Add(start);

        while (openList.Count != 0 && !closedList.Exists(x => x.position == end.position))
        {
            current = openList[0];
            openList.Remove(current);
            closedList.Add(current);
            adjacencies = current.GetNeighbours(grid);

            foreach (Node n in adjacencies)
            {
                if (!closedList.Contains(n))
                {
                    if (!openList.Contains(n))
                    {
                        n.parent = current;
                        n.HScore = Mathf.Abs(n.position.x - end.position.x) + Mathf.Abs(n.position.y - end.position.y);
                        n.GScore = 1 + n.parent.GScore;
                        openList.Add(n);
                        openList = openList.OrderBy(node => node.FScore).ToList<Node>();
                    }
                }
            }
        }

        // if end not closed return null
        if (!closedList.Exists(x => x.position == end.position))
        {
            Debug.Log("Path no possible!");
            return null;
        }
        
        //Convert successfull path to vector2int
        Node temp = closedList[closedList.IndexOf(current)];
        while (temp != null)
        {
            path.Push(temp);
            temp = temp.parent;
        }
        List<Vector2Int> pathInVector = new List<Vector2Int>();
        foreach (var p in path)
        {
            pathInVector.Add(p.position);
        }
        return pathInVector;
    }
    
}


/// <summary>
/// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
/// </summary>
public class Node
{
    public Vector2Int position; //Position on the grid
    public Node parent; //Parent Node of this node
    
    public float FScore
    { //GScore + HScore
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
    
    public List<Node> GetNeighbours(Cell[,] grid)
    {
        List<Node> neighbours = new List<Node>();

        int row = position.y;
        int col = position.x;

        Cell currentCell = grid[col, row];

        if (row + 1 < grid.GetLength(0))
        {
            if (currentCell.HasWall(Wall.UP) == false)
                neighbours.Add(currentCell.neighbourUp);
        }
        if (row - 1 >= 0)
        {
            if (currentCell.HasWall(Wall.DOWN) == false)
                neighbours.Add(currentCell.neighbourDown);
        }
        if (col - 1 >= 0)
        {
            if (currentCell.HasWall(Wall.LEFT) == false)
                neighbours.Add(currentCell.neighbourLeft);
        }
        if (col + 1 < grid.GetLength(1))
        {
            if (currentCell.HasWall(Wall.RIGHT) == false)
                neighbours.Add(currentCell.neighbourRight);
        }
        return neighbours;
    }
}