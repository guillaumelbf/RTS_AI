using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;
using XNode;
using Node = XNode.Node;

[CreateAssetMenu]
public class BehaviourTreeGraph : NodeGraph
{
    public override Node AddNode(Type type)
    {
        if (type.Name == nameof(RootNode))
        {
            if (GetRootNode() != null)
                return null;

            return base.AddNode(type);
        } 
            
        return base.AddNode(type);
    }

    public Node GetRootNode()
    {
        foreach (var node in nodes)
        {
            if (node.GetType().FullName == nameof(RootNode))
                return node;
        }

        return null;
    }
}