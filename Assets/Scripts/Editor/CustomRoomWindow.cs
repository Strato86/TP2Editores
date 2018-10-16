using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public class CustomRoomWindow : EditorWindow {

    List<ModuleNode> modules;

    Rect roomGraph;
    float toolBarWidth = 250;
    float bottomBarheight = 50;

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
    GridNode[,] graphNodes;
    Vector2Int moduleSize;
    Vector2Int boardSize;

    GridNode pickedGridNode;

    string groupName = "";

    string prefabRoute = "LevelCreator";
    string prefabFolder = "Prefabs Painter";

    public static void OpenWindow(int amount, Vector2Int moduleSize, Vector2Int boardSize)
    {
        var w = (CustomRoomWindow)GetWindow(typeof(CustomRoomWindow));
        w.modules = new List<ModuleNode>();
        w.graphPan = new Vector2(w.toolBarWidth, w.smallBorder);
        w.roomGraph = new Rect(w.toolBarWidth, w.smallBorder, 1000000, 1000000);

        if(boardSize == Vector2Int.zero)
        {
            boardSize = new Vector2Int(80, 50);
        }
        w.graphNodes = new GridNode[boardSize.x,boardSize.y];
        w.moduleSize = moduleSize;
        w.boardSize = boardSize;
        w.pickedGridNode = new GridNode(0, 0, 0, 0, Color.clear,-1);

        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                w.graphNodes[j, i] = new GridNode(i * w.gridSeparation, j * w.gridSeparation, w.gridSeparation, w.gridSeparation, Color.clear, -1);
                EditorGUI.DrawRect(w.graphNodes[j, i].rect, w.graphNodes[j, i].color);
            }
        }

        if (!AssetDatabase.IsValidFolder("Assets/LevelDesign"))
        {
            AssetDatabase.CreateFolder("Assets", "LevelDesign");
        }
        if (!AssetDatabase.IsValidFolder("Assets/LevelDesign/" + w.prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets/LevelDesign", w.prefabFolder);
        }
        string[] folders = new string[1];
        folders[0] = "Assets/LevelDesign/" + w.prefabFolder;
        var paths = AssetDatabase.FindAssets("t: Object", folders);
        
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);

            var pf = new ModuleNode();
            pf.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            w.modules.Add(pf);
        }

    }

    private void OnGUI()
    {
        
        CheckMouseInput(Event.current);
        EditorGUILayout.BeginVertical(GUILayout.Height(position.height - bottomBarheight + 20));

        roomGraph.x = graphPan.x;
        roomGraph.y = graphPan.y;
        EditorGUI.DrawRect(new Rect(toolBarWidth, smallBorder, position.width - toolBarWidth - smallBorder, position.height - bottomBarheight), Color.gray);
        

        GUI.BeginGroup(roomGraph);

        BeginWindows();
        //Board Nodes
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                graphNodes[j, i].rect = new Rect(i * gridSeparation, j * gridSeparation, gridSeparation, gridSeparation);
                var c = graphNodes[j, i].id < 0 ? graphNodes[j, i].color : modules[graphNodes[j, i].id].color;
                
                EditorGUI.DrawRect(graphNodes[j, i].rect, c);

            }
        }
        //Horizontal Lines
        for (int i = 0; i * gridSeparation + graphPan.y <= position.height - bottomBarheight && i <= boardSize.y; i++)
        {
            var b = i % gridBold == 0 ? 2 : 1;
            //var width = boardSize.x * gridSeparation < position.width - toolBarWidth - smallBorder ? boardSize.x * gridSeparation + 2 + graphPan.x : position.width - smallBorder;
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
        EditorGUILayout.LabelField("Prefab Amount: " + modules.Count);

        var add = GUILayout.Button("Add Prefab");
        if (add)
        {
            var pf = new ModuleNode();
            modules.Add(pf);
        }
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


        scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Width(250));
        //Prefabs
        for (int i = 0; i < modules.Count; i++)
        {
            DrawLine(Color.gray);
            modules[i].id = i;
            EditorGUILayout.LabelField("id: " + (modules[i].id).ToString());
            modules[i].color = EditorGUILayout.ColorField("Color", modules[i].color);
            modules[i].color = new Color(modules[i].color.r, modules[i].color.g, modules[i].color.b, 1);
            modules[i].prefab = (GameObject)EditorGUILayout.ObjectField(modules[i].prefab, typeof(GameObject), true);
            if (modules[i].prefab)
            {
                var p = GUILayout.Button("Pick");
                if (p)
                {
                    pickedGridNode.SetColorAndID(modules[i].color, modules[i].id);
                }
            }

        }
        EditorGUILayout.EndScrollView();
       
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Zoom: ", GUILayout.Width(50));
        var zoomIn = GUILayout.Button("+", GUILayout.Width(20));
        var zoomOut = GUILayout.Button("-", GUILayout.Width(20));

        if (zoomIn) gridSeparation++;
        if (zoomOut) gridSeparation = gridSeparation <= 10 ? 10 : gridSeparation - 1;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            for (int i = 0; i < boardSize.y; i++)
            {
                for (int j = 0; j < boardSize.x; j++)
                {
                    graphNodes[i, j] = new GridNode(i * gridSeparation, j * gridSeparation, gridSeparation,gridSeparation, Color.clear, -1);
                    EditorGUI.DrawRect(graphNodes[i, j].rect, graphNodes[i, j].color);
                }
            }
        }
        
        //GUILayout.Button("Paint");
        if (GUILayout.Button("Erase"))
        {
            pickedGridNode.SetColorAndID(Color.clear, -1);
        }
        if (GUILayout.Button("Create"))
        {
            CreateRoom(groupName);
        }
        EditorGUILayout.EndHorizontal();
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

        if(current.button == 0 && (current.type == EventType.MouseDrag || current.type == EventType.MouseDown))
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
        if(x < boardSize.x && y < boardSize.y)
        {
            graphNodes[x, y].SetColorAndID(pickedGridNode.color, pickedGridNode.id);
            Repaint();
        }
    }

    private void CreateRoom(string groupName)
    {
        var goParent = new GameObject(groupName);
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                var id = graphNodes[i, j].id;
                if (id != -1)
                {
                    var pos = new Vector3(i * moduleSize.x, 0f, j * moduleSize.y);
                    var pf = PrefabUtility.InstantiatePrefab(modules[id].prefab);
                    var go = ((GameObject)pf);
                    go.transform.position = pos;
                    go.transform.rotation = Quaternion.identity;
                    go.transform.SetParent(goParent.transform);
                }
            }
        }
    }

    private void DrawLine(Color col, bool bold = false, bool vertical = false)
    {
        EditorGUILayout.Space();
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), col);
        EditorGUILayout.Space();
    }
}
