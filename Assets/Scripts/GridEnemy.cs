using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEnemy : GridNode {
    public List<int> path = new List<int>();


    public GridEnemy(float x, float y, float width, float heigth, Color col, int id, int gX, int gY): base(x, y, width, heigth, col, id, gX, gY)
    {
    }

    void Start () {
		
	}
	
	void Update () {
		
	}
}
