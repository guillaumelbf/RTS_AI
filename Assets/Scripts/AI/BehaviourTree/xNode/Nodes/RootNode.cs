using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class RootNode : Node
{

	[Output(connectionType = ConnectionType.Override)] public Node nextNode;
	
	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}