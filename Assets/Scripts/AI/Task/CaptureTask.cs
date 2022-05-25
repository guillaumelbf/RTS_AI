using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using BT = BehaviourTree;

public class CaptureTask : BT.Node
{
    private AIController aiController;
    private ETeam playerTeam;

    private List<UnitSquad> captureSquadList = new List<UnitSquad>();
    public CaptureTask(AIController _aiController)
    {
        aiController = _aiController;
        playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;
        captureSquadList.Add(new UnitSquad());
        captureSquadList.Add(new UnitSquad());
    }

    public override BT.NodeState Evaluate()
    {
        TargetBuilding nearestBuilding = GetNearestBuilding();

        if (nearestBuilding != null)
        {
            //Create Squad for capture
            foreach (var captureSquad in captureSquadList)
            {
                if (captureSquad.members.Count < 5)
                {
                    foreach (var unit in aiController.GetAllUnitsAvailable())
                    {
                        captureSquad.members.Add(unit);
                        if(captureSquad.members.Count == 5)
                            break;
                    }
                }
            }
            
            //Set squad to capture point
            foreach (var captureSquad in captureSquadList)
            {
                foreach (var unit in captureSquad.members)
                {
                    unit.SetCaptureTarget(nearestBuilding);
                }
            }
        }
        

        
        return BT.NodeState.SUCCESS;
    }

    TargetBuilding GetNearestBuilding()
    {
        List<TargetBuilding> availableTargetBuildings = new List<TargetBuilding>();
        Vector3 mainFactoryPos = aiController.GetAllFactorys()[0].transform.position;

        foreach (var targetBuilding in GameServices.GetTargetBuildings())
        {
            if ((targetBuilding.GetTeam() == ETeam.Neutral || targetBuilding.GetTeam() == playerTeam) &&
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
