using UnityEngine;
using System.Collections;

public class Node
{
	/*逻辑中用的*/
    public int gCost;//
	public int hCost; //和终点的距离
	public int fCost
	{
		get { return gCost + hCost; }
	}
	public Node parent;

	/*在Unity当中用的*/
	public bool canWalk;
	//网格的下标
	public int gridX;
	public int gridY;
	//节点的位置
	public Vector3 worldPos;

	public Node(bool _canWalk, Vector3 position, int x, int y)
	{
		canWalk = _canWalk;
		worldPos = position;
		gridX = x;
		gridY = y;
	}
}