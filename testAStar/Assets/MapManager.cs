using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

//[ExecuteInEditMode]
public class MapManager : EditorWindow
{

    FindNode findNode;
    private static MapManager window;
    private MapManager(){}
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
    public List<Node> wallList = new List<Node>();
    public Node startNode;
    public Node endNode;

    public Node[,] getAllGrid()
    {
        return allGrid;
    }
    public List<Node> getWallList()
    {
        return wallList;
    }
    
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

    Node findAllGridNode(GameObject obj)
    {
        for (int i = 0; i < _width; i++)
        {
            for (int t = 0; t < _height; t++)
            {
                if (object.ReferenceEquals(allGrid[i, t].transform.gameObject,obj))
                {
                    Debug.Log(allGrid[i,t].transform.name);
                    return allGrid[i, t];
                }
                
            }
        }
        return null;
    }

    void clearAllGridNode()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int t = 0; t < _height; t++)
            {
                if (!wallList.Contains(allGrid[i,t]))
                {
                    allGrid[i, t].transform.GetComponent<Renderer>().material.color = Color.white;
                    allGrid[i, t].all = 0;
                    allGrid[i, t].endDistance = 0;
                    allGrid[i, t].distance = 0;
                }
            }
        }

    }

    [MenuItem("MapManager/Setting %m",false,0)]
    private static void Setting()
    {
        Rect rect = new Rect(100, 100, 150, 500);
        window = (MapManager)EditorWindow.GetWindowWithRect(typeof(MapManager),rect,false,"Setting");
        window.Show();
    }


    void OnGUI()
    {
        GUI.UnfocusWindow();
        //绘制标题
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("地图设置");


        GUILayout.Space(10);
        findNode = (FindNode)EditorGUILayout.ObjectField("Find Node", findNode, typeof(FindNode), true);

        if (GUILayout.Button("生成地图", GUILayout.Width(80), GUILayout.Height(20)))
        {
            //EditorApplication.playmodeStateChanged += initAllGrid;
            if (EditorApplication.isPlaying == false)
            {
                EditorApplication.isPlaying = true;
            }
            initAllGrid();
        }

        if (GUILayout.Button("设置Wall", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;
            foreach (var node in transforms)
            {
                Node curNode = findAllGridNode(node.transform.gameObject);
                if (!wallList.Contains(curNode))
                {
                    curNode.transform.GetComponent<Renderer>().material.color = Color.black;
                    wallList.Add(curNode);
                }
            }

        }
        if (GUILayout.Button("清理Wall", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;
            foreach(var node in transforms)
            {
                for (int i = 0; i < wallList.Count; i++)
                {
                    if (Object.ReferenceEquals(node,wallList[i].transform))
                    {
                        node.transform.GetComponent<Renderer>().material.color = Color.white;
                        wallList.RemoveAt(i);
                    }
                }
            }
        }

        if (GUILayout.Button("设置开始点", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;
            if (transforms.Length > 0)
            {
                startNode = findAllGridNode(transforms[0].transform.gameObject);
                startNode.transform.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        
        if (GUILayout.Button("设置结束点", GUILayout.Width(80), GUILayout.Height(20)))
        {
            Transform[] transforms = Selection.transforms;
            if (transforms.Length > 0)
            {
                endNode = findAllGridNode(transforms[0].transform.gameObject);
                endNode.transform.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
        if (GUILayout.Button("清理", GUILayout.Width(80), GUILayout.Height(20)))
        {
            if (findNode != null)
            {
                findNode.clear();
                clearAllGridNode();
            }
        }

        if (GUILayout.Button("开始寻路", GUILayout.Width(80), GUILayout.Height(20)))
        {
            if (findNode != null)
            {
                if (findNode.openList.Count != 0)
                {
                    Debug.LogErrorFormat("需要清理才能寻路");
                }
                else
                {
                    findNode.settingData();
                    findNode.handleFindNode();
                }
            }
        }
    }
}

