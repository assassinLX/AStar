using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;


public class FindNode : MonoBehaviour
{
    private int width;
    private int height;
    public Node startNode;
    private Node curStartNode;
    public Node endNode;
    public Color startColor = Color.red;
    public Color endColor = Color.yellow;
    public Color wellColor = Color.black;

    public Node[,] allGrid;
    public List<Node> wallObjList;
    public List<Node> openList;
    public List<Node> closeList;


    private void createList()
	{
        openList = new List<Node>();
        closeList = new List<Node>();
	}
    
    private void Awake()
    {
        createList();
    }

    public void clear()
    {
        openList.RemoveRange(0,openList.Count);
        closeList.RemoveRange(0, closeList.Count);
    }

    public void settingData()
    {
        var mapManager = MapManager.GetInstance();
        allGrid = mapManager.allGrid;
        wallObjList = mapManager.getWallList();
        width = mapManager.width;
        height = mapManager.height;
        startNode = mapManager.startNode;
        curStartNode = startNode;
        endNode = mapManager.endNode;
    }


    public void handleFindNode()
    {
        do
        {
            calCurrentGoGrid(curStartNode);
            string[] args = new string[] { "all", "up", "endDistance", "up" };
            sortOn(openList, args);
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

            if (firstStep == null)
            {
                return;
            }

            if (firstStep != null)
            {
                curStartNode = firstStep;
                closeList.Add(curStartNode);
                curStartNode.GetComponent<Renderer>().material.color = startColor;
                openList.Remove(firstStep);
            }

        } while (openList.Count > 0);
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
                if (openInfo.distance > newCost || !openList.Contains(openInfo))
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
