using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject mazeCell;
    [SerializeField] public Vector3 mazePrefabScale = new Vector3(1, 1, 1);
    private MazeCell cell;
    [Header("Prefab Scale Restraints")]
    [SerializeField] private float minXScale = 1;
    [SerializeField] private float minYScale = 1;
    [SerializeField] private float minZScale = 1;

    [Header("Maze Dimensions")]
    [SerializeField] private int mazeWidth = 10;
    [SerializeField] private int mazeDepth = 10;
    [SerializeField] private int mazeHeight = 1;

    [Header("Boundary Cell Settings")]
    [SerializeField] Color boundaryCellColor;
    public bool changeBoundaryCellColor = false;

    [Header("Cell Wall Settings")]
    [SerializeField][Range(0f, 1f)] float minWallPercent = 0.45f;
    [SerializeField][Range(0f, 1f)] float maxIsolatedCells = 0.25f;
    public bool enableTopDownView = false;

    [Header("Maze Stats")]
    public int totalCells;
    public int boundaryCellsCount;
    public int innerCells;
    public int unvisitedCells;
    public int isolatedCellsCount;
    public int startingNumberOfWalls;
    public int currentNumberOfWalls;
    public int minNumberOfWalls;
    public int maxNumberOfIsolatedCells;
    public List<MazeCell> boundaryCells = new List<MazeCell>();
    public List<MazeCell> isolatedCells = new List<MazeCell>();
    public MazeCell[,,] mazeLevel;
    private MazeCell prevCell;
    private MazeCell currCell;

    [Header("Generation Statuses")]
    public bool isFinishedRefining = false;
    public bool isFinishedSolvingBlockages = false;
    public bool isFinishedAddingBackWalls = false;
    public bool generationInProgress = false;


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
        startingNumberOfWalls = 0;
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
                    Vector3 cellPosition = new Vector3(i, k, j);
                    mazeLevel[i, k, j] = Instantiate(cell, cellPosition, Quaternion.identity);
                    startingNumberOfWalls += mazeLevel[i, k, j].startingNumberOfCellWalls;
                }
            }
            transform.position = new Vector3(transform.position.x, (transform.position.y + mazePrefabScale.y), transform.position.z);
        }
        currentNumberOfWalls = startingNumberOfWalls;
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

        if (x == 0 || x == (mazeWidth - 1) || y == 0 || y == (mazeHeight - 1) || z == 0 || z == (mazeDepth - 1))
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
            UpdateMazeStats();
        }
    }

    private void UpdateMazeStats()
    {
        // Update the number of unvisited cells
        unvisitedCells = 0;
        foreach (MazeCell cell in mazeLevel)
        {
            if (!cell.cellWasVisited)
            {
                unvisitedCells++;
            }
        }

        // Update boundary cell count
        boundaryCellsCount = boundaryCells.Count;

        // Update isolated cell count
        isolatedCells.Clear(); // Clear the previous isolated cells list
        foreach (MazeCell cell in mazeLevel)
        {
            if (cell.remainingWalls.Count >= 4)
            {
                if (cell.hasRightWall && cell.hasLeftWall && cell.hasFrontWall && cell.hasBackWall)
                {
                    isolatedCells.Add(cell);
                }
            }
        }

        isolatedCellsCount = isolatedCells.Count;

        // Update number of walls (assuming walls are being removed or restored)
        int wallCount = 0;
        foreach (MazeCell cell in mazeLevel)
        {
            wallCount += cell.remainingWalls.Count;
        }
        currentNumberOfWalls = wallCount;
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
        foreach (MazeCell cell in mazeLevel)
        {
            if (cell.DeleteRandomRemainingWall(mazeHeight))
            {
                currentNumberOfWalls -= 1;
            }
        }

        int wallCount = 0;
        foreach (MazeCell cell in mazeLevel)
        {
            cell.RemoveBoundaryWallsFromRemainingWalls();
            wallCount += cell.remainingWalls.Count;
        }
        minNumberOfWalls = (int)((float)totalCells * minWallPercent);
        if (wallCount >= minNumberOfWalls)
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
        foreach (MazeCell cell in mazeLevel)
        {
            if (cell.remainingWalls.Count >= 4)
            {
                if (cell.hasRightWall && cell.hasLeftWall && cell.hasFrontWall && cell.hasBackWall)
                {
                    isolatedCells.Add(cell);
                    if (cell.DeleteRandomRemainingWall(mazeHeight))
                    {
                        currentNumberOfWalls -= 1;
                    }
                }
            }
        }

        isolatedCellsCount = isolatedCells.Count;
        maxNumberOfIsolatedCells = (int)(maxIsolatedCells * totalCells);

        if (isolatedCellsCount > maxNumberOfIsolatedCells && isolatedCellsCount !=0)
        {
            int randomCellIndex = Random.Range(0, isolatedCellsCount);
            isolatedCells[randomCellIndex].DeleteRandomRemainingWall(mazeHeight);
        }
        isFinishedSolvingBlockages = true;
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
