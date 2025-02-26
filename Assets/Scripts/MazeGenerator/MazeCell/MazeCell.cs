using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] public GameObject LeftWall;
    public bool hasLeftWall { get; set; }
    public void DestroyLeftWall()
    {
        hasLeftWall = false; LeftWall.SetActive(false); numOfWalls -= 1;
        remainingWalls.Remove(LeftWall);
    }

    [SerializeField] public GameObject RightWall;
    public bool hasRightWall { get; private set; }
    public void DestroyRightWall()
    {
        hasRightWall = false; RightWall.SetActive(false); numOfWalls -= 1;
        remainingWalls.Remove(RightWall);
    }

    [SerializeField] public GameObject FrontWall;
    public bool hasFrontWall { get; private set; }
    public void DestroyFrontWall()
    {
        hasFrontWall = false; FrontWall.SetActive(false); numOfWalls -= 1;
        remainingWalls.Remove(FrontWall);
    }

    [SerializeField] public GameObject BackWall;
    public bool hasBackWall { get; private set; }
    public void DestroyBackWall()
    {
        hasBackWall = false; BackWall.SetActive(false); numOfWalls -= 1;
        remainingWalls.Remove(BackWall);
    }

    [SerializeField] public GameObject Ceiling;
    public bool hasCeiling { get; private set; }
    public void DestroyCeiling()
    {
        hasCeiling = false; Ceiling.SetActive(false); numOfWalls -= 1;
        remainingWalls.Remove(Ceiling);
    }

    [SerializeField] public GameObject Floor;
    public bool hasFloor { get; private set; }
    public void DestroyFloor()
    {
        hasFloor = false; Floor.SetActive(false); numOfWalls -= 1;
        remainingWalls.Remove(Floor);
    }


    [SerializeField] public GameObject UnvisitedCell;
    public bool cellWasVisited { get; private set; }
    public void Visit()
    {
        cellWasVisited = true; UnvisitedCell.SetActive(false);
    }


    public int numOfWalls { get; private set; }
    public bool isBoundaryCell = false;
    public List<GameObject> boundaryWalls = new List<GameObject>();
    public List<GameObject> remainingWalls = new List<GameObject>();

    private void Awake()
    {
        cellWasVisited = false;
        boundaryWalls.Clear();
        remainingWalls.Clear();
        numOfWalls = 0;

        if (LeftWall != null)
        {
            hasLeftWall = true;
            remainingWalls.Add(LeftWall);
            numOfWalls += 1;
        }

        if (RightWall != null)
        {
            hasRightWall = true;
            remainingWalls.Add(RightWall);
            numOfWalls += 1;
        }

        if (FrontWall != null)
        {
            hasFrontWall = true;
            remainingWalls.Add(FrontWall);
            numOfWalls += 1;
        }

        if (BackWall != null)
        {
            hasBackWall = true;
            remainingWalls.Add(BackWall);
            numOfWalls += 1;
        }

        if (Ceiling != null)
        {
            hasCeiling = true;
            remainingWalls.Add(Ceiling);
            numOfWalls += 1;
        }

        if (Floor != null)
        {
            hasFloor = true;
            remainingWalls.Add(Floor);
            numOfWalls += 1;
        }


    }

    public void DeleteRandomRemainingWall(int mazeHeight)
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
        }

    }

    public void RemoveBoundaryWallsFromRemainingWalls()
    {
        foreach (GameObject wall in boundaryWalls)
        {
            remainingWalls.Remove(wall);
        }
    }
}
