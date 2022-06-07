using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;
public class ContainerTask : MonoBehaviour
{
    private Dictionary<string, BT.Node> taskList = new Dictionary<string, BT.Node>();

    public void AddTask(BT.Node _nodeTask)
    {
        taskList.Add(_nodeTask.GetType().Name,_nodeTask);
    }

    public BT.Node GetTask(string _name)
    {
        return taskList[_name];
    }
}
