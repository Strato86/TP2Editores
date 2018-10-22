using System.Collections;
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
    int gridSeparation = 20;
    int gridBold = 5;

    int rulerBorder = 20;
    Color gridColor = new Color(0.2f, 0.2f, 0.2f, 1);

    Color editorColor = new Color(194f/255f, 194f/255f, 194f/255f, 1);

    Color rulerColor = new Color(0.4f,0.4f,0.4f,1);
    Color rulerGUIColor = new Color(0.8f,0.8f,0.8f,0.8f);

    int amount;

    //Paint Tool Variables
    List<GridNode> floorNodes;
    GridNode pickedGridNode;
    List<GridNode> obstacleNodes;
    Vector2Int moduleSize;
    Vector2Int boardSize;


    Layers layer;
    bool floorlayer = true;
    bool obstaclesLayer;
    bool enemiesLayer;
    bool eventLayer;

    Tools selectedTool;

    //Duplicate Tool Variables
    List<GridNode> duplicateGroup;
    Vector2 firstSelection;
    Vector2 lastSelection;


    string groupName = "";

    string prefabRoute = "LevelCreator";
    string floorFolder = "Floor prefabs";
    string obstaclesFolder = "Obstacle prefabs";

    public static void OpenWindow(int amount, Vector2Int moduleSize, Vector2Int boardSize)
    {
        var w = (CustomRoomWindow)GetWindow(typeof(CustomRoomWindow));
        w.floorModules = new List<ModuleNode>();
        w.obstacleModules = new List<ModuleNode>();

        w.graphPan = new Vector2(w.toolBarWidth + w.rulerBorder, w.smallBorder + w.rulerBorder);
        w.roomGraph = new Rect(w.toolBarWidth + w.rulerBorder, w.smallBorder + w.rulerBorder, 1000000, 1000000);

        if(boardSize == Vector2Int.zero)
        {
            boardSize = new Vector2Int(80, 50);
        }
        w.moduleSize = moduleSize;
        w.boardSize = boardSize;
        w.pickedGridNode = new GridNode(0, 0, 0, 0, Color.clear,-1,0,0);

        w.floorNodes = new List<GridNode>();
        w.obstacleNodes = new List<GridNode>();
        w.duplicateGroup = new List<GridNode>();

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

        w.minSize = new Vector2(500,350);
    }

    private void OnGUI()
    {
        
        CheckMouseInput(Event.current);

        roomGraph.x = graphPan.x;
        roomGraph.y = graphPan.y;
        EditorGUI.DrawRect(new Rect(toolBarWidth + rulerBorder, smallBorder + rulerBorder, position.width - toolBarWidth - smallBorder, position.height - bottomBarheight), Color.gray);
        

        GUI.BeginGroup(roomGraph);

        BeginWindows();
        //Board Nodes
        var otherLayersActivacted = obstaclesLayer || enemiesLayer || eventLayer;
        if(floorlayer)
        {
            foreach(var fN in floorNodes)
            {
                fN.rect.width = gridSeparation;
                fN.rect.height = gridSeparation;
                fN.rect.x = fN.gridX * gridSeparation;
                fN.rect.y = fN.gridY * gridSeparation; 
                fN.color = floorModules[fN.id].color;
                if(otherLayersActivacted)
                {
                    fN.color = Color.blue;
                    fN.color = new Color(fN.color.r,fN.color.g,fN.color.b,0.5f);
                }else
                {
                    fN.color = new Color(fN.color.r,fN.color.g,fN.color.b,1f);
                }
                EditorGUI.DrawRect(fN.rect, fN.color);
            }
        }
        if(obstaclesLayer)
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

        DrawGrid();

        DrawSelectionForDuplicateTool();

        EndWindows();

        GUI.EndGroup();

        DrawRulers();


        //Editor Borders
        EditorGUI.DrawRect(new Rect(0, 0, toolBarWidth, position.height), editorColor);
        EditorGUI.DrawRect(new Rect(0, position.height - bottomBarheight, position.width, bottomBarheight), editorColor);
        EditorGUI.DrawRect(new Rect(0, 0, position.width, smallBorder), editorColor);
        EditorGUI.DrawRect(new Rect(position.width - smallBorder, 0, smallBorder, position.height), editorColor);

        //Layer Border
        EditorGUI.DrawRect(new Rect(toolBarWidth + 95, 0, position.width - toolBarWidth - smallBorder - 95 , smallBorder - 5), rulerColor);

        //Editor buttons
        EditorGUILayout.BeginVertical(GUILayout.Height(position.height - bottomBarheight + 20));
        EditorGUILayout.BeginHorizontal(GUILayout.Height( position.height - bottomBarheight));
        EditorGUILayout.BeginVertical(GUILayout.Width(200));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Room Painter", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Group Name");
        groupName = EditorGUILayout.TextField(groupName);
        moduleSize = EditorGUILayout.Vector2IntField("Module Dimensions", moduleSize);
 
        EditorGUILayout.LabelField("Selected Tool", EditorStyles.boldLabel);
        if(pickedGridNode.id < 0)
        {
            GUI.DrawTexture(GUILayoutUtility.GetRect(32, 32, GUILayout.Width(32)), (Texture2D)Resources.Load("eraser"), ScaleMode.ScaleToFit);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(32, 32, GUILayout.Width(32)), (Texture2D)Resources.Load("paint"), ScaleMode.ScaleToFit);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("id: " + (pickedGridNode.id).ToString());
            EditorGUILayout.ColorField("Color", pickedGridNode.color);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Draw Layer", GUILayout.Width(100));
        layer = (Layers)EditorGUILayout.EnumPopup(layer,GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Width(250), GUILayout.Height(Mathf.Max(40,(position.height - bottomBarheight - 220))));
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
                    var p = GUILayout.Button("Pick");
                    if (p)
                    {
                        pickedGridNode.SetColorAndID(floorModules[i].color, floorModules[i].id);
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

        if (GUILayout.Button("Reset"))
        {
            floorNodes = new List<GridNode>();
            obstacleNodes = new List<GridNode>();
        }
        /*if (GUILayout.Button("Erase"))
        {
            pickedGridNode.SetColorAndID(Color.clear, -1);
        }*/
        if (GUILayout.Button("Duplicate"))
        {
            selectedTool = Tools.Duplicate;
        }
        if (GUILayout.Button("Create"))
        {
            CreateRoom(groupName);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Zoom: ", GUILayout.Width(40));
        var zoomIn = GUILayout.Button("+", GUILayout.Width(20));
        var zoomOut = GUILayout.Button("-", GUILayout.Width(20));

        EditorGUILayout.LabelField("Layers: ", EditorStyles.boldLabel, GUILayout.Width(50));
        //layer = (Layers)EditorGUILayout.EnumPopup(layer,GUILayout.Width(100));
        if (zoomIn) gridSeparation++;
        if (zoomOut) gridSeparation = gridSeparation <= 10 ? 10 : gridSeparation - 1;

        floorlayer = EditorGUILayout.Toggle("Floor", floorlayer);
        obstaclesLayer = EditorGUILayout.Toggle("Obstacles", obstaclesLayer);
        enemiesLayer = EditorGUILayout.Toggle("Enemies", enemiesLayer);
        eventLayer = EditorGUILayout.Toggle("Event Trigger", eventLayer);
    
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
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
            graphPan.x = Mathf.Min(toolBarWidth + rulerBorder,_prevPan.x + current.mousePosition.x - _originalMousePosition.x);
            graphPan.y = Mathf.Min(smallBorder + rulerBorder,_prevPan.y + current.mousePosition.y - _originalMousePosition.y);
            Repaint();
        }

        if(current.button == 0 && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
        {
            if(selectedTool != Tools.Duplicate)
            {
                DrawPrefabNode(current);
            }else if(current.type != EventType.MouseDrag)
            {
                DrawDuplicateGroup(current);
            }
        }
        else if(current.button == 1 &&(current.type == EventType.MouseDown))
        {
            if(selectedTool == Tools.Duplicate)
            {
                ResetDuplicateSelection(current);
            }
        }
        else if(current.button == 1 &&(current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
        {
            if(selectedTool == Tools.Duplicate)
            {
                SelectDuplicateGroup(current);
            }
        }
        else if(current.button == 1 && current.type == EventType.MouseUp)
        {
            SetDuplicateGroup();
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

    private void DrawDuplicateGroup(Event current)
    {
        var x = (int)((current.mousePosition.x - graphPan.x) / gridSeparation);
        var y = (int)((current.mousePosition.y - graphPan.y) / gridSeparation);

        var minX = (int)Mathf.Min(firstSelection.x, lastSelection.x);
        var minY = (int)Mathf.Min(firstSelection.y, lastSelection.y);
        var maxX = (int)Mathf.Max(firstSelection.x, lastSelection.x);
        var maxY = (int)Mathf.Max(firstSelection.y, lastSelection.y);

        foreach(var dn in duplicateGroup)
        {
            var gX = (dn.gridX + x - minX);
            var gY = (dn.gridY + y - minY);
            var g = new GridNode(gX * gridSeparation, gY * gridSeparation ,gridSeparation,gridSeparation,dn.color, dn.id, gX, gY);
            floorNodes.Add(g);
            Repaint();
            
        }
    }

    private void SelectDuplicateGroup(Event current)
    {
        var x = (int)((current.mousePosition.x - graphPan.x) / gridSeparation);
        var y = (int)((current.mousePosition.y - graphPan.y) / gridSeparation);

        lastSelection.x = x;
        lastSelection.y = y;

    }

    void ResetDuplicateSelection(Event current)
    {
        var x = (int)((current.mousePosition.x - graphPan.x) / gridSeparation);
        var y = (int)((current.mousePosition.y - graphPan.y) / gridSeparation);

        firstSelection.x = x;
        firstSelection.y = y;
        lastSelection.x = x;
        lastSelection.y = y;
    }

    void DrawSelectionForDuplicateTool()
    {
        if(selectedTool == Tools.Duplicate)
        {
            var c = Color.yellow;
            c = new Color(c.r,c.g,c.b,c.a/3);
            EditorGUI.DrawRect(new Rect(firstSelection.x * gridSeparation, firstSelection.y * gridSeparation, (lastSelection.x - firstSelection.x) * gridSeparation, (lastSelection.y - firstSelection.y) * gridSeparation), c);
            //Border Linse
            c = Color.yellow;
            EditorGUI.DrawRect(new Rect(firstSelection.x * gridSeparation, firstSelection.y * gridSeparation, 2 ,(lastSelection.y - firstSelection.y) * gridSeparation), c);
            EditorGUI.DrawRect(new Rect(firstSelection.x * gridSeparation, firstSelection.y * gridSeparation, (lastSelection.x - firstSelection.x) * gridSeparation,2),c);
            EditorGUI.DrawRect(new Rect(lastSelection.x* gridSeparation, firstSelection.y * gridSeparation, 2 ,(lastSelection.y - firstSelection.y) * gridSeparation), c);
            EditorGUI.DrawRect(new Rect(firstSelection.x*gridSeparation, lastSelection.y * gridSeparation, (lastSelection.x - firstSelection.x) * gridSeparation + 2 ,2),c);
            Repaint();
        }
    }

    void SetDuplicateGroup()
    {
        duplicateGroup = new List<GridNode>();
        var minX = (int)Mathf.Min(firstSelection.x, lastSelection.x);
        var minY = (int)Mathf.Min(firstSelection.y, lastSelection.y);
        var maxX = (int)Mathf.Max(firstSelection.x, lastSelection.x);
        var maxY = (int)Mathf.Max(firstSelection.y, lastSelection.y);
        for(int i = minY; i<maxY ; i++)
        {
            for(int j = minX; j<maxX ; j++)
            {
                foreach(var fn in floorNodes)
                {
                    if(fn.gridX == j && fn.gridY == i)
                    {
                        duplicateGroup.Add(fn);
                    }
                }
            }
        }
    }
    void DrawGrid()
    {
        //Horizontal Lines
        for (int i = 0; i * gridSeparation + graphPan.y <= position.height - bottomBarheight; i++)
        {
            var b = i % gridBold == 0 ? 2 : 1;
            var width = roomGraph.width;
            EditorGUI.DrawRect(new Rect(0, i * gridSeparation, width, b), gridColor);
        }

        //Vertical Lines
        for (int i = 0; i*gridSeparation + graphPan.x <= position.width  - smallBorder; i++)
        {
            var b = i % 5 == 0 ? 2 : 1;
            var height = roomGraph.height;
            EditorGUI.DrawRect(new Rect(i * gridSeparation, 0, b, height), gridColor);
        }
    }

    void DrawRulers()
    {
        //Vertical Ruler
        EditorGUI.DrawRect(new Rect(toolBarWidth,smallBorder,rulerBorder, position.height - smallBorder), rulerColor);
        EditorGUI.DrawRect(new Rect(toolBarWidth + rulerBorder - 2 ,smallBorder, 2 , position.height - smallBorder), rulerGUIColor);

        //Horizontal Ruler
        EditorGUI.DrawRect(new Rect(toolBarWidth,smallBorder,position.width - toolBarWidth, rulerBorder), rulerColor);
        EditorGUI.DrawRect(new Rect(toolBarWidth,smallBorder + rulerBorder - 2 ,position.width - toolBarWidth, 2), rulerGUIColor);

        for(int i = 0; i*gridSeparation <= position.width - graphPan.x - smallBorder; i++)
        {
            var b = 1;
            if(i % gridBold == 0 )
            {
                b = 2;
                //EditorGUILayout.TextField(i);
                //TODO: Texto para la regla
            }
            EditorGUI.DrawRect(new Rect(i*gridSeparation + graphPan.x,smallBorder + rulerBorder/2,b,rulerBorder/2), rulerGUIColor);
        }

        for(int i = 0; i*gridSeparation <= position.width - graphPan.x - smallBorder; i++)
        {
            var b = 1;
            if(i % gridBold == 0 )
            {
                b = 2;
                //EditorGUILayout.TextField(i);
                //TODO: Texto para la regla
            }
            EditorGUI.DrawRect(new Rect(toolBarWidth + rulerBorder/2, i*gridSeparation + graphPan.y,rulerBorder/2,b), rulerGUIColor);
        }

        //Corner Square
        EditorGUI.DrawRect(new Rect(toolBarWidth, smallBorder, rulerBorder,rulerBorder), rulerColor);
        EditorGUI.DrawRect(new Rect(toolBarWidth + rulerBorder - 2, smallBorder, 2,rulerBorder), rulerGUIColor);
        EditorGUI.DrawRect(new Rect(toolBarWidth , smallBorder + rulerBorder - 2,rulerBorder, 2), rulerGUIColor);
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

    public enum Tools
    {
        Brush,
        Eraser,
        Duplicate
    }
}
