using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class BehaviourTreeGraph : NodeGraph
{
	private BehaviourTree.Tree behaviourTree = new BehaviourTree.Tree();

	public BehaviourTree.Node GetRootNode()
	{
		return behaviourTree.root;
	}
}