using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindPath_AStar : MonoBehaviour
{
	public Transform startPoint;
	public Transform endPoint;

    private bool isFind = true;
	private Grid grid;
	// Use this for initialization
	void Start()
	{
		grid = GetComponent<Grid>();

		
    }

    void Update()
    {
        if(isFind){
			FindPath(startPoint.position, endPoint.position);
            isFind = false;
        }
	}

	//
	void FindPath(Vector3 startPos, Vector3 endPos)
	{
		//开启列表
		List<Node> opentSet = new List<Node>();
		//关闭列表
		List<Node> closeSet = new List<Node>();

		//起点格子
		Node startNode = grid.FindWithPosition(startPos);
		//终点格子
		Node endNode = grid.FindWithPosition(endPos);

		//把起点加入开启列表
		opentSet.Add(startNode);

		//开始循环（开启列表有值）
		while (opentSet.Count > 0)
		{
			//当前点
			Node currentNode = opentSet[0];
			//开启列表中最小的f_Cost
			for (int i = 0; i < opentSet.Count; i++)
			{
				//如果总花费最小，并且离目标点最近
                if (opentSet[i].fCost <= currentNode.fCost && opentSet[i].hCost < currentNode.hCost)
				{
					currentNode = opentSet[i];
				}
			}

			//把这个点 点红
			//把当前点从开启列表删除
			opentSet.Remove(currentNode);
			//把当前点添加到关闭列表
			closeSet.Add(currentNode);

            Debug.Log(string.Format("{0},{1}|||| all : = {2}", currentNode.gridX, currentNode.gridY, currentNode.fCost));
			//If当前点是终点，跳出循环
			if (currentNode == endNode)
			{
				GetPath(startNode, endNode);
				return;
			}

			//周围的点
			List<Node> around = grid.GetAroundNode(currentNode);
			//循环周围的点
			foreach (Node node in around)
			{
				//这个点不能走或者在关闭列表中，跳过这个点
				if (!node.canWalk || closeSet.Contains(node))
				{
					continue;
				}

				//点开一个红点，周围的点会得到一个新的花费g
				//int newCost_g = currentNode.gCost + GetCost(currentNode, node);
                int newCost_g = GetCost(startNode, node);


                //Debug.Log(string.Format("当前的 ===》》》 {0},{1} ------ new cost : {2}",node.gridX,node.gridY,newCost_g));
                //Debug.Log(string.Format("===》》》 {0},{1} ------ node.gCost : {2}", node.gridX, node.gridY,node.gCost));


				//比较新花费和原来的花费，谁更小(谁离我们起点近) || 这个点不再开启列表中
				if (!opentSet.Contains(node))//node.gCost > newCost_g ||
				{
					//替换成新的花费
					node.gCost = newCost_g;
					node.hCost = GetCost(node, endNode);
					//将这个点（周围的点里面的）的父节点设置为当前点（新点开的红点）
					node.parent = currentNode;

					//这个点不再开启列表中
					//if (!opentSet.Contains(node))
					//{
                    //}
					opentSet.Add(node);
                }
			}
		}
	}

	//计算花费
	int GetCost(Node a, Node b)
	{
		//等到两点之间的一个距离（x方向和y方向）
		int coutX = Mathf.Abs(a.gridX - b.gridX);
		int coutY = Mathf.Abs(a.gridY - b.gridY);

        return (coutX + coutY) * 10;
		//if (coutX > coutY)
		//{
		//	return (coutX - coutY) * 10 + coutY * 14;
		//}
		//else
		//{
		//	return (coutY - coutX) * 10 + coutX * 14;
		//}
	}


	//得到路径
	void GetPath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node temp = endNode;
		while (temp != startNode)
		{
			path.Add(temp);
			temp = temp.parent;
		}
		//列表转置
		path.Reverse();
		grid.path = path;
	}
}