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
    [SerializeField] public int mazePrefabScale = 1;

    public MazeCell[,] mazeLevel;
    public List<MazeCell[,]> levels = new List<MazeCell[,]>();


    private IEnumerator Start()
    {
        Initialize();

        yield return GenerateMaze(null, levels[0][0, 0]) ;


    }
    public void Initialize()
    {
        cell = mazeCell.GetComponent<MazeCell>();
        levels.Clear();

        for (int k = 0; k < mazeHeight; k++)
        {
            mazeLevel = new MazeCell[mazeWidth, mazeDepth];
            for (int i = 0; i < mazeWidth; i++)
            {
                for (int j = 0; j < mazeDepth; j++)
                {
                    mazeLevel[i, j] = Instantiate(cell, new Vector3(i, k, j), Quaternion.identity);
                }
            }
            levels.Add(mazeLevel);
            transform.position = new Vector3(transform.position.x, (transform.position.y + 1), transform.position.z);
        }

    }

    private IEnumerator GenerateMaze(MazeCell prevCell, MazeCell currCell)
    {
        if (!currCell.cellWasVisited)
        {
            currCell.Visit();
            //ClearWalls(prevCell, currCell);

            MazeCell nextCell = GetNextUnvisitedNeighbor(currCell);
            if (nextCell != null) { yield return GenerateMaze(currCell, nextCell); }    
        }

    }

    private MazeCell GetNextUnvisitedNeighbor(MazeCell currCell)
    {
        var unvistedCells = GetUnvisitedCells(currCell);
        return unvistedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();

    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currCell)
    {

        int x = (int)(currCell.transform.position.x);
        int z = (int)(currCell.transform.position.z);

        if ((x + 1) < mazeWidth)
        {
            var cellToRight = mazeLevel[x + 1, z];
            if (cellToRight.cellWasVisited == false)
            {
                yield return cellToRight;
            }

        }



        if ((x - 1) >= 0)
        {
            var cellToLeft = mazeLevel[x - 1, z];
            if (cellToLeft.cellWasVisited == false)
            {
                yield return cellToLeft;
            }

        }


        if ((z + 1) < mazeDepth)
        {
            var cellToFront = mazeLevel[x, z + 1];
            if (cellToFront.cellWasVisited == false)
            {
                yield return cellToFront;
            }

        }

        if ((z - 1) >= 0)
        {
            var cellToBack = mazeLevel[x, z - 1];
            if (cellToBack.cellWasVisited == false)
            {
                yield return cellToBack;
            }

        }

        int randWidth = Random.Range(0, mazeWidth);
        int randDepth = Random.Range(0, mazeDepth);

        if (mazeLevel[randWidth, randDepth].cellWasVisited == false)
        {
            var cellToVisit = mazeLevel[randWidth, randDepth];
            yield return cellToVisit;
        }
            


            

    }





    private void ClearWalls(MazeCell prevCell, MazeCell currCell)
    {
      
        
        if (prevCell.transform.position.x < currCell.transform.position.x) { prevCell.DestroyRightWall(); currCell.DestroyLeftWall(); return; }
        if (prevCell.transform.position.x > currCell.transform.position.x) { prevCell.DestroyLeftWall(); currCell.DestroyRightWall(); return; }

        if (prevCell.transform.position.z < currCell.transform.position.z) { prevCell.DestroyFrontWall(); currCell.DestroyBackWall(); return; }
        if (prevCell.transform.position.z > currCell.transform.position.z) { prevCell.DestroyBackWall(); currCell.DestroyFrontWall(); return; }

        if (prevCell.transform.position.y < currCell.transform.position.y) { prevCell.DestroyCieling(); currCell.DestroyFloor(); return; }
        if (prevCell.transform.position.y > currCell.transform.position.y) { prevCell.DestroyFloor(); currCell.DestroyCieling(); }

    }




    void Update()
    {
            int randWidth = Random.Range(0, mazeWidth);
            int randDepth = Random.Range(0, mazeDepth);
            int randHeight = Random.Range(0, mazeHeight);
            ClearWalls(null, levels[randHeight][randWidth, randDepth]);
        //for (int i = 0; i < levels.Count; i++)
        //{
        //    for (int j = 0;  j < levels[i].Length; j++) 
        //    {


        //    }
        //}
    }
}
