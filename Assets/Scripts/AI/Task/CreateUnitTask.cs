using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class CreateUnitTask : BT.Node
{
    private AIController aiController;
    public CreateUnitTask(AIController _aiController)
    {
        aiController = _aiController;
    }
    
    public override BT.NodeState Evaluate()
    {
        
        
        return BT.NodeState.SUCCESS;
    }
}
