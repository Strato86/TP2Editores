using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : ScriptableObject
{
    public string roomName;
    public List<GridNode> floorNodes;
    public List<GridNode> obstacleNodes;
    public Dictionary<ModuleNode, List<GridNode>> enemiesPath;
    public List<GridNode> triggerNodes;
    public List<GridNode> waypointNodes;
    public List<GridEnemy> enemiesNodes;
}
