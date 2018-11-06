using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class HowToNodes
{
    public Rect rect;
    public Color color;
    public string function;
    private bool isOnNode;
    public int gridY;
    public int identity = 0;


    public HowToNodes()
    {
        color = Color.grey;
        identity = -1;
        gridY = -1;
    }

    public HowToNodes(float x, float y, float width, float height, int gY, int id, Color col)
    {
        color = col;
        rect = new Rect(x, y, width, height);
        this.identity = id;
        gridY = gY;
    }

    public void MouseCheck(Event current, Vector2 mouseover)
    {
        if (rect.Contains(current.mousePosition - mouseover)) { isOnNode = true; Debug.Log("yes"); } else isOnNode = false;
    }

    public void SelectColorID(Color col, int id)
    {
        color = col;
        this.identity = id;
    }

    public bool overnode { get { return isOnNode; } }
}
