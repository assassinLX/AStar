using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
	//存放点节点的数组
	public Node[,] grid;

	//网格的大小
	public Vector2 gridSize;
	//节点的大小
	public float nodeRadius;
	public float nodeDiameter;
	//一个层，代表可不可以通过
	public LayerMask cantLayer;

	//x和y方向上各有多少个格子
	public int gridContX;
	public int gridContY;

	//起点
	public Transform start;

	//用来保存路径的列表
	public List<Node> path = new List<Node>();


	void Start()
	{
		cantLayer = LayerMask.GetMask("CantWalk");
		nodeDiameter = nodeRadius * 2;
		//gridContX = (int)(gridSize.x / nodeDiameter);
		gridContX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
		gridContY = Mathf.RoundToInt(gridSize.y / nodeDiameter);

		grid = new Node[gridContX, gridContY];
		CreatGrid();
	}

	void Update()
	{

	}
	//创建格子
	void CreatGrid()
	{
		//网格起点
		Vector3 startPoint = transform.position - gridSize.y / 2 * Vector3.forward - gridSize.x / 2 * Vector3.right;

		for (int i = 0; i < gridContX; i++)
		{
			for (int j = 0; j < gridContY; j++)
			{
				Vector3 worldPos = startPoint + Vector3.right * (nodeDiameter * i + nodeRadius) + Vector3.forward * (nodeDiameter * j + nodeRadius);
				//检测有没有碰到不能走的层上的物体
				bool canwalk = !Physics.CheckSphere(worldPos, nodeRadius, cantLayer);

				grid[i, j] = new Node(canwalk, worldPos, i, j);
			}
		}
	}

	//Unity中的辅助类
	void OnDrawGizmos()
	{
		if (grid == null)
		{
			return;
		}
		foreach (Node node in grid)
		{
			if (node.canWalk)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawCube(node.worldPos, (nodeDiameter - 0.02f) * new Vector3(1, 0.2f, 1));
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawCube(node.worldPos, (nodeDiameter - 0.02f) * new Vector3(1, 0.2f, 1));
			}
		}

		//画出起点的位置
		Node startNode = FindWithPosition(start.position);
		if (startNode.canWalk)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawCube(startNode.worldPos, (nodeDiameter - 0.02f) * new Vector3(1, 0.2f, 1));
		}


		//画路径
		if (path != null)
		{
			foreach (var node in path)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(node.worldPos, (nodeDiameter - 0.02f) * new Vector3(1, 0.2f, 1));
			}
		}
	}

	//通过位置得到在哪一个格子
	public Node FindWithPosition(Vector3 position)
	{
		//在x方向的占比
		float percentX = (position.x + gridSize.x / 2) / gridSize.x;
		float percentY = (position.z + gridSize.y / 2) / gridSize.y;

		//算出在哪个格子
		int x = Mathf.RoundToInt((gridContX - 1) * percentX);
		int y = Mathf.RoundToInt((gridContY - 1) * percentY);

		return grid[x, y];
	}

	//通过一个点寻找周围的点
	public List<Node> GetAroundNode(Node node)
	{
		List<Node> aroundNodes = new List<Node>();

		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				//传进来的点的下标  跳过
				if (i == 0 && j == 0)
				{
					continue;
				}

				int tempX = node.gridX + i;
				int tempY = node.gridY + j;

				//判断有没有越界
				if (tempX >= 0 && tempX < gridContX && tempY >= 0 && tempY < gridContY)
				{
					aroundNodes.Add(grid[tempX, tempY]);
				}
			}
		}

		return aroundNodes;
	}
}