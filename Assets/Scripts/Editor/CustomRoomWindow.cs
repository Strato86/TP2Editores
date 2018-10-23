﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public class CustomRoomWindow : EditorWindow {

    List<ModuleNode> floorModules;
    List<ModuleNode> obstacleModules;
    Rect roomGraph;
    float toolBarWidth = 250;
    float bottomBarheight = 10;

    Vector2 scrollView;

    Vector2 graphPan;
    Vector2 _prevPan;
    Vector2 _originalMousePosition;

    bool _panningScreen;
    bool _singleTap = true;

    Color col;
    GameObject go;

    int smallBorder = 25;
    float gridSeparation = 20;
    int gridBold = 5;
    Color gridColor = new Color(0.2f, 0.2f, 0.2f, 1);

    Color editorColor = new Color(194f/255f, 194f/255f, 194f/255f, 1);

    int amount;
    List<GridNode> floorNodes;
    List<GridNode> obstacleNodes;
    Vector2Int moduleSize;
    Vector2Int boardSize;

    GridNode pickedGridNode;

    Layers layer;

    string groupName = "";

    string prefabRoute = "LevelCreator";
    string floorFolder = "Floor prefabs";
    string obstaclesFolder = "Obstacle prefabs";

    Editor _prev;
    private Color pencilColor;
    private Color defaultPencilColor;
    public static void OpenWindow(int amount, Vector2Int moduleSize, Vector2Int boardSize)
    {
        var w = (CustomRoomWindow)GetWindow(typeof(CustomRoomWindow));
        w.floorModules = new List<ModuleNode>();
        w.obstacleModules = new List<ModuleNode>();

        w.graphPan = new Vector2(w.toolBarWidth, w.smallBorder);
        w.roomGraph = new Rect(w.toolBarWidth, w.smallBorder, 1000000, 1000000);

        if(boardSize == Vector2Int.zero)
        {
            boardSize = new Vector2Int(80, 50);
        }
        w.moduleSize = moduleSize;
        w.boardSize = boardSize;
        w.pickedGridNode = new GridNode(0, 0, 0, 0, Color.clear,-1,0,0);

        w.floorNodes = new List<GridNode>();
        w.obstacleNodes = new List<GridNode>();

        if (!AssetDatabase.IsValidFolder("Assets/LevelDesign"))
        {
            AssetDatabase.CreateFolder("Assets", "LevelDesign");
        }
        if (!AssetDatabase.IsValidFolder("Assets/LevelDesign/" + w.floorFolder))
        {
            AssetDatabase.CreateFolder("Assets/LevelDesign", w.floorFolder);
        }
        if (!AssetDatabase.IsValidFolder("Assets/LevelDesign/" + w.obstaclesFolder))
        {
            AssetDatabase.CreateFolder("Assets/LevelDesign", w.obstaclesFolder);
        }
        string[] folders = new string[1];
        folders[0] = "Assets/LevelDesign/" + w.floorFolder;
        var paths = AssetDatabase.FindAssets("t: Object", folders);
        
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);

            var pf = new ModuleNode();
            pf.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            w.floorModules.Add(pf);
        }

        folders[0] = "Assets/LevelDesign/" + w.obstaclesFolder;
        paths = AssetDatabase.FindAssets("t: Object", folders);
        
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);

            var pf = new ModuleNode();
            pf.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            w.obstacleModules.Add(pf);
        }
    }


    private void OnGUI()
    {
        defaultPencilColor = GUI.color;
        CheckMouseInput(Event.current);
        EditorGUILayout.BeginVertical(GUILayout.Height(position.height - bottomBarheight + 20));

        roomGraph.x = graphPan.x;
        roomGraph.y = graphPan.y;
        EditorGUI.DrawRect(new Rect(toolBarWidth, smallBorder, position.width - toolBarWidth - smallBorder, position.height - bottomBarheight), Color.gray);
        

        GUI.BeginGroup(roomGraph);

        BeginWindows();
        //Board Nodes
        
        foreach(var fN in floorNodes)
        {
            fN.rect.width = gridSeparation;
            fN.rect.height = gridSeparation;
            fN.rect.x = fN.gridX * gridSeparation;
            fN.rect.y = fN.gridY * gridSeparation; 
            fN.color = floorModules[fN.id].color;
            if(layer != Layers.Floor)
            {
                fN.color = new Color(fN.color.r,fN.color.g,fN.color.b,0.5f);
            }else
            {
                fN.color = new Color(fN.color.r,fN.color.g,fN.color.b,1f);
            }
            EditorGUI.DrawRect(fN.rect, fN.color);
        }
        if(layer != Layers.Floor)
        {
            foreach(var oN in obstacleNodes)
            {
                oN.rect.width = gridSeparation;
                oN.rect.height = gridSeparation;
                oN.rect.x = oN.gridX * gridSeparation;
                oN.rect.y = oN.gridY * gridSeparation; 
                oN.color = obstacleModules[oN.id].color;
                EditorGUI.DrawRect(oN.rect, oN.color);
                //Center color to diferenciate from floor
                var c = oN.color/2;
                var r = new Rect(oN.rect.x + gridSeparation/3, oN.rect.y + gridSeparation/3,gridSeparation/3,gridSeparation/3);
                EditorGUI.DrawRect(r,c);
            }   
        }
        //Horizontal Lines
        for (int i = 0; i * gridSeparation + graphPan.y <= position.height - bottomBarheight && i <= boardSize.y; i++)
        {
            var b = i % gridBold == 0 ? 2 : 1;
            var width = boardSize.x * gridSeparation + 2 + graphPan.x - toolBarWidth;
            EditorGUI.DrawRect(new Rect(-graphPan.x + toolBarWidth, i * gridSeparation, width, b), gridColor);
        }

        //Vertical Lines
        for (int i = 0; i*gridSeparation + graphPan.x <= position.width  - smallBorder && i <= boardSize.x; i++)
        {
            var b = i % 5 == 0 ? 2 : 1;
            var height = boardSize.y * gridSeparation + graphPan.y < position.height - bottomBarheight ? boardSize.y * gridSeparation + graphPan.y - smallBorder + 2 : position.height - bottomBarheight;
            EditorGUI.DrawRect(new Rect(i * gridSeparation, - graphPan.y + smallBorder, b, height), gridColor);
        }

        
        EndWindows();

        GUI.EndGroup();

        //Editor Borders
        EditorGUI.DrawRect(new Rect(0, 0, toolBarWidth, position.height), editorColor);
        EditorGUI.DrawRect(new Rect(0, position.height - bottomBarheight, position.width, bottomBarheight), editorColor);
        EditorGUI.DrawRect(new Rect(0, 0, position.width, smallBorder), editorColor);
        EditorGUI.DrawRect(new Rect(position.width - smallBorder, 0, smallBorder, position.height), editorColor);


        //Editor buttons
        EditorGUILayout.BeginHorizontal(GUILayout.Height( position.height - bottomBarheight));
        EditorGUILayout.BeginVertical(GUILayout.Width(200));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Room Painter", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Group Name");
        groupName = EditorGUILayout.TextField(groupName);
        moduleSize = EditorGUILayout.Vector2IntField("Module Dimensions", moduleSize);
        //EditorGUILayout.LabelField("Prefab Amount: " + floorModules.Count);

        /*var add = GUILayout.Button("Add Prefab");
        if (add)
        {
            var pf = new ModuleNode();
            floorModules.Add(pf);
        }*/
        EditorGUILayout.LabelField("Selected Tool", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUI.DrawTexture(GUILayoutUtility.GetRect(32, 32, GUILayout.Width(75)), (Texture2D)Resources.Load("eraser"), ScaleMode.ScaleToFit);

        var point = new Vector2(0, 120);
        var isPressed = false;
            
        isPressed = Handles.Button(GUILayoutUtility.GetLastRect().position+point, Quaternion.identity, 45f, 45f, Handles.CubeHandleCap);
        if (isPressed) {

            Debug.Log("presionó el botón 1");
            pickedGridNode.SetColorAndID(Color.clear, -1);
        }


        GUI.color = pencilColor;
        GUI.DrawTexture(GUILayoutUtility.GetRect(32, 32, GUILayout.Width(75)), (Texture2D)Resources.Load("paint"), ScaleMode.ScaleToFit);
        GUI.color = defaultPencilColor;
        point = new Vector2(75, 120);
         isPressed = false;

        isPressed = Handles.Button(GUILayoutUtility.GetLastRect().position + point, Quaternion.identity, 45f, 45f, Handles.CubeHandleCap);
        if (isPressed)
        {

            Debug.Log("presionó el botón ");
            pickedGridNode.SetColorAndID(Color.clear, -1);
        }
        EditorGUILayout.EndHorizontal();

      /*  EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("id: " + (pickedGridNode.id).ToString());
        EditorGUILayout.ColorField("Color", pickedGridNode.color);

        EditorGUILayout.EndVertical();

        */
   


        scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Width(250));
        //Prefabs
        switch(layer)
        {
            case Layers.Floor:
                for (int i = 0; i < floorModules.Count; i++)
                {
                    DrawLine(Color.gray);
                    floorModules[i].id = i;
                    EditorGUILayout.LabelField("id: " + (floorModules[i].id).ToString());
                    floorModules[i].color = EditorGUILayout.ColorField("Color", floorModules[i].color);
                    floorModules[i].color = new Color(floorModules[i].color.r, floorModules[i].color.g, floorModules[i].color.b, 1);
                    floorModules[i].prefab = (GameObject)EditorGUILayout.ObjectField(floorModules[i].prefab, typeof(GameObject), true);
                    if (floorModules[i].prefab)
                    {
                        GUIStyle myS = new GUIStyle();
                        myS.normal.background = EditorGUIUtility.whiteTexture;
                        if (pickedGridNode.id== floorModules[i].id) {
                            GUI.color = floorModules[i].color;
                        }

                        Texture2D texture= AssetPreview.GetAssetPreview(floorModules[i].prefab);
                        //  GUI.DrawTexture(GUILayoutUtility.GetRect(100, 100, GUILayout.Width(100)), (Texture2D)Resources.Load("eraser"), ScaleMode.ScaleToFit);
                       
                        GUI.DrawTexture(GUILayoutUtility.GetRect(100, 100, GUILayout.Width(100)), texture, ScaleMode.ScaleToFit);

                        GUI.color = defaultPencilColor;

                        point = new Vector2(0, 120 + 70*(i+1));
                        isPressed = false;
                        isPressed = Handles.Button(point, Quaternion.identity, 100, 100, Handles.CubeHandleCap);
                        if (isPressed)
                        {

                            Debug.Log("presionó el botón 2");
                            pickedGridNode.SetColorAndID(floorModules[i].color, floorModules[i].id);
                            pencilColor = floorModules[i].color;
                        }
 
                    }
                }


                break;
            case Layers.Obstacles:
                for(int i = 0; i< obstacleModules.Count; i++)
                {
                    DrawLine(Color.gray);
                    obstacleModules[i].id = i;
                    EditorGUILayout.LabelField("id: " + (obstacleModules[i].id).ToString());
                    obstacleModules[i].color = EditorGUILayout.ColorField("Color", obstacleModules[i].color);
                    obstacleModules[i].color = new Color(obstacleModules[i].color.r, obstacleModules[i].color.g, obstacleModules[i].color.b, 1);
                    obstacleModules[i].prefab = (GameObject)EditorGUILayout.ObjectField(obstacleModules[i].prefab, typeof(GameObject), true);
                    if (obstacleModules[i].prefab)
                    {
                        var p = GUILayout.Button("Pick");
                        if (p)
                        {
                            pickedGridNode.SetColorAndID(obstacleModules[i].color, obstacleModules[i].id);
                        }
                    }
                }
                break;
        }
        
        EditorGUILayout.EndScrollView();
       
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Zoom: ", GUILayout.Width(40));
        var zoomIn = GUILayout.Button("+", GUILayout.Width(20));
        var zoomOut = GUILayout.Button("-", GUILayout.Width(20));

        EditorGUILayout.LabelField("Layer: ", GUILayout.Width(40));
        layer = (Layers)EditorGUILayout.EnumPopup(layer,GUILayout.Width(100));
        if (zoomIn) gridSeparation++;
        if (zoomOut) gridSeparation = gridSeparation <= 10 ? 10 : gridSeparation - 1;

        if (GUILayout.Button("Reset"))
        {
            floorNodes = new List<GridNode>();
            obstacleNodes = new List<GridNode>();
        }
        
  /*      if (GUILayout.Button("Erase"))
        {
            pickedGridNode.SetColorAndID(Color.clear, -1);
        }*/
        if (GUILayout.Button("Create"))
        {
            CreateRoom(groupName);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        /*
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
        */
    }

    private void CheckMouseInput(Event current)
    {
        if (!roomGraph.Contains(current.mousePosition) || !(focusedWindow == this || mouseOverWindow == this))
            return;

        if(current.button == 2 && current.type == EventType.MouseDown)
        {
            _panningScreen = true;
            _prevPan = new Vector2(graphPan.x, graphPan.y);
            _originalMousePosition = current.mousePosition;
        }else if(current.button == 2 && current.type == EventType.MouseUp)
        {
            _panningScreen = false;
        }

        if (_panningScreen)
        {
            graphPan.x = _prevPan.x + current.mousePosition.x - _originalMousePosition.x;
            graphPan.y = _prevPan.y + current.mousePosition.y - _originalMousePosition.y;
            Repaint();
        }

        if(current.button == 0 && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
        {
            _singleTap = false;
            DrawPrefabNode(current);
        }
        else if(current.button == 0 && current.type == EventType.MouseUp)
        {
            _singleTap = true;
        }
    }

    private void DrawPrefabNode(Event current)
    {
        var x = (int)((current.mousePosition.x - graphPan.x) / gridSeparation);
        var y = (int)((current.mousePosition.y - graphPan.y) / gridSeparation);
        
        var id = pickedGridNode.id;
        bool isOcupied = false;
        switch(layer)
        {
            case Layers.Floor:
                foreach(var g in floorNodes)
                {
                    if(!isOcupied)
                        isOcupied = (g.gridX == x && g.gridY == y);
                }
                if(id < 0 && isOcupied)
                {
                    for(var i = floorNodes.Count-1; i >= 0;i--)
                    {
                        if(floorNodes[i].gridX == x && floorNodes[i].gridY == y)
                        {
                            floorNodes.RemoveAt(i);
                            Repaint();
                        }
                    }
                }
                else if(id >= 0)
                {
                    if(x < boardSize.x && y < boardSize.y && !isOcupied)
                    {
                        var g = new GridNode(x *gridSeparation,y*gridSeparation,gridSeparation,gridSeparation,pickedGridNode.color, pickedGridNode.id,x,y);
                        floorNodes.Add(g);
                        Repaint();
                    }
                }
                break;

            case Layers.Obstacles:
                foreach(var g in obstacleNodes)
                {
                    if(!isOcupied)
                        isOcupied = (g.gridX == x && g.gridY == y);
                }
                if(id < 0 && isOcupied)
                {
                    for(var i = obstacleNodes.Count-1; i >= 0;i--)
                    {
                        if(obstacleNodes[i].gridX == x && obstacleNodes[i].gridY == y)
                        {
                            floorNodes.RemoveAt(i);
                            Repaint();
                        }
                    }
                }
                else if(id >= 0)
                {
                    if(x < boardSize.x && y < boardSize.y && !isOcupied)
                    {
                        var g = new GridNode(x *gridSeparation,y*gridSeparation,gridSeparation,gridSeparation,pickedGridNode.color, pickedGridNode.id,x,y);
                        obstacleNodes.Add(g);
                        Repaint();
                    }
                }
                break;
        }
        

        
    }

    private void CreateRoom(string groupName)
    {
        var goParent = new GameObject(groupName);

        foreach(var fN in floorNodes)
        {
            var pos = new Vector3(-fN.gridX * moduleSize.x, 0f, fN.gridY * moduleSize.y);
            var pf = PrefabUtility.InstantiatePrefab(floorModules[fN.id].prefab);
            var go = ((GameObject)pf);
            go.transform.position = pos;
            go.transform.rotation = Quaternion.identity;
            go.transform.SetParent(goParent.transform);
        }

        foreach(var oN in obstacleNodes)
        {
            var pos = new Vector3(-oN.gridX * moduleSize.x, 0f, oN.gridY * moduleSize.y);
            var pf = PrefabUtility.InstantiatePrefab(obstacleModules[oN.id].prefab);
            var go = ((GameObject)pf);
            go.transform.position = pos;
            go.transform.rotation = Quaternion.identity;
            go.transform.SetParent(goParent.transform);
        }

    }

    private void DrawLine(Color col, bool bold = false, bool vertical = false)
    {
        EditorGUILayout.Space();
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), col);
        EditorGUILayout.Space();
    }

    public enum Layers
    {
        Floor,
        Obstacles,
        Enemies,
        EventTriggers
    }
}
