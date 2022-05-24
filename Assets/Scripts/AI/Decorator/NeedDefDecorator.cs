using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT = BehaviourTree;

public class NeedDefDecorator : BT.Node
{
    private AIController aiController;

    public NeedDefDecorator(AIController _aiController)
    {
        aiController = _aiController;
    }

    public override BT.NodeState Evaluate()
    {
        return BT.NodeState.FAILED;
    }
}
