using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT = BehaviourTree;

public class HasPointToCaptureDecorator : BT.Node
{
    private AIController aiController;
    private ETeam playerTeam;

    public HasPointToCaptureDecorator(AIController _aiController)
    {
        aiController = _aiController;
        playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;
    }

    public override BT.NodeState Evaluate()
    {
        if (IsBuildingAvailable() && aiController.GetAllUnitsAvailable().Count >= 5)
            return BT.NodeState.SUCCESS;
        
        return BT.NodeState.FAILED;
    }

    private bool IsBuildingAvailable()
    {
        foreach (var targetBuilding in GameServices.GetTargetBuildings())
        {
            if ((targetBuilding.GetTeam() == ETeam.Neutral || targetBuilding.GetTeam() == playerTeam) &&
                Vector3.Distance(aiController.GetAllFactorys()[0].transform.position,targetBuilding.transform.position) <= aiController.maxCaptureDistance)
            {
                return true;
            }
        }

        return false;
    }
}
