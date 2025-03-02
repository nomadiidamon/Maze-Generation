using UnityEngine;
using UnityEngine.AI;

public class MazeTester : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector3 startingPos;
    public Vector3 goalPos;
    MazeGenerator maze;
    public bool testingInProgress = false;
    public bool testingDone = false;

    public void Start()
    {
        maze = FindAnyObjectByType<MazeGenerator>();
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (testingDone) return;
        if (maze.mazeIsFinished)
        {
            if (maze.mazeParentObject.levelIsReady)
            {
                if (!testingInProgress)
                {
                    gameObject.SetActive(true);
                    transform.position = startingPos;
                    if (agent.destination == null)
                    {
                        agent.SetDestination(goalPos);
                    }
                    testingInProgress = true;
                }
                else
                {
                    if (transform.position == goalPos)
                    {
                        testingDone = true;
                    }
                }
            }
        }
    }

}
