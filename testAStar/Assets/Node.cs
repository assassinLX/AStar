﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Node
{
    public GameObject obj;

    public int all = 0;
    public int distance = 0;
    public int endDistance = 0;
    public string ceng;


    public int xIndex;
    public int yIndex;
    public Node father;

    public Node(GameObject gameObj)
    {
        obj = gameObj;
    }
    
    public Transform transform
    {
        get
        {
            return obj.transform;
        }
    }

    public T GetComponent<T>()
    {
        return obj.transform.GetComponent<T>();
    }

    public void setDisplay()
    {   
        Transform objPrefabTrans = GetChildByName(obj.transform, "Canvas(Clone)");
        if (objPrefabTrans == null)
        {
            GameObject curObj = (GameObject)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Canvas.prefab", typeof(GameObject)));
            objPrefabTrans = curObj.transform;
            objPrefabTrans.SetParent(obj.transform);
            objPrefabTrans.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,0,0);
        }
        var text = GetChildComponent(objPrefabTrans, "Text", "Text") as Text;
        var text1 = GetChildComponent(objPrefabTrans, "Text (1)", "Text") as Text;
		var text2 = GetChildComponent(objPrefabTrans, "Text (2)", "Text") as Text;
		var text3 = GetChildComponent(objPrefabTrans, "Text (3)", "Text") as Text;
		var text4 = GetChildComponent(objPrefabTrans, "Text (4)", "Text") as Text;

        text.text = all.ToString();
        text1.text = distance.ToString();
        text2.text = endDistance.ToString();
        text3.text = ceng;
        text4.text = transform.name;
    }

    private Component GetChildComponent(Transform tar, string childName, string comName)
    {
        Transform t = GetChildByName(tar, childName);
        if (t != null)
        {
            Component comp = t.gameObject.GetComponent(comName);
            return comp;
        }
        return null;
    }

    private Transform GetChildByName(Transform transform,string name)
    {
        Transform[] tarList = transform.GetComponentsInChildren<Transform>();
        foreach (var co in tarList)
        {
            if (co.transform.name == name)
            {
                return co;
            }
        }
        return null;
    }
}
