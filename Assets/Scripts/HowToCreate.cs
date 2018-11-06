
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class HowToCreate : EditorWindow
{
    private GUIStyle _labelStyle;
    private GUIStyle _btextStyle;
    private Color _arrowColor;
    private Color _textColor;
    private Vector2 _scrollPosition;
    private GUIStyle _textStyle;
    private int startWidth = 0;
    private int endWidth = 800;
    private bool wasPressed = false;
    public HowToUse howUse;
    private String headLabel;

    private void OnEnable()
    {
        StyleCreator();
    }

    private void StyleCreator()
    {
        _arrowColor = new Color(0.3f, 0.3f, 0.3f, 1);
        _textColor = new Color(0.9f, 0.9f, 0.9f, 1);

        _labelStyle = new GUIStyle();
        _labelStyle.fontSize = 18;
        _labelStyle.normal.textColor = _arrowColor;

        _textStyle = new GUIStyle();
        _textStyle.fontSize = 11;
        _textStyle.normal.textColor = _textColor;

        _btextStyle = new GUIStyle();
        _btextStyle.fontSize = 12;
        _btextStyle.normal.textColor = _textColor;
        _btextStyle.fontStyle = FontStyle.Bold;

    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(headLabel, EditorStyles.boldLabel);
        var line = new Rect(0, 0, 1, 1000);
        var line2 = new Rect(position.width - 1, 0, 1, 1000);
        var line3 = new Rect(0, 0, 1000, 1);
        var line4 = new Rect(0, position.height - 1, 1000, 1);

        EditorGUI.DrawRect(line, Color.black);
        EditorGUI.DrawRect(line2, Color.black);
        EditorGUI.DrawRect(line3, Color.black);
        EditorGUI.DrawRect(line4, Color.black);
        Features();

        GUILayout.BeginArea(new Rect(7, position.height/2 - 10, 40, 40));
        var margin = new Vector2(0, 0);
        var isPressed = false;
        isPressed = Handles.Button(margin, Quaternion.identity, 10, 15, Handles.CubeHandleCap);
        if (isPressed) wasPressed = true;
        EditorGUILayout.LabelField("«", _labelStyle);
        GUILayout.EndArea();
        Sizes();
    }

    private void Sizes()
    {
        if (startWidth < 800)
        {
            startWidth += 50;
            var self = (HowToCreate)HowToCreate.GetWindow(typeof(HowToCreate), true);
            self.position = new Rect(howUse.popPosx, howUse.popPosy, startWidth, 500);
        }
        if (wasPressed)
        {
            endWidth -= 50;
            var self = (HowToCreate)HowToCreate.GetWindow(typeof(HowToCreate), true);
            self.position = new Rect(howUse.popPosx, howUse.popPosy, endWidth, 500);
        }
        if (endWidth < 0)
        {
            Close(); howUse.winOpened = false;
        }
       /* if(startWidth >= 800 && endWidth == 800)
        {
            var self = (HowToCreate)HowToCreate.GetWindow(typeof(HowToCreate), true);
            self.position = new Rect(howUse.popPosx, howUse.popPosy, 800, 500);
        }*/
    }

    private void Features()
    {
        EditorGUI.DrawRect(new Rect(24,24, 752, 452), Color.black);
        EditorGUI.DrawRect(new Rect(25, 25, 750, 450), howUse.bgColor);

        GUILayout.BeginArea(new Rect(25, 25, 750, 450));
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
        if (!howUse) Close();

        switch (howUse.selectId)
        {
            case 0:
                Starting();
                break;
            case 1:
                BTools();
                break;
            case 2:
                ATools();
                break;
            case 3:
                Layers();
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
        }
        Repaint();

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }


    void Starting()
    {
        headLabel = "Starting";
        GUILayout.Label(" Getting started \n", _btextStyle);

        GUILayout.Label(" Welcome to the AMG2 Level Editor Tool. \n" +
                        " This tool will allow you to easily create and edit levels and maps in Unity, as well as manage custom events and enemy \n" +
                        " locations. \n \n " +
                        "In order to get started on the starting window press the button labeled New Level, this will take you to the level editor itself \n" +
                        " with a fresh new level. \n \n " +
                        "If you wish to load an already existing level then press the Load Level button, this will take you to the default save folder for \n" +
                        " saved levels. \n \n  " +
                        "In order to enter the Event Manager just press the Event Manager button", _textStyle);
    }

    void BTools()
    {
        headLabel = "Basic tools";
        GUILayout.Label(" Basic Tools \n", _btextStyle);
        GUILayout.Label(" In order to start designing your level, first you must select a color from the square texture, this will allow you to color the \n" +
           " spaces in the grid with your selected color by left-clicking on it thus generating tiles in the scene.\n\n", _textStyle);
        GUILayout.BeginHorizontal();
        GUI.DrawTexture(GUILayoutUtility.GetRect(25, 25, GUILayout.Width(35)), (Texture2D)Resources.Load("paint"), ScaleMode.ScaleToFit);
        GUILayout.Label("\n With the Paint tool you'll be able to paint on the map, generating both floors, obstacles, enemies and events depending \n on the layer selected", _textStyle);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.DrawTexture(GUILayoutUtility.GetRect(25, 25, GUILayout.Width(35)), (Texture2D)Resources.Load("eraser"), ScaleMode.ScaleToFit);
        GUILayout.Label("\n By selecting the Eraser tool you will be able to delete any tiles you've painted", _textStyle);
        GUILayout.EndHorizontal();
    }

    void ATools()
    {
        headLabel = "Advanced tools";
        GUILayout.Label(" Advanced Tools \n", _btextStyle);

        GUILayout.BeginHorizontal();
        GUI.DrawTexture(GUILayoutUtility.GetRect(15, 15, GUILayout.Width(35)), (Texture2D)Resources.Load("weapon"), ScaleMode.ScaleToFit);
        GUILayout.Label("\n By selecting the Sword Icon you will be able to select an area by right click and dragging your mouse cursor on the grid.\n", _textStyle);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        var rec = new Rect(14, 67, 123, 123);
        EditorGUI.DrawRect(rec, Color.black);
        GUI.DrawTexture(GUILayoutUtility.GetRect(141, 121, GUILayout.Width(151)), (Texture2D)Resources.Load("HowTo/selection"), ScaleMode.ScaleToFit);

        GUILayout.BeginArea(new Rect(145, 70, 400, 121));
        GUILayout.Label("After selecting an area you may Copy it or Move it around. ", _textStyle);
        GUILayout.EndArea();

        GUILayout.EndHorizontal();
    }

    void Layers()
    {
        headLabel = "Layers";
        GUILayout.Label(" Layers \n", _btextStyle);
    }
}
