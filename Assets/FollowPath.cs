using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour {
    public List<Vector3> positions= new List<Vector3>();
    public int index = 1;
    public float speed=2;
    private bool shouldMove=true;
    public float minDis = 0.5f;

    public void Set(List<Vector3> positions) {
        this.positions = positions;

    }

    public void Start()
    {
        if (positions.Count <= 1)
        {
            shouldMove = false;
        }
        else {
            this.transform.LookAt(positions[index]);
        }
    }

    public void Update()
    {
        if (shouldMove) {
            this.transform.position += this.transform.forward * speed * Time.deltaTime;
            if (Vector3.Distance(this.transform.position, positions[index]) < minDis) {
                ChangeIndex();
            }
        }
    }

    private void ChangeIndex()
    {
        index++;
        if (index >= positions.Count) {
            index = 0;
        }
        this.transform.LookAt(positions[index]);
    }
}
