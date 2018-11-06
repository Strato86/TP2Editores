using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public class CustomRoomWindow : EditorWindow {

    List<ModuleNode> floorModules;
    List<ModuleNode> obstacleModules;
    List<ModuleNode> triggerModules;
    Rect roomGraph;
    float toolBarWidth = 250;
    float bottomBarheight = 10;

    Vector2 scrollView;

    Vector2 graphPan;
    Vector2 initialGraphPan;
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
    List<GridNode> triggerNodes;
    Vector2Int moduleSize;
    Vector2Int boardSize;


    Layers layer;
    bool floorlayer = true;
    bool obstaclesLayer;
    bool enemiesLayer;
    bool eventLayer;

    Tools selectedTool;
    DuplicateToolTip duplicateToolTip;

    //Duplicate Tool Variables
    List<GridNode> duplicateFloorGroup;
    List<GridNode> duplicateObstacleGroup;
    List<GridNode> duplicateTriggerGroup;
    Vector2 firstSelection;
    Vector2 lastSelection;


    string groupName = "";

    string prefabRoute = "LevelCreator";
    string floorFolder = "Floor prefabs";
    string obstaclesFolder = "Obstacle prefabs";
    string triggerFolder = "Trigger Prefabs";

    Editor _prev;
    private Color pencilColor = Color.white;
    private Color defaultColor;
    private int lastPencilId=0;


    public static void OpenWindow(int amount, Vector2Int moduleSize, Vector2Int boardSize)
    {
        var w = (CustomRoomWindow)GetWindow(typeof(CustomRoomWindow));
        w.floorModules = new List<ModuleNode>();
        w.obstacleModules = new List<ModuleNode>();
        w.triggerModules = new List<ModuleNode>();

        w.graphPan = new Vector2(w.toolBarWidth + w.rulerBorder, w.smallBorder + w.rulerBorder);
        w.initialGraphPan = w.graphPan;
        w.roomGraph = new Rect(w.toolBarWidth + w.rulerBorder, w.smallBorder + w.rulerBorder, 1000000, 1000000);

        if (boardSize == Vector2Int.zero)
        {
            boardSize = new Vector2Int(80, 50);
        }
        w.moduleSize = moduleSize;
        w.boardSize = boardSize;
        w.pickedGridNode = new GridNode();

        w.floorNodes = new List<GridNode>();
        w.obstacleNodes = new List<GridNode>();
        w.triggerNodes = new List<GridNode>();
        w.duplicateFloorGroup = new List<GridNode>();
        w.duplicateObstacleGroup = new List<GridNode>();
        w.duplicateTriggerGroup = new List<GridNode>();

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
        LoadModuleListFromFolder(w.floorModules, w.floorFolder);
        LoadModuleListFromFolder(w.obstacleModules, w.obstaclesFolder);
        LoadModuleListFromFolder(w.triggerModules, w.triggerFolder);
        w.minSize = new Vector2(500, 350);
    }

    private static void LoadModuleListFromFolder(List<ModuleNode> moduleList, string folderName)
    {
        string[] folders = new string[1];
        folders[0] = "Assets/LevelDesign/" + folderName;
        var paths = AssetDatabase.FindAssets("t: Object", folders);

        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);

            var pf = new ModuleNode();
            pf.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            moduleList.Add(pf);
        }
    }

    private void OnGUI()
    {
        defaultColor = GUI.color;
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
                var c = oN.color/2;
                var r = new Rect(oN.rect.x + gridSeparation/3, oN.rect.y + gridSeparation/3,gridSeparation/3,gridSeparation/3);
                EditorGUI.DrawRect(r,c);
            }   
        }

        if (eventLayer)
        {
            foreach (var tN in triggerNodes)
            {
                tN.rect.width = gridSeparation;
                tN.rect.height = gridSeparation;
                tN.rect.x = tN.gridX * gridSeparation;
                tN.rect.y = tN.gridY * gridSeparation;
                tN.color = triggerNodes[tN.id].color;
                EditorGUI.DrawRect(tN.rect, tN.color);
                var c = defaultColor;
                var r = new Rect(tN.rect.x + gridSeparation / 3, tN.rect.y + gridSeparation / 3, gridSeparation / 3, gridSeparation / 3);
                EditorGUI.DrawRect(r, c);
            }
        }

        DrawGrid();

        DrawSelectionForDuplicateTool();

        DrawSelectionPreview(Event.current);

        EndWindows();

        GUI.EndGroup();

        DrawRulers();


        //Editor Borders
        EditorGUI.DrawRect(new Rect(0, 0, toolBarWidth, position.height), editorColor);
        EditorGUI.DrawRect(new Rect(0, position.height - bottomBarheight, position.width, bottomBarheight), editorColor);
        EditorGUI.DrawRect(new Rect(0, 0, position.width, smallBorder), editorColor);
        EditorGUI.DrawRect(new Rect(position.width - smallBorder, 0, smallBorder, position.height), editorColor);

        //Layer Border
        EditorGUI.DrawRect(new Rect(toolBarWidth + 150, 0, position.width - toolBarWidth - smallBorder - 95 , smallBorder - 5), rulerColor);

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

        EditorGUILayout.BeginHorizontal();

        var opts = new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) };
        if (selectedTool == Tools.Eraser) {
            GUI.color = Color.yellow;
        }
        if (GUILayout.Button((Texture2D)Resources.Load("eraser"), opts))
        {

            pickedGridNode.SetColorAndID(Color.clear, -1);
            selectedTool = Tools.Eraser;
        }
        GUI.color = defaultColor;
        /*     var rect = GUILayoutUtility.GetRect(32, 32, GUILayout.Width(45));
             Repaint();

             if (rect.width != 1f)
             {
                 GUI.DrawTexture(rect, (Texture2D)Resources.Load("eraser"), ScaleMode.ScaleToFit);

                 var isPressedButton1 = false;

                 isPressedButton1 = Handles.Button(rect.position, Quaternion.identity, 45f, 45f, Handles.CubeHandleCap);
                 if (isPressedButton1)
                 {
                     //Debug.Log("presionó el botón 1");
                     pickedGridNode.SetColorAndID(Color.clear, -1);
                     selectedTool = Tools.Eraser;
                 }

             }
             */

        if (selectedTool == Tools.Brush)
        {
            GUI.color = Color.yellow;
        }
        if (GUILayout.Button((Texture2D)Resources.Load("paint"), opts))
        {

            pickedGridNode.SetColorAndID(floorModules[lastPencilId].color, lastPencilId);
            selectedTool = Tools.Brush;
        }
        GUI.color = defaultColor;

        if (selectedTool == Tools.Duplicate)
        {
            GUI.color = Color.yellow;
        }
        if (GUILayout.Button((Texture2D)Resources.Load("WEAPON"), opts))
        {

            pickedGridNode.SetColorAndID(Color.clear, -1);
            selectedTool = Tools.Duplicate;
        }
        GUI.color = defaultColor;

        EditorGUILayout.EndHorizontal();


   

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Draw Layer", GUILayout.Width(100));
        layer = (Layers)EditorGUILayout.EnumPopup(layer,GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Width(250), GUILayout.Height(Mathf.Max(40,(position.height - bottomBarheight - 220))));
        //Prefabs
        switch(selectedTool)
        {
            case Tools.Brush:
            switch(layer)
            {
                    case Layers.Floor:
                        DrawPrefabModuleFromList(floorModules);
                        break;
                    case Layers.Obstacles:
                        DrawPrefabModuleFromList(obstacleModules);
                        break;
                    case Layers.EventTriggers:
                        DrawPrefabModuleFromList(triggerModules);
                        foreach (var item in triggerModules)
                        {
                            if (item.prefab.GetComponent<TriggerEvent>() == null) {
                                EditorGUILayout.HelpBox("Alguno de los objectos en Trigger prefab no tiene un trigger event",MessageType.Warning);
                                break;
                            }
                        }
                        break;
                }
                break;
            
            case Tools.Duplicate:
                if(GUILayout.Button("Copy"))
                {
                    duplicateToolTip = DuplicateToolTip.Copy;
                }
                if(GUILayout.Button("Move"))
                {
                    duplicateToolTip = DuplicateToolTip.Move;
                }
            break;

            case Tools.Eraser:
            break;
        }
        
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Reset"))
        {
            floorNodes = new List<GridNode>();
            obstacleNodes = new List<GridNode>();
        }
        if (GUILayout.Button("Create"))
        {
            CreateRoom(groupName);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Zoom: ", GUILayout.Width(40));
        var zoomIn = GUILayout.Button("+", GUILayout.Width(20));
        var zoomOut = GUILayout.Button("-", GUILayout.Width(20));

        if(GUILayout.Button("Reset", GUILayout.Width(50)))
        {
            graphPan = initialGraphPan;
            gridSeparation = 20;
        }

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

    private void DrawPrefabModuleFromList(List<ModuleNode> list)
    {
        if (pickedGridNode.id >= list.Count) pickedGridNode.id = list.Count - 1;

        for (int i = 0; i < list.Count; i++)
        {
            DrawPrefabModule(list, i);
        }
    }


    private void DrawPrefabModule(List<ModuleNode> list, int i)
    {
        DrawLine(Color.gray);
        list[i].id = i;
        EditorGUILayout.BeginHorizontal();
        if (list[i].prefab)
        {
            GUIStyle myS = new GUIStyle();
            GUI.color = defaultColor;
            myS.normal.background = EditorGUIUtility.whiteTexture;
            if (pickedGridNode.id == list[i].id)
            {
                GUI.color = Color.yellow;
            }

            Texture2D texture = AssetPreview.GetAssetPreview(list[i].prefab);
            var rec = GUILayoutUtility.GetRect(100, 100, GUILayout.Width(100));
            //GUI.DrawTexture(rec, (Texture2D)Resources.Load("FreshLemEDT"), ScaleMode.ScaleToFit);
            EditorGUI.DrawRect(rec, GUI.color);
            var rec2 = GUILayoutUtility.GetLastRect();
            rec2.size = new Vector2(90f, 90f);
            rec2.position = rec2.position + new Vector2(5, 5);

            GUI.color = defaultColor;
            GUI.DrawTexture(rec2, texture, ScaleMode.ScaleToFit);

            var point = new Vector2(0, 250 + (200 * i));
            var isPressed = false;
            isPressed = Handles.Button(point, Quaternion.identity, 100, 100, Handles.CubeHandleCap);
            if (isPressed)
            {
                pickedGridNode.SetColorAndID(list[i].color, list[i].id);
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("id: " + (list[i].id).ToString());
            list[i].color = EditorGUILayout.ColorField("Color", list[i].color);
            list[i].color = new Color(list[i].color.r, list[i].color.g, list[i].color.b, 1);
            list[i].prefab = (GameObject)EditorGUILayout.ObjectField(list[i].prefab, typeof(GameObject), true);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
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
        switch(layer)
        {
            case Layers.Floor:
                PaintSquareScreen(x, y, floorNodes);
                break;

            case Layers.Obstacles:
                PaintSquareScreen(x, y, obstacleNodes);
                break;

            case Layers.EventTriggers:
                PaintSquareScreen(x, y, triggerNodes);
                break;
        }
        

        
    }

    private void PaintSquareScreen(int x, int y, List<GridNode> gridList)
    {

        GridNode auxNode = new GridNode();

        bool isOcupied = false;
        var id = pickedGridNode.id;
        foreach (var g in gridList)
        {
            if (!isOcupied)
            {
                isOcupied = (g.gridX == x && g.gridY == y);
                auxNode = g;
            }
        }
        if (id < 0 && isOcupied)
        {
            for (var i = gridList.Count - 1; i >= 0; i--)
            {
                if (gridList[i].gridX == x && gridList[i].gridY == y)
                {
                    gridList.RemoveAt(i);
                    Repaint();
                }
            }
        }
        else if (id >= 0)
        {
            if (!isOcupied)
            {
                var g = new GridNode(x * gridSeparation, y * gridSeparation, gridSeparation, gridSeparation, pickedGridNode.color, pickedGridNode.id, x, y);
                gridList.Add(g);
            }
            else
            {
                auxNode.SetColorAndID(pickedGridNode.color, id);
            }
            Repaint();
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

        //remove if is move
        if(duplicateToolTip == DuplicateToolTip.Move)
        {
            if(floorlayer)
            {
                SelectMovingGroup(floorNodes,duplicateFloorGroup);
            }
            if (obstaclesLayer)
            {
                SelectMovingGroup(obstacleNodes, duplicateObstacleGroup);
            }
            if (eventLayer) {
                SelectMovingGroup(triggerNodes, duplicateTriggerGroup);
            }
        }

        if(floorlayer)
        {
            MoveGroup(floorNodes, duplicateFloorGroup, x, y, minX, minY);
        }
        if(obstaclesLayer)
        {
            MoveGroup(obstacleNodes,duplicateObstacleGroup, x, y, minX, minY);
        }

        if (eventLayer)
        {
            MoveGroup(triggerNodes, duplicateTriggerGroup, x, y, minX, minY);
        }

        if (duplicateToolTip == DuplicateToolTip.Move)
        {
            duplicateFloorGroup = new List<GridNode>();
            duplicateObstacleGroup = new List<GridNode>();
            duplicateTriggerGroup = new List<GridNode>();
            firstSelection = new Vector2Int();
            lastSelection = new Vector2Int();
        }
    }

    private void MoveGroup(List<GridNode> nodes, List<GridNode> duplicateGroup,int x, int y, int minX, int minY)
    {
        foreach (var dob in duplicateGroup)
        {
            var gX = (dob.gridX + x - minX);
            var gY = (dob.gridY + y - minY);
            var ocupied = false;
            foreach (var n in nodes)
            {
                if (n.gridX == gX && n.gridY == gY)
                {
                    n.SetColorAndID(dob.color, dob.id);
                    break;
                }
            }
            if (!ocupied)
            {
                var g = new GridNode(gX * gridSeparation, gY * gridSeparation, gridSeparation, gridSeparation, dob.color, dob.id, gX, gY);
                nodes.Add(g);
            }
            Repaint();
        }
    }

    private void SelectMovingGroup(List<GridNode> nodes, List<GridNode> duplicateGroup)
    {
        for (int i = 0; i < duplicateGroup.Count; i++)
        {
            for (int j = nodes.Count - 1; j >= 0; j--)
            {
                if (nodes[j].gridX == duplicateGroup[i].gridX
                && nodes[j].gridY == duplicateGroup[i].gridY)
                {
                    nodes.RemoveAt(j);
                }
            }
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
            var c = duplicateToolTip == DuplicateToolTip.Copy ? Color.yellow:Color.green;
            c = new Color(c.r,c.g,c.b,c.a/3);
            EditorGUI.DrawRect(new Rect(firstSelection.x * gridSeparation, firstSelection.y * gridSeparation, (lastSelection.x - firstSelection.x) * gridSeparation, (lastSelection.y - firstSelection.y) * gridSeparation), c);
            //Border Linse
            c = duplicateToolTip == DuplicateToolTip.Copy ? Color.yellow:Color.green;
            EditorGUI.DrawRect(new Rect(firstSelection.x * gridSeparation, firstSelection.y * gridSeparation, 2 ,(lastSelection.y - firstSelection.y) * gridSeparation), c);
            EditorGUI.DrawRect(new Rect(firstSelection.x * gridSeparation, firstSelection.y * gridSeparation, (lastSelection.x - firstSelection.x) * gridSeparation,2),c);
            EditorGUI.DrawRect(new Rect(lastSelection.x* gridSeparation, firstSelection.y * gridSeparation, 2 ,(lastSelection.y - firstSelection.y) * gridSeparation), c);
            EditorGUI.DrawRect(new Rect(firstSelection.x*gridSeparation, lastSelection.y * gridSeparation, (lastSelection.x - firstSelection.x) * gridSeparation + 2 ,2),c);
            Repaint();
        }
    }

    void DrawSelectionPreview(Event current)
    {
        var x = (int)(current.mousePosition.x / gridSeparation);
        var y = (int)(current.mousePosition.y/ gridSeparation);
        var minX = (int)Mathf.Min(firstSelection.x, lastSelection.x);
        var minY = (int)Mathf.Min(firstSelection.y, lastSelection.y);

        if(floorlayer)
        {
            for(int i = 0 ; i< duplicateFloorGroup.Count; i++)
            {
                DrawFloorSelectionPreview(duplicateFloorGroup[i].gridX + x - minX, duplicateFloorGroup[i].gridY + y - minY);
            }
        }
        if(obstaclesLayer)
        {
            for(int i = 0 ; i< duplicateObstacleGroup.Count; i++)
            {
                DrawObstacleSelectionPreview(duplicateObstacleGroup[i].gridX + x - minX, duplicateObstacleGroup[i].gridY + y - minY);
            }
        }
        if (eventLayer)
        {
            for (int i = 0; i < duplicateTriggerGroup.Count; i++)
            {
                DrawObstacleSelectionPreview(duplicateTriggerGroup[i].gridX + x - minX, duplicateTriggerGroup[i].gridY + y - minY);
            }
        }
    }

    void SetDuplicateGroup()
    {
        duplicateFloorGroup = new List<GridNode>();
        duplicateObstacleGroup = new List<GridNode>();
        duplicateTriggerGroup = new List<GridNode>();
        var minX = (int)Mathf.Min(firstSelection.x, lastSelection.x);
        var minY = (int)Mathf.Min(firstSelection.y, lastSelection.y);
        var maxX = (int)Mathf.Max(firstSelection.x, lastSelection.x);
        var maxY = (int)Mathf.Max(firstSelection.y, lastSelection.y);
        for(int i = minY; i<maxY ; i++)
        {
            for(int j = minX; j<maxX ; j++)
            {
                if(floorlayer)
                {
                    foreach(var fn in floorNodes)
                    {
                        if(fn.gridX == j && fn.gridY == i)
                        {
                            duplicateFloorGroup.Add(fn);
                        }
                    }
                }
                if(obstaclesLayer)
                {
                    foreach(var on in obstacleNodes)
                    {
                        if(on.gridX == j && on.gridY == i)
                        {
                            duplicateObstacleGroup.Add(on);
                        }
                    }
                }
                if (eventLayer)
                {
                    foreach (var tn in triggerNodes)
                    {
                        if (tn.gridX == j && tn.gridY == i)
                        {
                            duplicateTriggerGroup.Add(tn);
                        }
                    }
                }

            }
        }
    }

    void DrawFloorSelectionPreview(int x, int y)
    {
        var c = Color.blue;
        
        c = new Color(c.r,c.g,c.b,c.a/2);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation,gridSeparation,gridSeparation), c);

        c = Color.blue;
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, 2, gridSeparation),c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation + gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation + gridSeparation, y*gridSeparation, 2, gridSeparation),c);
    }

    void DrawObstacleSelectionPreview(int x, int y)
    {
        var c = Color.red;

        c = new Color(c.r,c.g,c.b,c.a/2);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation,gridSeparation,gridSeparation), c);

        c = Color.red;
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, 2, gridSeparation),c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation + gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation + gridSeparation, y*gridSeparation, 2, gridSeparation),c);
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
                var gs = EditorStyles.whiteLabel;
                gs.fontSize = 10;
                gs.alignment = UnityEngine.TextAnchor.MiddleCenter;
                EditorGUI.LabelField(new Rect(i*gridSeparation + graphPan.x - gridSeparation/2,smallBorder/2 + rulerBorder/2,gridSeparation,rulerBorder), i.ToString(), gs);
            }
            EditorGUI.DrawRect(new Rect(i*gridSeparation + graphPan.x,smallBorder + rulerBorder*2/3,b,rulerBorder/3), rulerGUIColor);
            
        }

        for(int i = 0; i*gridSeparation <= position.height - graphPan.y; i++)
        {
            var b = 1;
            if(i % gridBold == 0 )
            {
                b = 0;
                //EditorGUILayout.TextField(i);
                //TODO: Texto para la regla
                var gs = EditorStyles.whiteLabel;
                gs.fontSize = 10;
                gs.alignment = UnityEngine.TextAnchor.MiddleLeft;
                EditorGUI.LabelField(new Rect(toolBarWidth, i*gridSeparation + graphPan.y - gridSeparation/2 ,rulerBorder, gridSeparation), i.ToString(), gs);
            }
            EditorGUI.DrawRect(new Rect(toolBarWidth + rulerBorder * 2 / 3, i*gridSeparation + graphPan.y,rulerBorder/3,b), rulerGUIColor);
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
            CreatwPrefab(goParent, fN, floorModules);
        }

        foreach (var oN in obstacleNodes)
        {

            CreatwPrefab(goParent, oN, obstacleModules);
        }

        foreach (var tr in triggerNodes)
        {

            CreatwPrefab(goParent, tr, triggerModules);
        }
    }

    private void CreatwPrefab(GameObject goParent, GridNode node, List<ModuleNode> list)
    {
        var pos = new Vector3(-node.gridX * moduleSize.x, 0f, node.gridY * moduleSize.y);
        var pf = PrefabUtility.InstantiatePrefab(list[node.id].prefab);
        var go = ((GameObject)pf);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;
        go.transform.SetParent(goParent.transform);
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

    public enum DuplicateToolTip
    {
        Copy,
        Move
    }
}
