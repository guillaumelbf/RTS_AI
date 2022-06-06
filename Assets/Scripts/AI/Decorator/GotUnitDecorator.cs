using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class GotUnitDecorator : BT.Node
{
    private AIController aiController;
    
    public GotUnitDecorator(AIController _aiController)
    {
        aiController = _aiController;
    }

    public override BT.NodeState Evaluate()
    {
        if (aiController.TotalBuildPoints <= 0)
            return BT.NodeState.SUCCESS;

        return BT.NodeState.FAILED;
    }
}
