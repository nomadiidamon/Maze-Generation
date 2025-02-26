using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] GameObject mazeCell;
    private MazeCell cell;
    [SerializeField] private int mazeWidth = 10;
    [SerializeField] private int mazeDepth = 10;
    [SerializeField] private int mazeHeight = 1;
    [SerializeField] private float minXScale = 1;
    [SerializeField] private float minYScale = 1;
    [SerializeField] private float minZScale = 1;
    [SerializeField] public Vector3 mazePrefabScale = new Vector3(1, 1, 1);
    [SerializeField] Color boundaryCellColor;
    [SerializeField][Range(0f, 0.45f)] float minWallPercent = 0.45f;
    [SerializeField][Range(0f, 0.5f)] float maxIsolatedCells = 0.25f;

    public bool changeBoundaryCellColor = false;
    public bool enableTopDownView = false;
    public int totalCells;
    public List<MazeCell> boundaryCells = new List<MazeCell>();
    public int boundaryCellsCount;
    public int innerCells;
    public int unvisitedCells;
    private MazeCell prevCell;
    private MazeCell currCell;
    public bool generationInProgress = false;
    public MazeCell[,,] mazeLevel;
    public bool isFinishedRefining = false;
    public bool isFinishedSolvingBlockages = false;
    public bool isFinishedAddingBackWalls = false;


    private void Start()
    {
        Initialize();
        StartMazeGeneration();
        if (enableTopDownView)
        {
            foreach (MazeCell cell in mazeLevel)
            {
                cell.DestroyCeiling();
            }
        }
    }

    public void Initialize()
    {
        totalCells = (mazeWidth * mazeHeight * mazeDepth);
        unvisitedCells = totalCells;
        if (mazePrefabScale.x < minXScale || mazePrefabScale.y < minYScale || mazePrefabScale.z < minZScale)
        {
            if (mazePrefabScale.x < 1)
            {
                mazePrefabScale.x = minXScale;
                Debug.LogError("Maze prefab X scale must be greater than 1");
            }
            if (mazePrefabScale.y < 0.5)
            {
                mazePrefabScale.y = minYScale;
                Debug.LogError("Maze prefab Y scale must be greater than 0.5");
            }
            if (mazePrefabScale.z < 1)
            {
                mazePrefabScale.z = minZScale;
                Debug.LogError("Maze prefab Z scale must be greater than 1");
            }
        }
        mazeCell.transform.localScale = mazePrefabScale;
        cell = mazeCell.GetComponent<MazeCell>();
        mazeLevel = new MazeCell[mazeWidth, mazeHeight, mazeDepth];
        for (int k = 0; k < mazeHeight; k++)
        {
            for (int i = 0; i < mazeWidth; i++)
            {
                for (int j = 0; j < mazeDepth; j++)
                {
                    Vector3 cellPosition = new Vector3(i * mazePrefabScale.x, k * mazePrefabScale.y, j * mazePrefabScale.z);
                    mazeLevel[i, k, j] = Instantiate(cell, cellPosition, Quaternion.identity);
                }
            }
            transform.position = new Vector3(transform.position.x, (transform.position.y + mazePrefabScale.y), transform.position.z);
        }
        MarkBoundaryCells();
    }

    public void MarkBoundaryCells()
    {
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeDepth; j++)
            {
                for (int k = 0; k < mazeHeight; k++)
                {
                    if (IsBoundaryCell(mazeLevel[i, k, j]))
                    {
                        if (mazeLevel[i, k, j].isBoundaryCell)
                        {
                            boundaryCells.Add(mazeLevel[i, k, j]);
                        }
                    }
                }
            }
        }
    }

    public bool IsBoundaryCell(MazeCell cell)
    {
        int x = (int)(cell.transform.position.x);
        int y = (int)(cell.transform.position.y);
        int z = (int)(cell.transform.position.z);

        bool isBoundary = false;

        if (x == 0 || x == (mazeWidth - 1) * mazePrefabScale.x || y == 0 || y == (mazeHeight - 1) * mazePrefabScale.y || z == 0 || z == (mazeDepth - 1) * mazePrefabScale.z)
        {
            isBoundary = true;
            MarkBoundaryWalls(cell, x, y, z);
        }

        return isBoundary;
    }


    public void MarkBoundaryWalls(MazeCell cell, int x, int y, int z)
    {


        // Mark boundaries based on the position of the cell, taking scaling into account
        if (x <= 0)
        {
            cell.boundaryWalls.Add(cell.RightWall);
            cell.RightWall.SetActive(true);
            cell.RightWall.GetComponentInChildren<Renderer>().material.color = boundaryCellColor;
        }
        if (x >= (mazeWidth - 1) * mazePrefabScale.x)
        {
            cell.boundaryWalls.Add(cell.LeftWall);
            cell.LeftWall.SetActive(true);
            cell.LeftWall.GetComponentInChildren<Renderer>().material.color = boundaryCellColor;
        }
        if (z <= 0)
        {
            cell.boundaryWalls.Add(cell.BackWall);
            cell.BackWall.SetActive(true);
            cell.BackWall.GetComponentInChildren<Renderer>().material.color = boundaryCellColor;
        }
        if (z >= (mazeDepth - 1) * mazePrefabScale.z)
        {
            cell.boundaryWalls.Add(cell.FrontWall);
            cell.FrontWall.SetActive(true);
            cell.FrontWall.GetComponentInChildren<Renderer>().material.color = boundaryCellColor;
        }

        if (mazeHeight > 1)
        {
            if (y <= 0)
            {
                cell.boundaryWalls.Add(cell.Floor);
                cell.Floor.SetActive(true);
                cell.Floor.GetComponentInChildren<Renderer>().material.color = boundaryCellColor;
            }
            if (y >= (mazeHeight - 1) * mazePrefabScale.y)
            {
                cell.boundaryWalls.Add(cell.Ceiling);
                cell.Ceiling.SetActive(true);
                cell.Ceiling.GetComponentInChildren<Renderer>().material.color = boundaryCellColor;
            }
        }

        cell.isBoundaryCell = (cell.boundaryWalls.Count > 0);
    }

    private void Update()
    {
        if (generationInProgress)
        {
            GenerateStep();
        }
        else
        {
            if (isFinishedRefining == false)
            {
                if (!RefineMaze())
                {
                    RefineMaze();
                }
            }
        }
        if (isFinishedRefining && !isFinishedSolvingBlockages)
        {
            ReduceBlockages();
        }
        if (isFinishedSolvingBlockages && !isFinishedAddingBackWalls)
        {
            AddBackWalls();
        }

    }

    private void GenerateStep()
    {
        MazeCell nextCell = GetNextUnvisitedNeighbor(currCell);

        if (nextCell != null)
        {
            ClearWalls(prevCell, currCell);

            prevCell = currCell;
            currCell = nextCell;
            currCell.Visit();
            unvisitedCells -= 1;
        }
        else
        {
            foreach (MazeCell cell in mazeLevel)
            {
                if (cell.cellWasVisited == true && unvisitedCells == 0)
                {
                    innerCells = (mazeWidth - 2) * (mazeDepth - 2);
                    boundaryCellsCount = boundaryCells.Count;
                    if (cell.isBoundaryCell == false)
                    {
                        boundaryCells.Remove(cell);
                    }
                    Debug.Log("All cells have been visited");
                    generationInProgress = false;
                    return;
                }
            }

            Debug.Log("Current Cell has no unvisted neighbors. Finding remaining unvisited cells...");
            for (int i = 0; i < mazeWidth; i++)
            {
                for (int j = 0; j < mazeDepth; j++)
                {
                    for (int k = 0; k < mazeHeight; k++)
                    {
                        if ((mazeLevel[i, k, j].cellWasVisited == false))
                        {
                            mazeLevel[i, k, j].Visit();
                            unvisitedCells -= 1;
                        }
                    }
                }
            }

        }
    }

    public bool RefineMaze()
    {
        List<MazeCell> innerCells = new List<MazeCell>();

        foreach (MazeCell cell in mazeLevel)
        {
            cell.DeleteRandomRemainingWall(mazeHeight);
        }



        int wallCount = 0;
        foreach (MazeCell cell in mazeLevel)
        {
            cell.RemoveBoundaryWallsFromRemainingWalls();
            foreach (GameObject wall in cell.remainingWalls)
            {
                wallCount += cell.numOfWalls;
            }
        }

        if (wallCount <= (int)((float)totalCells) / minWallPercent)
        {
            isFinishedRefining = true;
            Debug.Log("Maze has been refined");
        }

        return isFinishedRefining;
    }

    public void ReduceBlockages()
    {
        isFinishedSolvingBlockages = false;

        // get the isolated cells
        List<MazeCell> isolatedCells = new List<MazeCell>();
        foreach (MazeCell cell in mazeLevel)
        {
            if (cell.remainingWalls.Count == cell.numOfWalls)
            {
                isolatedCells.Add(cell);
                cell.DeleteRandomRemainingWall(mazeHeight);
            }
        }

        // calculate the number of isolated cells to remove
        int numIsolatedCellsToRemove = (int)(maxIsolatedCells * totalCells);

        // select a random isolated cell
        int randomCellIndex = Random.Range(0, isolatedCells.Count);

        // if the cell is isolated, remove a random wall
        isolatedCells[randomCellIndex].DeleteRandomRemainingWall(mazeHeight);


        isFinishedSolvingBlockages = true;
    }

    public void AddBackWalls()
    {
        isFinishedAddingBackWalls = false;


        foreach (MazeCell cell in mazeLevel)
        {
            //cell.AddBackWall();
        }




        isFinishedAddingBackWalls = true;
    }

    private void StartMazeGeneration()
    {
        prevCell = null;
        currCell = mazeLevel[0, 0, 0]; // Start from the first cell
        generationInProgress = true;
        currCell.Visit();
        unvisitedCells -= 1;
        prevCell = currCell;
    }

    private MazeCell GetNextUnvisitedNeighbor(MazeCell currCell)
    {
        var unvistedCells = GetUnvisitedCells(currCell);
        int max = unvistedCells.Count();
        return unvistedCells.OrderBy(_ => Random.Range(1, max)).FirstOrDefault();

    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currCell)
    {

        int x = (int)(currCell.transform.position.x);
        int y = (int)(currCell.transform.position.y);
        int z = (int)(currCell.transform.position.z);

        if ((x + 1) < mazeWidth)
        {
            var cellToRight = mazeLevel[x + 1, y, z];
            if (cellToRight.cellWasVisited == false)
            {
                yield return cellToRight;
            }
        }
        if ((x - 1) >= 0)
        {
            var cellToLeft = mazeLevel[x - 1, y, z];
            if (cellToLeft.cellWasVisited == false)
            {
                yield return cellToLeft;
            }
        }


        if ((z + 1) < mazeDepth)
        {
            var cellToFront = mazeLevel[x, y, z + 1];
            if (cellToFront.cellWasVisited == false)
            {
                yield return cellToFront;
            }
        }
        if ((z - 1) >= 0)
        {
            var cellToBack = mazeLevel[x, y, z - 1];
            if (cellToBack.cellWasVisited == false)
            {
                yield return cellToBack;
            }
        }

        if (mazeHeight > 1)
        {
            if (y + 1 < mazeHeight)
            {
                var cellToUp = mazeLevel[x, y + 1, z];
                if (cellToUp.cellWasVisited == false)
                {
                    yield return cellToUp;
                }
            }
            if (y - 1 >= 0)
            {
                var cellToDown = mazeLevel[x, y - 1, z];
                if (cellToDown.cellWasVisited == false)
                {
                    yield return cellToDown;
                }
            }
        }

        int randWidth = Random.Range(0, mazeWidth);
        int randHeight = Random.Range(0, mazeHeight);
        int randDepth = Random.Range(0, mazeDepth);

        if (mazeLevel[randWidth, randHeight, randDepth].cellWasVisited == false)
        {
            var cellToVisit = mazeLevel[randWidth, randHeight, randDepth];
            yield return cellToVisit;
        }

    }

    private void ClearWalls(MazeCell prevCell, MazeCell currCell)
    {
        if (prevCell == null) { return; }

        int prevX = (int)(prevCell.transform.position.x);
        int prevY = (int)(prevCell.transform.position.y);
        int prevZ = (int)(prevCell.transform.position.z);
        int currX = (int)(currCell.transform.position.x);
        int currY = (int)(currCell.transform.position.y);
        int currZ = (int)(currCell.transform.position.z);

        // Check the difference in positions and remove walls accordingly
        if (prevX < currX && prevCell.hasRightWall && !prevCell.boundaryWalls.Contains(prevCell.RightWall))
        {
            prevCell.DestroyRightWall();

            if (!currCell.boundaryWalls.Contains(currCell.LeftWall))
            {
                currCell.DestroyLeftWall();
            }
        }
        else if (prevX > currX && prevCell.hasLeftWall && !prevCell.boundaryWalls.Contains(prevCell.LeftWall))
        {
            prevCell.DestroyLeftWall();

            if (!currCell.boundaryWalls.Contains(currCell.RightWall))
            {
                currCell.DestroyRightWall();
            }
        }
        else if (prevZ < currZ && prevCell.hasFrontWall && !prevCell.boundaryWalls.Contains(prevCell.FrontWall))
        {
            prevCell.DestroyFrontWall();

            if (!currCell.boundaryWalls.Contains(currCell.BackWall))
            {
                currCell.DestroyBackWall();
            }
        }
        else if (prevZ > currZ && prevCell.hasBackWall && !prevCell.boundaryWalls.Contains(prevCell.BackWall))
        {
            prevCell.DestroyBackWall();

            if (!currCell.boundaryWalls.Contains(currCell.FrontWall))
            {
                currCell.DestroyFrontWall();
            }
        }
        else if (prevY < currY && prevCell.hasCeiling && !prevCell.boundaryWalls.Contains(prevCell.Ceiling))
        {
            prevCell.DestroyCeiling();

            if (!currCell.boundaryWalls.Contains(currCell.Floor))
            {
                currCell.DestroyFloor();
            }
        }
        else if (prevY > currY && prevCell.hasFloor && !prevCell.boundaryWalls.Contains(prevCell.Floor))
        {
            prevCell.DestroyFloor();

            if (!currCell.boundaryWalls.Contains(currCell.Ceiling))
            {
                currCell.DestroyCeiling();
            }
        }
    }

}
