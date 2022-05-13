using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class MoveUnitTask : BT.Node
{

    public MoveUnitTask()
    {
    }
    
    public override BT.NodeState Evaluate()
    {
        Debug.Log("MoveUnit");
        
        return BT.NodeState.RUNNING;
    }
}
