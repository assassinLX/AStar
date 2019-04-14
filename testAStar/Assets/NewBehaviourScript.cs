using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private const int width = 20;
    private const int height = 20;
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

    public Hashtable openList;
    public Stack<Node> closeList;
	Queue<Node> nodes = new Queue<Node>();
    private Node targetNode = null;
    private Node branchNode = null;

	private void createList()
	{
		allGrid = new Node[width, height];
		wallObjList = new List<Node>();
		closeList = new Stack<Node>();
		openList = new Hashtable();
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
				//allGrid[i, t].transform.name =""+ i + "," + t;
				allGrid[i, t].transform.name = string.Format("{0},{1}", i, t);

			}
		}
	}

	private void initCloseListAndOpenList()
	{
		//添加不能走的格子
		for (int i = 0; i < width * height; i++)
		{
			var grid = allGrid[(int)(i / width), (int)(i % height)];
			if (wallObjList.Contains(grid))
			{
				//closeList.Push(grid);
			}
			else
			{
				if (!object.ReferenceEquals(grid, startNode))
				{
					string str = String.Format("{0},{1}", (int)(i / width), (int)(i % height));
					openList.Add(str, grid);
				}

			}
		}
	}


	private void initWall()
	{
		wallObjList.Add(allGrid[1, 1]);
		for (int i = 3; i <= 11; i++)
		{
			wallObjList.Add(allGrid[4, i]);
		}
		for (int i = 4; i < 9; i++)
		{
			wallObjList.Add(allGrid[6, i]);
		}

		for (int i = 5; i < 13; i++)
		{
			wallObjList.Add(allGrid[i, 11]);
		}
		for (int i = 4; i < 14; i++)
		{
			wallObjList.Add(allGrid[i, 2]);
		}

		for (int i = 6; i < 11; i++)
		{
			wallObjList.Add(allGrid[i, 9]);
		}

		for (int i = 4; i < 11; i++)
		{
			wallObjList.Add(allGrid[12, i]);
		}

		for (int i = 7; i < 12; i++)
		{
			wallObjList.Add(allGrid[i, 4]);
		}

		for (int i = 0; i < wallObjList.Count; i++)
		{
			wallObjList[i].GetComponent<Renderer>().material.color = wellColor;
		}

	}


    private void Awake()
    {
        createList();
		initAllGrid();

        startNode = allGrid[17,16];
        endNode = allGrid[7, 8];
        curStartNode = startNode;
        startNode.GetComponent<Renderer>().material.color = startColor;
        endNode.GetComponent<Renderer>().material.color = endColor;
        initWall();
        initCloseListAndOpenList();
    }








    void Start () {
        handle();
		//foreach(var node in nodes){
        //    node.GetComponent<Renderer>().material.color = Color.green;
        //}
    }



    private void handle()
    {
        Queue<Node> curList = calCurrentGoGrid(curStartNode);
        string[] args = new string[] { "all", "up", "endDistance", "up" };
        curList = sortOn(curList, args);

		if (openList.Count == 0)
        {
            return;
        }
        if (isCanGoNode(endNode, curStartNode))
        {
            return;
        }
		Node firstStep = null;
		if (curList.Count > 0)
		{
			firstStep = curList.Dequeue();
		}


        if (branchNode != null && openList.Contains(string.Format("{0},{1}",branchNode.xIndex,branchNode.yIndex)) && firstStep != null &&firstStep.all > branchNode.all){

   //         var array = nodes.ToArray();
   //         var list = new List<Node>(array);
   //         for (int i = list.Count - 1; i >= 0; i--)
			//{
				
   //             if (object.ReferenceEquals(list[i], targetNode))
   //             {
   //                 list[i].GetComponent<Renderer>().material.color = Color.blue;
			//		list.Remove(list[i]);
   //                 break;
   //             }
   //             list[i].GetComponent<Renderer>().material.color = Color.blue;
			//	list.Remove(list[i]);
			//}
            //nodes = new Queue<Node>(list.ToArray());

			curStartNode = targetNode;
			branchNode = null;
            targetNode = null;
        }        
        else
        {
			if (firstStep != null && isCanGoNode(firstStep, curStartNode))
			{
                if (curList.Count > 0){
                    if (firstStep.all == curList.Peek().all){
                        if (branchNode == null || (branchNode != null && branchNode.all > curList.Peek().all)){
                            targetNode = firstStep;
                            branchNode = curList.Peek();
							branchNode.GetComponent<Renderer>().material.color = Color.blue;                     
                        }
					}

                }
                curStartNode = firstStep;
				closeList.Push(curStartNode);
				var key = String.Format("{0},{1}", firstStep.xIndex, firstStep.yIndex);
                openList.Remove(key);

				curStartNode.GetComponent<Renderer>().material.color = startColor;
				nodes.Enqueue(curStartNode);
			}
			else
			{
                if(closeList.Count == 0){
                    Debug.Log("不是通路");
                    return;
                }
				if (nodes.Count > 0)
				{
					nodes.Dequeue();
				}
				var node = closeList.Pop();
				curStartNode.GetComponent<Renderer>().material.color = Color.cyan;
				curStartNode = node;
			}
        }
		//handle();
		Invoke("handle", 0.1f);
    }



    private bool isCanGoNode(Node tagNode,Node curStart)
    {
        var difX = (tagNode.xIndex - curStart.xIndex) * (tagNode.xIndex - curStart.xIndex);
        var difY = (tagNode.yIndex - curStart.yIndex) * (tagNode.yIndex - curStart.yIndex);
        var value = (int)(Mathf.Sqrt(difX + difY) * 10);
        return value <= 14;
    }

    private void initTestValue(ref Queue<Node> testQueue)
    {
        foreach (var openInfo in testQueue)
        {
            if (openInfo != null)
            {
				var xValueM = (startNode.xIndex - openInfo.xIndex) * (startNode.xIndex - openInfo.xIndex);
				var yValueM = (startNode.yIndex - openInfo.yIndex) * (startNode.yIndex - openInfo.yIndex);
				var difXY = Mathf.Sqrt(xValueM + yValueM);
				var argXValue = (endNode.xIndex - openInfo.xIndex) * (endNode.xIndex - openInfo.xIndex);
				var argYValue = (endNode.yIndex - openInfo.yIndex) * (endNode.yIndex - openInfo.yIndex);
				var difArgXY = Mathf.Sqrt(argXValue + argYValue);
				var arg = (int)difArgXY / difXY;
				openInfo.distance = (int)(Mathf.Sqrt(xValueM + yValueM) * arg * 10);

                openInfo.endDistance = (Mathf.Abs(endNode.xIndex - openInfo.xIndex) + Mathf.Abs(endNode.yIndex - openInfo.yIndex)) * 10;
                openInfo.all = openInfo.distance + openInfo.endDistance;
                openInfo.setDisplay();
            }
        }
    }

    Queue<Node> calCurrentGoGrid(Node _startNode)
    {
        Queue<Node> testQueue = new Queue<Node>();
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
                    string str = String.Format("{0},{1}", x, y);
                    if (openList.ContainsKey(str))
                    {
                        testQueue.Enqueue(allGrid[x, y]);
                    }
                }
                y++;
            }
            x++;
        }
        initTestValue(ref testQueue);
        return testQueue;
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

    Queue<T> sortOn<T>(Queue<T> array,string [] tags) 
    {
        var _left = 0;
        var _right = array.Count - 1;
        
        var currentArray = array.ToArray();
        quickSort(currentArray, _left,_right,tags);
        var _array = new Queue<T>(currentArray);
        return _array;
    }

    private void quickSort<T>(T[] array, int left, int right, string [] tags)
    {
        if (left < right)
        {
            T temp = array[left];
            int i = left;
            int j = right;
            while (i != j)
            {
                //while (i < j && (int)GetPropertyValue(array[j], tag) >= (int)GetPropertyValue(temp, tag))
                while (i < j && HandleTags(array[j], temp, tags))
                {
                    j--;
                }
                //while (i < j && (int)GetPropertyValue(array[i], tag) <= (int)GetPropertyValue(temp, tag))
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



//print("======================closeList===============================" + closeList.Count);
//foreach (var info in closeList)
//{
//    Debug.Log(info.transform.name);
//}
//print("======================openList===============================");
//foreach (var obj in openList.Keys)
//{
//    var info = openList[obj] as Node;
//    Debug.Log(info.transform.name);
//}

//print("======================openList===============================");
//foreach (var obj in openList.Keys)
//{
//    var info = openList[obj] as Node;
//    Debug.Log(info.transform.name + "distance: " + info.distance + "all:" + info.all);
//}

//print("======================closeList===============================");
//foreach (var info in closeList)
//{
//    Debug.Log(info.transform.name + "distance: " + info.distance + "all:" + info.all);
//}

//print("======================nodes===============================");
//foreach (var info in nodes)
//{
//    Debug.Log(info.transform.name + "distance: " + info.distance + "all:" + info.all);
//}

//print("======================curList===============================");
//foreach (var info in curList)
//{
//    Debug.Log(info.transform.name + "distance: " + info.distance + "all:" + info.all);
//}

