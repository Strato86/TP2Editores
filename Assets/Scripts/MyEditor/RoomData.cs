using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : ScriptableObject
{
    public string roomName;
    public List<GridNode> floorNodes;
    public List<GridNode> obstacleNodes;
    public List<GridNode> triggerNodes;
    public List<GridNode> waypointNodes;
    public List<GridEnemy> enemiesNodes;
}
