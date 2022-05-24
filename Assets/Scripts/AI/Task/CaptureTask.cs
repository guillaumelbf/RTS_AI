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

        TargetBuilding nearestBuilding = GetNearestBuilding();

        if (nearestBuilding != null && aiController.GetAllUnits().Count > 0)
        {
            aiController.GetAllUnits()[0].SetCaptureTarget(nearestBuilding);
        }
        
        return BT.NodeState.SUCCESS;
    }

    TargetBuilding GetNearestBuilding()
    {
        List<TargetBuilding> availableTargetBuildings = new List<TargetBuilding>();
        Vector3 mainFactoryPos = aiController.GetAllFactorys()[0].transform.position;

        foreach (var targetBuilding in GameServices.GetTargetBuildings())
        {
            if (targetBuilding.GetTeam() == ETeam.Neutral &&
                Vector3.Distance(mainFactoryPos,targetBuilding.transform.position) <= aiController.maxCaptureDistance)
            {
                availableTargetBuildings.Add(targetBuilding);
            }
        }

        if (availableTargetBuildings.Count == 0)
            return null;
        
        TargetBuilding nearest = availableTargetBuildings[0];


        foreach (var targetBuilding in availableTargetBuildings)
        {
            if (Vector3.Distance(mainFactoryPos, nearest.transform.position) >=
                Vector3.Distance(mainFactoryPos, targetBuilding.transform.position))
                nearest = targetBuilding;
        }

        return nearest;
    }
}
