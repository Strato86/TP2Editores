using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class EventObject : MonoBehaviour {


    private void Update()
    {
 
    }

    public void PrintHelloWorld() {
        Debug.Log("HelloWorld");
    }
    public void PrintHi()
    {
        Debug.Log("Hi");
    }
    public void PrintBye()
    {
        Debug.Log("Bye");
    }
}
