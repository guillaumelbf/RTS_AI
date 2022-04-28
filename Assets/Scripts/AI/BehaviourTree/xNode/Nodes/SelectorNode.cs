﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SelectorNode : Node 
{
	
	[Input] public Node prevNode;
	[Output] public Node nextNode;

	public BehaviourTree.Selector selector;

	public int index = 0;
	public ContainerSelector myContainer;

	// Use this for initialization
	protected override void Init() {
		base.Init();
		selector = myContainer.GetSelectorByIndex(index);
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}
