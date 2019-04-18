using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private const int width = 50;
    private const int height = 50;
    private const int wallNum = width * height;
    private const int interval = 15;

    public Node[,] allGrid;
    public Node startNode;
    public Node endNode;
    private Node curStartNode;

    public Color startColor = Color.red;
    public Color endColor = Color.yellow;
    public Color wellColor = Color.black;
    public List<Node> wallObjList;
    public List<Node> openList;
    public List<Node> closeList;

	private void createList()
	{
		allGrid = new Node[width, height];
		wallObjList = new List<Node>();
        openList = new List<Node>();
        closeList = new List<Node>();
	}
    
	private void initAllGrid()
	{
		for (int i = 0; i < width; i++)
		{
			for (int t = 0; t < height; t++)
			{
				allGrid[i, t] = new Node(GameObject.CreatePrimitive(PrimitiveType.Plane));
				allGrid[i, t].transform.position = new Vector3(i * interval, 0, t * interval);
				allGrid[i, t].xIndex = i;
				allGrid[i, t].yIndex = t;
				allGrid[i, t].transform.name = string.Format("{0},{1}", i, t);

			}
		}
	}


	private void initWall()
	{


		for (int i = 3; i <= 11; i++)
		{
			wallObjList.Add(allGrid[4, i]);
		}
		//for (int i = 6; i <= 8; i++)
		//{
		//	wallObjList.Add(allGrid[10, i]);
		//}

		//for (int i = 4; i < 9; i++)
		//{
		//	wallObjList.Add(allGrid[6, i]);
		//}

		for (int i = 5; i < 45; i++)
		{
			wallObjList.Add(allGrid[i, 11]);
		}

        for (int i = 0; i < 4; i++)
        {
            wallObjList.Add(allGrid[i, 3]);
        }


        //for (int i = 4; i < 14; i++)
        //{
        //	wallObjList.Add(allGrid[i, 2]);
        //}

        //for (int i = 6; i < 11; i++)
        //{
        //	wallObjList.Add(allGrid[i, 9]);
        //}

        //for (int i = 4; i < 11; i++)
        //{
        //	wallObjList.Add(allGrid[12, i]);
        //}

        //for (int i = 7; i < 12; i++)
        //{
        //	wallObjList.Add(allGrid[i, 4]);
        //}
        //wallObjList.Add(allGrid[7, 7]);
        //wallObjList.Add(allGrid[8, 7]);

        for (int i = 0; i < wallObjList.Count; i++)
		{
			wallObjList[i].GetComponent<Renderer>().material.color = wellColor;
		}

	}


    private void Awake()
    {
        createList();
		initAllGrid();

        startNode = allGrid[45,45];
        endNode = allGrid[9, 7];
        curStartNode = startNode;
        startNode.GetComponent<Renderer>().material.color = startColor;
        endNode.GetComponent<Renderer>().material.color = endColor;
        initWall();
    }

    void Start () {
        handle();
    }



    private void handle()
    {
        calCurrentGoGrid(curStartNode);
        string[] args = new string[] { "all", "up", "endDistance","up" };
        sortOn(openList, args);

        if (openList.Count == 0)
        {
            return;
        }

        if (isCanGoNode(endNode, curStartNode))
        {
            Node temp = curStartNode;
			while (temp != startNode)
			{
				temp.GetComponent<Renderer>().material.color = Color.green;
                temp = temp.father;
			}
            return;
        }
		
        Node firstStep = null;
        if (openList.Count > 0)
		{
			firstStep = openList[0];
		}

        if (firstStep == null){
            return;
        }

        if (firstStep != null )
		{
            curStartNode = firstStep;
			closeList.Add(curStartNode);
			
			curStartNode.GetComponent<Renderer>().material.color = startColor;
            openList.Remove(firstStep);
		}
        Invoke("handle", 0.01f);
        //handle();
    }



    private bool isCanGoNode(Node tagNode,Node curStart)
    {
        var difX = (tagNode.xIndex - curStart.xIndex) * (tagNode.xIndex - curStart.xIndex);
        var difY = (tagNode.yIndex - curStart.yIndex) * (tagNode.yIndex - curStart.yIndex);
        var value = (int)(Mathf.Sqrt(difX + difY) * 10);
        return value <= 14;
    }

    private void initTestValue(ref List<Node> testList,Node _curStartNode)
    {
        foreach (var openInfo in testList)
        {
            if (openInfo != null)
            {
                var newCost = _curStartNode.distance + GetCost(_curStartNode, openInfo);
                if (newCost < openInfo.distance || !openList.Contains(openInfo))
                {
                    openInfo.distance = newCost;
                    openInfo.endDistance = GetCost(openInfo, endNode);
                    openInfo.all = openInfo.distance + openInfo.endDistance;
                    openInfo.father = _curStartNode;
                    
                    if (!openList.Contains(openInfo))
                    {
                        openList.Add(openInfo);
                        openInfo.GetComponent<Renderer>().material.color = Color.grey;
                    }
                }
                if (openInfo.father != null){
                    openInfo.ceng = openInfo.father.transform.name;
                }
                openInfo.setDisplay();
            }
        }
    }

    int GetCost(Node a, Node b)
    {
        //等到两点之间的一个距离（x方向和y方向）
        int coutX = Mathf.Abs(a.xIndex - b.xIndex);
        int coutY = Mathf.Abs(a.yIndex - b.yIndex);

        //return (coutX + coutY) * 10;
        if (coutX > coutY)
        {
            return (coutX - coutY) * 10 + coutY * 14;
        }
        else
        {
            return (coutY - coutX) * 10 + coutX * 14;
        }
    }


    void calCurrentGoGrid(Node _startNode)
    {
		List<Node> test = new List<Node>();
		var x = _startNode.xIndex - 1;
		var y = _startNode.yIndex - 1;
		var xWidth = _startNode.xIndex + 1;
		var yWidth = _startNode.yIndex + 1;
		while (x <= xWidth)
		{
			y = _startNode.yIndex - 1;
			while (y <= yWidth)
			{
				if (x >= 0 && y >= 0 && x < width && y < height && allGrid[x, y] != null && _startNode != allGrid[x, y])
				{
					
					if (!wallObjList.Contains(allGrid[x, y]) && !closeList.Contains(allGrid[x, y]))
					{
                        test.Add(allGrid[x, y]);
					}
				}
				y++;
			}
			x++;
		}
		initTestValue(ref test,_startNode);
    }



    public static object GetPropertyValue<T>(T obj, string name)
    {
        if (obj != null)
        {
            Type type = obj.GetType();
            FieldInfo a = type.GetField(name);
            object info = a.GetValue(obj);
            return info;
        }
        return null;
    }

    void sortOn<T>(List<T> array,string [] tags) 
    {
        var _left = 0;
        var _right = array.Count - 1;
        quickSort(array, _left,_right,tags);

    }

    private void quickSort<T>(List<T> array, int left, int right, string [] tags)
    {
        if (left < right)
        {
            T temp = array[left];
            int i = left;
            int j = right;
            while (i != j)
            {
                while (i < j && HandleTags(array[j], temp, tags))
                {
                    j--;
                }
                while (i < j && HandleTags(temp, array[i], tags))
                {
                    i++;
                }
                if (i < j)
                {
                    T replace = array[i];
                    array[i] = array[j];
                    array[j] = replace;
                }
            }
            array[left] = array[i];
            array[i] = temp;
            quickSort(array, left, i - 1, tags);
            quickSort(array, i + 1, right, tags);
        }
    }

    private bool HandleTags<T>(T obj,T objTag,string [] tags,int index = 0)
    {
   
        if (index >= tags.Length)
        {
            return true;
        }
     
        var comparingProperty = tags[index];
        var type = tags[index + 1];

        if(type == "up")
        {
            if ((int)GetPropertyValue(obj, comparingProperty) > (int)GetPropertyValue(objTag, comparingProperty))
            {
                return true;
            }
            else if((int)GetPropertyValue(obj, comparingProperty) == (int)GetPropertyValue(objTag, comparingProperty))
            {
                index += 2;
                return HandleTags(obj, objTag,tags,index);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if ((int)GetPropertyValue(obj, comparingProperty) < (int)GetPropertyValue(objTag, comparingProperty))
            {
                return true;
            }
            else if ((int)GetPropertyValue(obj, comparingProperty) == (int)GetPropertyValue(objTag, comparingProperty))
            {
                index += 2;
                return HandleTags(obj, objTag, tags, index);
            }
            else
            {
                return false;
            }
        }
            

    }
}
