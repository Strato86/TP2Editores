using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath  {
    Dictionary<GridNode, List<GridNode>> path = new Dictionary<GridNode, List<GridNode>>();


    public EnemyPath(GridNode enemy) {
        path[enemy] = new List<GridNode>();
    }

}
