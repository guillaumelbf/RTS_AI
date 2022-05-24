using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class CaptureTask : BT.Node
{
    private AIController aiController;
    public CaptureTask(AIController _aiController)
    {
        aiController = _aiController;
    }

    public override BT.NodeState Evaluate()
    {
        ETeam playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;

        //InfluenceMap.GetInfluenceMap();


        return BT.NodeState.SUCCESS;
    }
}
