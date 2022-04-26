using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SequenceNode : Node
{

	[Input] public Node prevNode;
	[Output] public Node nextNode;
	
	public BehaviourTree.Sequence selector;
	
	// Use this for initialization
	protected override void Init() {
		base.Init();
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}