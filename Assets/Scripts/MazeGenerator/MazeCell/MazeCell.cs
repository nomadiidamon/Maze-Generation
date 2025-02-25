using UnityEngine;

public class MazeCell : MonoBehaviour
{ 
    [SerializeField] GameObject LeftWall;       private bool hasLeftWall {  get; set; }
    public bool DestroyLeftWall() { hasLeftWall = false; LeftWall.SetActive(false); return true; ;    }
    
    [SerializeField] GameObject RightWall;       public bool hasRightWall { get; private set ; }
    public bool DestroyRightWall()    { hasRightWall = false; RightWall.SetActive(false);  return true; }

    [SerializeField] GameObject FrontWall;       public bool hasFrontWall { get; private set ; }
    public bool DestroyFrontWall()    { hasFrontWall = false; FrontWall.SetActive(false);  return true; }

    [SerializeField] GameObject BackWall;       public bool hasBackWall { get; private set ; }
    public bool DestroyBackWall()    { hasBackWall = false; BackWall.SetActive(false); return true;  }

    [SerializeField] GameObject Cieling;        public bool hasCieling { get; private set ; }
    public bool DestroyCieling() { hasCieling = false; Cieling.SetActive(false); return true; }

    [SerializeField] GameObject Floor;         public bool hasFloor {  get; private set; }
    public bool DestroyFloor()     { hasFloor = false; Floor.SetActive(false); return true;   }


    [SerializeField] GameObject UnvisitedCell;         public bool cellWasVisited {  get; private set; }
    public bool Visit() { cellWasVisited = true; UnvisitedCell.SetActive(false);return true; } 










}
