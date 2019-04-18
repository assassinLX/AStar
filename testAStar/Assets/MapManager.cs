using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

[ExecuteInEditMode]
public class MapManager : EditorWindow
{
    private static MapManager window;
    private MapManager()
    {

    }
    public static MapManager GetInstance()
    {
        if (window != null)
        {
            return window;
        }
        else
        {
            Setting();
            return window;
        }
    }

    private int _width = 30;
    private int _height = 30;

    public int width
    {
        get{
            return _width;
        }
    }
    public int height {
        get
        {
            return _height;
        }  
    }
    private const int interval = 3;

    public Node[,] allGrid;
    public List<Node> wallList;

    public Node[,] getAllGrid()
    {
        return allGrid;
    }

    public List<Node> getWallList()
    {
        return wallList;
    }

    [MenuItem("MapManager/生成地图", false, 1)]
    private static void initAllGrid()
    {
        if (window == null)
        {
            GetInstance();
        }

        if (window.allGrid == null)
        {
            window.allGrid = new Node[window._width, window._height];
            for (int i = 0; i < window._width; i++)
            {
                for (int t = 0; t < window._height; t++)
                {
                    window.allGrid[i, t] = new Node(GameObject.CreatePrimitive(PrimitiveType.Plane));
                    window.allGrid[i, t].transform.position = new Vector3(i * interval, 0, t * interval);
                    window.allGrid[i, t].xIndex = i;
                    window.allGrid[i, t].yIndex = t;
                    window.allGrid[i, t].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    window.allGrid[i, t].transform.name = string.Format("{0},{1}", i, t);

                }
            }

        }
    }

 

    [MenuItem("MapManager/Setting %m",false,0)]
    private static void Setting()
    {
        Rect rect = new Rect(100, 100, 250, 500);
        window = (MapManager)EditorWindow.GetWindowWithRect(typeof(MapManager),rect,false,"Setting");
        window.Show();
    }













    void OnGUI()
    {
        GUI.UnfocusWindow();
        if (GUILayout.Button("设置Wall", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;

        }
        if (GUILayout.Button("清理Wall", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;

        }

        if (GUILayout.Button("设置开始点", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;

        }

        if (GUILayout.Button("清理开始点", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;

        }
        if (GUILayout.Button("设置结束点", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;

        }

        if (GUILayout.Button("清理结束点", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;

        }


    }
}
