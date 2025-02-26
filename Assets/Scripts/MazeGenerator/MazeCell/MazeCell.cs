using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    /// The Left side wall of the maze cell
    [SerializeField] public GameObject LeftWall;
    public bool hasLeftWall { get; set; }
    public void DestroyLeftWall()
    {
        hasLeftWall = false; LeftWall.SetActive(false);
        remainingWalls.Remove(LeftWall);
        destroyedWalls.Add(LeftWall);
    }
    public void RestoreLeftWall()
    {
        if (hasLeftWall) return;
        hasLeftWall = true; LeftWall.SetActive(true);
        remainingWalls.Add(LeftWall);
        destroyedWalls.Remove(LeftWall);
    }

    /// The Right side wall of the maze cell
    [SerializeField] public GameObject RightWall;
    public bool hasRightWall { get; private set; }
    public void DestroyRightWall()
    {
        hasRightWall = false; RightWall.SetActive(false);
        remainingWalls.Remove(RightWall);
        destroyedWalls.Add(RightWall);
    }
    public void RestoreRightWall()
    {
        if (hasRightWall) return;
        hasRightWall = true; RightWall.SetActive(true);
        remainingWalls.Add(RightWall);
        destroyedWalls.Remove(RightWall);
    }

    /// The Front side wall of the maze cell
    [SerializeField] public GameObject FrontWall;
    public bool hasFrontWall { get; private set; }
    public void DestroyFrontWall()
    {
        hasFrontWall = false; FrontWall.SetActive(false);
        remainingWalls.Remove(FrontWall);
        destroyedWalls.Add(FrontWall);
    }
    public void RestoreFrontWall()
    {
        if (hasFrontWall) return;
        hasFrontWall = true; FrontWall.SetActive(true);
        remainingWalls.Add(FrontWall);
        destroyedWalls.Remove(FrontWall);
    }

    /// The Back side wall of the maze cell
    [SerializeField] public GameObject BackWall;
    public bool hasBackWall { get; private set; }
    public void DestroyBackWall()
    {
        hasBackWall = false; BackWall.SetActive(false);
        remainingWalls.Remove(BackWall);
        destroyedWalls.Add(BackWall);
    }
    public void RestoreBackWall()
    {
        if (hasBackWall) return;
        hasBackWall = true; BackWall.SetActive(true);
        remainingWalls.Add(BackWall);
        destroyedWalls.Remove(BackWall);
    }

    /// The Ceiling of the maze cell
    [SerializeField] public GameObject Ceiling;
    public bool hasCeiling { get; private set; }
    public void DestroyCeiling()
    {
        hasCeiling = false; Ceiling.SetActive(false);
        remainingWalls.Remove(Ceiling);
        destroyedWalls.Add(Ceiling);
    }
    public void RestoreCeiling()
    {
        if (hasCeiling) return;
        hasCeiling = true; Ceiling.SetActive(true);
        remainingWalls.Add(Ceiling);
        destroyedWalls.Remove(Ceiling);
    }

    /// The Floor of the maze cell
    [SerializeField] public GameObject Floor;
    public bool hasFloor { get; private set; }
    public void DestroyFloor()
    {
        hasFloor = false; Floor.SetActive(false);   
        remainingWalls.Remove(Floor);
        destroyedWalls.Add(Floor);
    }
    public void RestoreFloor()
    {
        if (hasFloor) return;
        hasFloor = true; Floor.SetActive(true);
        remainingWalls.Add(Floor);
        destroyedWalls.Remove(Floor);
    }

    /// Marks the cell as visited
    [SerializeField] public GameObject UnvisitedCell;
    public bool cellWasVisited { get; private set; }
    public void Visit()
    {
        cellWasVisited = true; UnvisitedCell.SetActive(false);
    }

    [Header("Maze Cell Information")]
    public int startingNumberOfCellWalls;
    public bool isBoundaryCell = false;
    public List<GameObject> boundaryWalls = new List<GameObject>();
    public List<GameObject> remainingWalls = new List<GameObject>();
    public List<GameObject> destroyedWalls = new List<GameObject>();

    private void Awake()
    {
        cellWasVisited = false;
        boundaryWalls.Clear();
        remainingWalls.Clear();
        startingNumberOfCellWalls = 0;

        if (LeftWall != null)
        {
            hasLeftWall = true;
            remainingWalls.Add(LeftWall);
            startingNumberOfCellWalls += 1;
        }

        if (RightWall != null)
        {
            hasRightWall = true;
            remainingWalls.Add(RightWall);
            startingNumberOfCellWalls += 1;
        }

        if (FrontWall != null)
        {
            hasFrontWall = true;
            remainingWalls.Add(FrontWall);
            startingNumberOfCellWalls += 1;
        }

        if (BackWall != null)
        {
            hasBackWall = true;
            remainingWalls.Add(BackWall);
            startingNumberOfCellWalls += 1;
        }

        if (Ceiling != null)
        {
            hasCeiling = true;
            remainingWalls.Add(Ceiling);
            startingNumberOfCellWalls += 1;
        }

        if (Floor != null)
        {
            hasFloor = true;
            remainingWalls.Add(Floor);
            startingNumberOfCellWalls += 1;
        }


    }

    public bool DeleteRandomRemainingWall(int mazeHeight)
    {
        if (remainingWalls.Count > 0)
        {
            int randomIndex = Random.Range(0, remainingWalls.Count);
            GameObject wall = remainingWalls[randomIndex];
            if (!boundaryWalls.Contains(wall))
            {
                if (wall == LeftWall)
                {
                    DestroyLeftWall();
                }
                else if (wall == RightWall)
                {
                    DestroyRightWall();
                }
                else if (wall == FrontWall)
                {
                    DestroyFrontWall();
                }
                else if (wall == BackWall)
                {
                    DestroyBackWall();
                }
                else if (mazeHeight > 1)
                {
                    if (wall == Ceiling)
                    {
                        DestroyCeiling();
                    }
                    else if (wall == Floor)
                    {
                        DestroyFloor();
                    }
                }
            }
            return true;
        }
        else return false;

    }

    public void RemoveBoundaryWallsFromRemainingWalls()
    {
        foreach (GameObject wall in boundaryWalls)
        {
            remainingWalls.Remove(wall);
        }
    }

    public void RestoreRandomWall(bool topDownView)
    {
        int randomIndex = Random.Range(0, destroyedWalls.Count);

        GameObject wall = destroyedWalls[randomIndex];
        if (wall == LeftWall)
        {
            RestoreLeftWall();
        }
        else if (wall == RightWall)
        {
            RestoreRightWall();
        }
        else if (wall == FrontWall)
        {
            RestoreFrontWall();
        }
        else if (wall == BackWall)
        {
            RestoreBackWall();
        }
        else
        {
            if (!topDownView)
            {
                if (wall == Ceiling)
                {
                    RestoreCeiling();
                }
                else if (wall == Floor)
                {
                    RestoreFloor();
                }
            }
        }

    }
}
