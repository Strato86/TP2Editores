using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public class CustomRoomWindow : EditorWindow {

    List<ModuleNode> floorModules;
    List<ModuleNode> obstacleModules;
    List<ModuleNode> enemiesModules;
    List<ModuleNode> waypointModules;
    List<ModuleNode> triggerModules;

    string[] _roomsToLoad;
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
    List<GridNode> waypointNodes;
    //  List<GridNode> enemiesNodes;
    List<GridNode> triggerNodes;
    Vector2Int moduleSize;
    Vector2Int boardSize;

    Dictionary<ModuleNode, List<GridNode>> enemiesPath = new Dictionary<ModuleNode, List<GridNode>>();

    Layers layer;
    bool floorlayer = true;
    bool obstaclesLayer;
    bool enemiesLayer;
    bool waypointLayer;
    bool eventLayer;

    Tools selectedTool;
    DuplicateToolTip duplicateToolTip;

    //Duplicate Tool Variables
    List<GridNode> duplicateFloorGroup;
    List<GridNode> duplicateObstacleGroup;
    List<GridNode> duplicateTriggerGroup;
    List<GridNode> duplicateWaypointGroup;
    List<GridNode> duplicateEnemiesGroup;
    Vector2 firstSelection;
    Vector2 lastSelection;


    string groupName = "";
    string floorFolder = "Floor prefabs";
    string obstaclesFolder = "Obstacle prefabs";
    string triggerFolder = "Trigger Prefabs";
    string enemiesFolder = "Enemies Prefabs";
    string dataFolder = "SavedRoom";

    Editor _prev;
    private Color pencilColor = Color.white;
    private Color defaultColor;
    private int lastPencilId=0;

    //Save variables
    string saveName = "";
    string loadName = "";


    public static void OpenWindow(int amount, Vector2Int moduleSize, Vector2Int boardSize)
    {
        var w = (CustomRoomWindow)GetWindow(typeof(CustomRoomWindow));
        w.floorModules = new List<ModuleNode>();
        w.obstacleModules = new List<ModuleNode>();
        w.enemiesModules = new List<ModuleNode>();
        w.waypointModules = new List<ModuleNode>();
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
        w.enemiesPath = new Dictionary<ModuleNode, List<GridNode>>();
        w.triggerNodes = new List<GridNode>();
        w.waypointNodes = new List<GridNode>();
        w.duplicateFloorGroup = new List<GridNode>();
        w.duplicateObstacleGroup = new List<GridNode>();
        w.duplicateEnemiesGroup = new List<GridNode>();
        w.duplicateTriggerGroup = new List<GridNode>();
        w.duplicateWaypointGroup = new List<GridNode>();

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

        if (!AssetDatabase.IsValidFolder("Assets/LevelDesign/" + w.enemiesFolder))
        {
            AssetDatabase.CreateFolder("Assets/LevelDesign", w.enemiesFolder);
        }

        if (!AssetDatabase.IsValidFolder("Assets/LevelDesign/" + w.dataFolder))
        {
            AssetDatabase.CreateFolder("Assets/LevelDesign", w.dataFolder);
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
        Debug.Log("busco en la carpeta" + folders[0]);
        paths = AssetDatabase.FindAssets("t: Object", folders);

        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            Debug.Log("hay algo en la carpeta");
            var pf = new ModuleNode();
            pf.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            w.obstacleModules.Add(pf);
        }

        folders[0] = "Assets/LevelDesign/" + w.enemiesFolder;
        Debug.Log("busco en la carpeta" + folders[0]);
        paths = AssetDatabase.FindAssets("t: Object", folders);

        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            Debug.Log("hay algo en la carpeta");
            var pf = new ModuleNode();
            pf.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            w.enemiesPath[pf] = new List<GridNode>();
            w.enemiesModules.Add(pf);
        }


        folders[0] = "Assets/LevelDesign/" + w.triggerFolder;
        Debug.Log("busco en la carpeta" + folders[0]);
        paths = AssetDatabase.FindAssets("t: Object", folders);

        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            Debug.Log("hay algo en la carpeta");
            var pf = new ModuleNode();
            pf.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            w.triggerModules.Add(pf);
        }

        folders[0] = "Assets/LevelDesign/" + w.dataFolder;
        paths = AssetDatabase.FindAssets("t: ScriptableObject", folders);
        
        w._roomsToLoad = new string[paths.Length];
        for(int i = 0; i< paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            var splitedPath = paths[i].Split('/');
            paths[i] = splitedPath[splitedPath.Length - 1];
            var withoutExtension = paths[i].Split('.');
            w._roomsToLoad[i] = withoutExtension[0];
        }
        w.minSize = new Vector2(500,350);
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
        /*
        if (enemiesLayer)
        {
            foreach (var eN in enemiesModules)
            {
                for (int i = 0; i < enemiesPath[eN].Count; i++)
                {
                    GridNode point = enemiesPath[eN][i];
                    point.rect.width = gridSeparation;
                    point.rect.height = gridSeparation;
                    point.rect.x = point.gridX * gridSeparation;
                    point.rect.y = point.gridY * gridSeparation;
                    //point.color = enemiesPath[eN].color;
                    //EditorGUI.DrawRect(point.rect, point.color);
                    GUI.color = point.color;
                    var r = new Rect(point.rect.x , point.rect.y , gridSeparation , gridSeparation);
                    GUI.color = defaultColor;
                    GUI.Box(r, i.ToString());
                    GUI.color = defaultColor;
                    //EditorGUI.DrawRect(r, c);
                } 
            }
        }*/

        if (waypointLayer)
        {

            for (int i = 0; i < waypointNodes.Count; i++)
            {
                GridNode point = waypointNodes[i];
                point.rect.width = gridSeparation;
                point.rect.height = gridSeparation;
                point.rect.x = point.gridX * gridSeparation;
                point.rect.y = point.gridY * gridSeparation;
                //point.color = enemiesPath[eN].color;
                //EditorGUI.DrawRect(point.rect, point.color);
                GUI.color = point.color;
                var r = new Rect(point.rect.x, point.rect.y, gridSeparation, gridSeparation);
                GUI.color = defaultColor;
                GUI.Box(r, i.ToString());
                GUI.color = defaultColor;
                //EditorGUI.DrawRect(r, c);
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
        if (GUILayout.Button((Texture2D)Resources.Load("selection"), opts))
        {

            pickedGridNode.SetColorAndID(Color.clear, -1);
            selectedTool = Tools.Duplicate;
        }

        GUI.color = defaultColor;

        if (selectedTool == Tools.DataManagement)
        {
            GUI.color = Color.yellow;
        }
        if (GUILayout.Button((Texture2D)Resources.Load("save"), opts))
        {
            selectedTool = Tools.DataManagement;
        }

        GUI.color = defaultColor;

        EditorGUILayout.EndHorizontal();


   

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Active Layer", GUILayout.Width(100));
        layer = (Layers)EditorGUILayout.EnumPopup(layer,GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Width(250), GUILayout.Height(Mathf.Max(40,(position.height - bottomBarheight - 250))));
        //Prefabs
        switch(selectedTool)
        {
            case Tools.Brush:
            switch(layer)
            {
                case Layers.Floor:
                    if(pickedGridNode.id >= floorModules.Count) pickedGridNode.id = floorModules.Count - 1;
                    for (int i = 0; i < floorModules.Count; i++)
                    {
                        DrawPrefabModule(floorModules, i);
                    }
                    break;
                case Layers.Obstacles:
                    if(pickedGridNode.id >= obstacleModules.Count) pickedGridNode.id = obstacleModules.Count - 1;
                    for(int i = 0; i< obstacleModules.Count; i++)
                    {
                        DrawPrefabModule(obstacleModules, i);
                        
                    }
                    break;
                case Layers.Enemies:
                    if (pickedGridNode.id >= enemiesModules.Count) pickedGridNode.id = obstacleModules.Count - 1;
                    for (int i = 0; i < enemiesModules.Count; i++)
                    {
                        DrawPrefabModule(enemiesModules, i);
                        DrawPath(i);

                    }
                    break;
                case Layers.Waypoint:
                    try
                    {
                        for (int x = 0; x < waypointNodes.Count; x++)
                        {
                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label("Punto: " + x);
                            if (GUILayout.Button("Delete"))
                            {
                                waypointNodes.RemoveAt(x);
                                Repaint();
                                break;
                            }
                            if (x < waypointNodes.Count - 1 && GUILayout.Button("+"))
                            {
                                var point = waypointNodes[x];
                                waypointNodes.RemoveAt(x);
                                waypointNodes.Insert(x + 1, point);
                                Repaint();
                                break;
                            }
                            if (x > 0 && GUILayout.Button("-"))
                            {
                                var point = waypointNodes[x];
                                waypointNodes.RemoveAt(x);
                                waypointNodes.Insert(x - 1, point);
                                Repaint();
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                            //Repaint();
                        }
                    }
                    catch (Exception e)
                    {
                        //Debug.Log(e.Message);
                    }
                    break;
                case Layers.EventTriggers:
                    if (pickedGridNode.id >=triggerModules.Count) pickedGridNode.id = triggerModules.Count - 1;
                    for (int i = 0; i < triggerModules.Count; i++)
                    {
                        DrawPrefabModule(triggerModules, i);
                    }
                        //   DrawPrefabModuleFromList(triggerModules);
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
                DrawLine(Color.gray);
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

            case Tools.DataManagement:
                DrawLine(Color.gray);
                GUIStyle myS = new GUIStyle();
                myS.fontSize = 18;
                myS.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField("Save Room", myS);
                
                EditorGUILayout.BeginHorizontal(GUILayout.Width(toolBarWidth - 14));
                EditorGUILayout.LabelField("Room name to save", GUILayout.Width(toolBarWidth/2 - 7));
                saveName = EditorGUILayout.TextField(saveName,GUILayout.Width(toolBarWidth/2 - 7));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("Automatic Overwrite", MessageType.Warning);

                if(GUILayout.Button("Save"))
                {
                    if(saveName != "")
                    {
                        var rd = ScriptableObject.CreateInstance<RoomData>();;
                        
                        rd.floorNodes = new List<GridNode>();
                        rd.obstacleNodes = new List<GridNode>();
                        rd.enemiesPath = new Dictionary<ModuleNode, List<GridNode>>();
                        rd.waypointNodes = new List<GridNode>();
                        rd.triggerNodes = new List<GridNode>();

                        foreach(var fn in floorNodes)
                        {
                            //float x, float y, float width, float heigth, Color col, int id, int gX, int gY
                            var n = new GridNode(fn.gridX * gridSeparation, fn.gridY * gridSeparation ,gridSeparation,gridSeparation, fn.color, fn.id, fn.gridX, fn.gridY);
                            rd.floorNodes.Add(n);
                        }
                        foreach(var on in obstacleNodes)
                        {
                            //float x, float y, float width, float heigth, Color col, int id, int gX, int gY
                            var n = new GridNode(on.gridX * gridSeparation, on.gridY * gridSeparation ,gridSeparation,gridSeparation, on.color, on.id, on.gridX, on.gridY);
                            rd.obstacleNodes.Add(n);
                        }
                        
                        foreach (var enemy in enemiesPath.Keys)
                        {
                            List<GridNode> list = new List<GridNode>();
                            foreach (var on in enemiesPath[enemy])
                            {

                                var n = new GridNode(on.gridX * gridSeparation, on.gridY * gridSeparation, gridSeparation, gridSeparation, on.color, on.id, on.gridX, on.gridY);
                                list.Add(n);

                            }
                            rd.enemiesPath[enemy]= list;
                            //float x, float y, float width, float heigth, Color col, int id, int gX, int gY
                        }

                        foreach (var wn in waypointNodes)
                        {
                            //float x, float y, float width, float heigth, Color col, int id, int gX, int gY
                            var n = new GridNode(wn.gridX * gridSeparation, wn.gridY * gridSeparation, gridSeparation, gridSeparation, wn.color, wn.id, wn.gridX, wn.gridY);
                            rd.waypointNodes.Add(n);
                        }

                        foreach (var tn in triggerNodes)
                        {
                            //float x, float y, float width, float heigth, Color col, int id, int gX, int gY
                            var n = new GridNode(tn.gridX * gridSeparation, tn.gridY * gridSeparation ,gridSeparation,gridSeparation, tn.color, tn.id, tn.gridX, tn.gridY);
                            rd.triggerNodes.Add(n);
                        }
                        rd.roomName = groupName;
                        RoomDataUtility.SaveRoom(saveName, rd);

                        string[] folders = new string[1];
                        folders[0] = "Assets/LevelDesign/" + dataFolder;
                        var paths = AssetDatabase.FindAssets("t: ScriptableObject", folders);
                        
                        _roomsToLoad = new string[paths.Length];
                        for(int i = 0; i< paths.Length; i++)
                        {
                            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
                            var splitedPath = paths[i].Split('/');
                            paths[i] = splitedPath[splitedPath.Length - 1];
                            var withoutExtension = paths[i].Split('.');
                            _roomsToLoad[i] = withoutExtension[0];
                        }
                        rd.roomName = groupName;

                    }else
                    {
                        EditorGUILayout.HelpBox("Empty Name", MessageType.Error);
                    }
                    Repaint();

                }

                DrawLine(Color.grey);

                myS.fontSize = 18;
                myS.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField("Load Room", myS);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(toolBarWidth - 14));
                EditorGUILayout.LabelField("Room name to load", GUILayout.Width(toolBarWidth/2 - 7));
                loadName = EditorGUILayout.TextField(loadName,GUILayout.Width(toolBarWidth/2 - 7));
                EditorGUILayout.EndHorizontal();   

                if(GUILayout.Button("Load"))
                {
                    
                    var rd = RoomDataUtility.LoadRoom(loadName);
                    if(rd != null)
                    {
                        floorNodes = new List<GridNode>();
                        obstacleNodes = new List<GridNode>();
                        enemiesPath = new Dictionary<ModuleNode, List<GridNode>>();
                        triggerNodes = new List<GridNode>();
                        waypointNodes = new List<GridNode>();

                        foreach (var fn in rd.floorNodes)
                        {
                            var n = new GridNode(fn.gridX * gridSeparation, fn.gridY * gridSeparation ,gridSeparation,gridSeparation, fn.color, fn.id, fn.gridX, fn.gridY);
                            floorNodes.Add(n);
                        }

                        foreach(var on in rd.obstacleNodes)
                        {
                            var n = new GridNode(on.gridX * gridSeparation, on.gridY * gridSeparation ,gridSeparation,gridSeparation, on.color, on.id, on.gridX, on.gridY);
                            obstacleNodes.Add(n);
                        }

                        foreach (var enemy in enemiesPath.Keys)
                        {
                            List<GridNode> list = new List<GridNode>();
                            foreach (var on in enemiesPath[enemy])
                            {

                                var n = new GridNode(on.gridX * gridSeparation, on.gridY * gridSeparation, gridSeparation, gridSeparation, on.color, on.id, on.gridX, on.gridY);
                                list.Add(n);

                            }
                            rd.enemiesPath[enemy] = list;
                        }

                        foreach (var tn in rd.triggerNodes)
                        {
                            var n = new GridNode(tn.gridX * gridSeparation, tn.gridY * gridSeparation ,gridSeparation,gridSeparation, tn.color, tn.id, tn.gridX, tn.gridY);
                            triggerNodes.Add(n);
                        }

                        foreach (var wn in rd.waypointNodes)
                        {
                            var n = new GridNode(wn.gridX * gridSeparation, wn.gridY * gridSeparation, gridSeparation, gridSeparation, wn.color, wn.id, wn.gridX, wn.gridY);
                            waypointNodes.Add(n);
                        }

                        groupName = rd.roomName;
                    }else
                    {
                        EditorGUILayout.HelpBox("No saved room with that name", MessageType.Error);
                        Repaint();
                    }
                    
                }

                EditorGUILayout.LabelField("Saved Room List:", EditorStyles.boldLabel);
                myS.fontSize = 10;
                myS.fontStyle = FontStyle.Italic;
                scrollView = EditorGUILayout.BeginScrollView(scrollView);
                for(int i = 0; i < _roomsToLoad.Length; i++)
                {
                    
                    EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 1), Color.grey);
                    
                    EditorGUILayout.LabelField(_roomsToLoad[i], myS);
                }
                EditorGUILayout.EndScrollView();
                break;
        }
        
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Reset Room"))
        {
            floorNodes = new List<GridNode>();
            obstacleNodes = new List<GridNode>();
            enemiesPath = new Dictionary<ModuleNode, List<GridNode>>();
            triggerNodes = new List<GridNode>();
            waypointNodes = new List<GridNode>();
            groupName = "";
        }
        if (GUILayout.Button("Create Room"))
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
        waypointLayer = EditorGUILayout.Toggle("Waypoint", waypointLayer);
        eventLayer = EditorGUILayout.Toggle("Event Trigger", eventLayer);
    
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawPath(int i)
    {
        try
        {
            for (int x = 0; x < enemiesPath[enemiesModules[i]].Count; x++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Punto: " + x);
                if (GUILayout.Button("Delete"))
                {
                    enemiesPath[enemiesModules[i]].RemoveAt(x);
                    Repaint();
                    break;
                }
                if (x < enemiesPath[enemiesModules[i]].Count - 1 && GUILayout.Button("+"))
                {
                    var point = enemiesPath[enemiesModules[i]][x];

                    enemiesPath[enemiesModules[i]].RemoveAt(x);
                    enemiesPath[enemiesModules[i]].Insert(x + 1, point);
                    Repaint();
                    break;
                }
                if (x > 0 && GUILayout.Button("-"))
                {
                    var point = enemiesPath[enemiesModules[i]][x];
                    enemiesPath[enemiesModules[i]].RemoveAt(x);
                    enemiesPath[enemiesModules[i]].Insert(x - 1, point);
                    Repaint();
                    break;
                }
                EditorGUILayout.EndHorizontal();
                //Repaint();
            }
        }
        catch (Exception e)
        {
            //Debug.Log(e.Message);
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

            case Layers.Enemies:
                foreach (var enemy in enemiesPath.Keys)
                {
                    if (pickedGridNode.id == enemy.id) {
                        PaintSquareScreen(x, y, enemiesPath[enemy]);
                    }
                }
                break;

            case Layers.Waypoint:
                PaintSquareScreen(x, y, waypointNodes);
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
        else if (id <= 0 && layer== Layers.Waypoint && selectedTool == Tools.Brush)
        {
            if (!isOcupied)
            {
                var g = new GridNode(x * gridSeparation, y * gridSeparation, gridSeparation, gridSeparation, pickedGridNode.color, pickedGridNode.id, x, y);
                gridList.Add(g);
            }
            else
            {
                auxNode.SetColorAndID(pickedGridNode.color, 0);
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
            if (enemiesLayer)
            {
                // TODO hacer que se mueva
                //SelectMovingGroup(enemiesNodes, duplicateEnemiesGroup);
            }
            if (waypointLayer)
            {
                // TODO hacer que se mueva
                //SelectMovingGroup(waypointNodes, duplicateWaypointGroup);
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

        if (enemiesLayer)
        {
            // TODO hacer que se mueva
            //MoveGroup(enemiesNodes, duplicateEnemiesGroup, x, y, minX, minY);
        }
        if (waypointLayer)
        {
            // TODO hacer que se mueva
            //MoveGroup(waypoint, duplicateWaypointGroup, x, y, minX, minY);
        }

        if (eventLayer)
        {
            MoveGroup(triggerNodes, duplicateTriggerGroup, x, y, minX, minY);
        }

        if (duplicateToolTip == DuplicateToolTip.Move)
        {
            duplicateFloorGroup = new List<GridNode>();
            duplicateObstacleGroup = new List<GridNode>();
            duplicateEnemiesGroup = new List<GridNode>();
            duplicateWaypointGroup = new List<GridNode>();
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
                DrawObjectSelectionPreview(duplicateFloorGroup[i].gridX + x - minX, duplicateFloorGroup[i].gridY + y - minY, Color.blue);
            }
        }
        if(obstaclesLayer)
        {
            for(int i = 0 ; i< duplicateObstacleGroup.Count; i++)
            {
                DrawObjectSelectionPreview(duplicateObstacleGroup[i].gridX + x - minX, duplicateObstacleGroup[i].gridY + y - minY, Color.red);
            }
        }

        if (enemiesLayer)
        {
            for (int i = 0; i < duplicateEnemiesGroup.Count; i++)
            {
                DrawObjectSelectionPreview(duplicateEnemiesGroup[i].gridX + x - minX, duplicateEnemiesGroup[i].gridY + y - minY, Color.green);
            }
        }

        if (waypointLayer)
        {
            for (int i = 0; i < duplicateWaypointGroup.Count; i++)
            {
                DrawObjectSelectionPreview(duplicateWaypointGroup[i].gridX + x - minX, duplicateWaypointGroup[i].gridY + y - minY, Color.yellow);
            }
        }

        if (eventLayer)
        {
            for (int i = 0; i < duplicateTriggerGroup.Count; i++)
            {
                DrawObjectSelectionPreview(duplicateTriggerGroup[i].gridX + x - minX, duplicateTriggerGroup[i].gridY + y - minY, Color.magenta);
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
                //TODO arreglar
            /*    if (enemiesLayer)
                {
                    foreach (var on in enemiesNodes)
                    {
                        if (on.gridX == j && on.gridY == i)
                        {
                            duplicateEnemiesGroup.Add(on);
                        }
                    }
                }
                */
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

    void DrawObjectSelectionPreview(int x, int y, Color color)
    {
        var c = color;
        
        c = new Color(c.r,c.g,c.b,c.a/2);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation,gridSeparation,gridSeparation), c);

        c = color;
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, 2, gridSeparation),c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation + gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation + gridSeparation, y*gridSeparation, 2, gridSeparation),c);
    }

   /* void DrawObstacleSelectionPreview(int x, int y)
    {
        //TODO: hay que hacer una 
        var c = Color.red;

        c = new Color(c.r,c.g,c.b,c.a/2);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation,gridSeparation,gridSeparation), c);

        c = Color.red;
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation, 2, gridSeparation),c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation,y*gridSeparation + gridSeparation, gridSeparation,2), c);
        EditorGUI.DrawRect(new Rect(x*gridSeparation + gridSeparation, y*gridSeparation, 2, gridSeparation),c);
    }*/
    
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
            CreatePrefab(goParent, fN, floorModules);
        }

        foreach (var oN in obstacleNodes)
        {

            CreatePrefab(goParent, oN, obstacleModules);
        }

        foreach (var eN in enemiesPath)
        {
            if (eN.Value.Count == 0) continue;
            GameObject enemy= CreatePrefab(goParent,eN.Value[0] , enemiesModules);
            List<Vector3> posiciones= new List<Vector3>();
          

            if (enemy.GetComponent<FollowPath>() == null) continue;
            foreach (var point in eN.Value)
            {
                var pos = new Vector3(-point.gridX * moduleSize.x, 0, point.gridY * moduleSize.y);
                posiciones.Add(pos);
            }
            enemy.GetComponent<FollowPath>().Set(posiciones);
            

        }
        
        foreach (var tr in triggerNodes)
        {

            CreatePrefab(goParent, tr, triggerModules, moduleSize.y);
        }
    }

    private GameObject CreatePrefab(GameObject goParent, GridNode node, List<ModuleNode> list, float height=0)
    {
        var pos = new Vector3(-node.gridX * moduleSize.x, height, node.gridY * moduleSize.y);
        var pf = PrefabUtility.InstantiatePrefab(list[node.id].prefab);
        var go = ((GameObject)pf);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;
        go.transform.SetParent(goParent.transform);
        return go;
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
        EventTriggers,
        Waypoint
    }

    public enum Tools
    {
        Brush,
        Eraser,
        Duplicate,
        DataManagement
    }

    public enum DuplicateToolTip
    {
        Copy,
        Move
    }
}
