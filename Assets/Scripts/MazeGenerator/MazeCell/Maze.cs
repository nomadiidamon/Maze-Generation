using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int totalCells;
    public MazeCell startingCell;
    public MazeCell endingCell;
    public bool alterStartAndEndColor = true;
    public Color startingCellColor = Color.green;
    public Color endingCellColor = Color.red;
    public float trapCellChance = 0.1f;
    public float treasureCellChance = 0.1f;
    public float enemyCellChance = 0.1f;
    public float powerUpCellChance = 0.1f;
    public float healthCellChance = 0.1f;
    public float ammoCellChance = 0.1f;
    public float weaponCellChance = 0.1f;
    public float armorCellChance = 0.1f;
    public float keyCellChance = 0.1f;
    public float doorCellChance = 0.1f;
    public float breakableSurfaceChance = 0.1f;
    public bool startOnBoundary = true;

    public List<MazeCell> cells = new List<MazeCell>();
    public List<MazeCell> boudaryCells = new List<MazeCell>();
    public List<MazeCell> innerCells = new List<MazeCell>();
    public List<MazeCell> isolatedCells = new List<MazeCell>();
    public List<MazeCell> specialCells = new List<MazeCell>();

    public List<MazeCell> trapCells = new List<MazeCell>();

    public bool levelIsReady = false;

    public static Maze CreateMaze(int totalCells, float trapCellChance = 0.1f, float treasureCellChance = 0.1f, float enemyChance = 0.1f,
        float powerUpChance = 0.1f, float healthCellChance = 0.1f, float ammoCellChance = 0.1f, float weaponCellChance = 0.1f,
        float armorCellChance = 0.1f, float keyCellChance = 0.1f, float doorCellChance = 0.1f, float breakableSurfaceChance = 0.1f, Color? startCell = null, Color? endCell = null, bool startOnBoundary = true)
    {
        GameObject mazeGameObject = new GameObject("Maze");
        Maze maze = mazeGameObject.AddComponent<Maze>();
        maze.totalCells = totalCells;
        maze.trapCellChance = trapCellChance;
        maze.treasureCellChance = treasureCellChance;
        maze.enemyCellChance = enemyChance;
        maze.powerUpCellChance = powerUpChance;
        maze.healthCellChance = healthCellChance;
        maze.ammoCellChance = ammoCellChance;
        maze.weaponCellChance = weaponCellChance;
        maze.armorCellChance = armorCellChance;
        maze.keyCellChance = keyCellChance;
        maze.doorCellChance = doorCellChance;
        maze.breakableSurfaceChance = breakableSurfaceChance;
        maze.startingCellColor = startCell ?? Color.green;
        maze.endingCellColor = endCell ?? Color.red;
        maze.startOnBoundary = startOnBoundary;
        return maze;
    }

    private void Update()
    {
        if (levelIsReady)
        {
            return;
        }
        else
        {

        }
    }

    public bool SignalToStart(bool start)
    {
        if (start)
        {
            InitializeSpecialCells();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ConfirmLevelIsReady()
    {
        return levelIsReady;
    }

    public void InitializeSpecialCells()
    {
        specialCells.Clear();
        PickStartAndEndCells();
        InitializeInnerCells();
        InitializeTrapCells();
        InitializeIsolatedCells();

    }
    public void PickStartAndEndCells()
    {
        startingCell = GetRandomCell(startOnBoundary);
        endingCell = GetRandomCell(startOnBoundary);
        while (endingCell == startingCell)
        {
            endingCell = GetRandomCell(startOnBoundary);
        }

        if (alterStartAndEndColor)
        {
            foreach (GameObject wall in startingCell.remainingWalls)
            {
                wall.GetComponentInChildren<MeshRenderer>().material.color = startingCellColor;
            }
            foreach (GameObject wall in endingCell.remainingWalls)
            {
                wall.GetComponentInChildren<MeshRenderer>().material.color = endingCellColor;
            }
        }
        specialCells.Add(startingCell);
        specialCells.Add(endingCell);
    }

    public void InitializeInnerCells()
    {
        if (cells.Count == 0)
        {
            return;
        }
        else
        {
            innerCells.Clear();
            foreach (MazeCell cell in cells)
            {
                if (!cell.isBoundaryCell)
                {
                    innerCells.Add(cell);
                }
            }
        }
    }

    public void InitializeTrapCells()
    {
        trapCells.Clear();
        foreach (MazeCell cell in cells)
        {
            if (Random.value < trapCellChance)
            {
                trapCells.Add(cell);
                specialCells.Add(cell);
            }
        }
    }

    public void InitializeIsolatedCells()
    {
        isolatedCells.Clear();

        //// loop through all of the cells
        //foreach (MazeCell cell in cells)
        //{
        //    //start a count for each cell
        //    int count = 0;
        //    // loop through all of the neighboring cells
        //    foreach (MazeCell neighbor in cell.neighboringCells)
        //    {
        //        if (cell.hasLeftWall && neighbor.hasRightWall && (Vector3.Distance(cell.LeftWall.transform.position, neighbor.RightWall.transform.position) <= 0.2))
        //        {
        //            count++;
        //        }
        //        if (cell.hasRightWall && neighbor.hasLeftWall && (Vector3.Distance(cell.RightWall.transform.position, neighbor.LeftWall.transform.position) <= 0.2))
        //        {
        //            count++;
        //        }
        //        if (cell.hasFrontWall && neighbor.hasBackWall && (Vector3.Distance(cell.FrontWall.transform.position, neighbor.BackWall.transform.position) <= 0.2))
        //        {
        //            count++;
        //        }
        //        if (cell.hasBackWall && neighbor.hasFrontWall && (Vector3.Distance(cell.BackWall.transform.position, neighbor.FrontWall.transform.position) <= 0.2))
        //        {
        //            count++;
        //        }
        //        cell.sharedWallCount = count;

        //        // if the current cell has a left, right, front, and back wall around it, add it to the isolated cells list
        //        if (count == 4)
        //        {
        //            isolatedCells.Add(cell);
        //        }
        //    }
        //    count = 0;
        //}

        // loop through all of the cells
        foreach (MazeCell cell in cells)
        {
            //start a count for each cell
            List<GameObject> leftWalls = new List<GameObject>();
            List<GameObject> rightWalls = new List<GameObject>();
            List<GameObject> frontWalls = new List<GameObject>();
            List<GameObject> backWalls = new List<GameObject>();
            List<GameObject> sharedWalls = new List<GameObject>();

            // loop through all of the neighboring cells
            foreach (MazeCell neighbor in cell.neighboringCells)
            {
                if (neighbor.hasLeftWall)
                {
                    leftWalls.Add(neighbor.LeftWall);
                }
                if (neighbor.hasRightWall)
                {
                    rightWalls.Add(neighbor.RightWall);
                }
                if (neighbor.hasFrontWall)
                {
                    frontWalls.Add(neighbor.FrontWall);
                }
                if (neighbor.hasBackWall)
                {
                    backWalls.Add(neighbor.BackWall);
                }
            }
            foreach (GameObject wall in leftWalls)
            {
                if (cell.hasLeftWall && (Vector3.Distance(cell.LeftWall.transform.position, wall.transform.position) <= 0.2))
                {
                    sharedWalls.Add(wall);
                }
            }
            foreach (GameObject wall in rightWalls)
            {
                if (cell.hasRightWall && (Vector3.Distance(cell.RightWall.transform.position, wall.transform.position) <= 0.2))
                {
                    sharedWalls.Add(wall);
                }
            }
            foreach (GameObject wall in frontWalls)
            {
                if (cell.hasFrontWall && (Vector3.Distance(cell.FrontWall.transform.position, wall.transform.position) <= 0.2))
                {
                    sharedWalls.Add(wall);
                }
            }
            foreach (GameObject wall in backWalls)
            {
                if (cell.hasBackWall && (Vector3.Distance(cell.BackWall.transform.position, wall.transform.position) <= 0.2))
                {
                    sharedWalls.Add(wall);
                }
            }
            if (sharedWalls.Count == 4)
            {
                isolatedCells.Add(cell);
            }
        }

    }

    public MazeCell GetRandomCell(bool isBoundaryCell)
    {
        if (isBoundaryCell)
        {
            return boudaryCells[Random.Range(0, boudaryCells.Count)];
        }
        else
        {
            return cells[Random.Range(0, cells.Count)];
        }
    }
}

