using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode{

    public Rect rect;
    public Color color;
    public int id = 0;
    private bool _overNode;

    public int gridX, gridY;

    public GridNode (float x, float y, float width, float heigth, Color col, int id, int gX, int gY)
    {
        rect = new Rect(x, y, width, heigth);
        color = col;
        this.id = id;
        gridX = gX;
        gridY = gY;
    }

    public void CheckMouse(Event current, Vector2 pan)
    {
        _overNode = rect.Contains(current.mousePosition - pan);
    }

    public void SetColorAndID(Color col, int id)
    {
        color = col;
        this.id = id;
    }

    public bool overNode { get { return _overNode; } }
}
