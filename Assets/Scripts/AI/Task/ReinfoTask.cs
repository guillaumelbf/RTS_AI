using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class ReinfoTask : BT.Node
{
    private AIController aiController;
    public ReinfoTask(AIController _aiController)
    {
        aiController = _aiController;
    }

    public override BT.NodeState Evaluate()
    {
        ETeam playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;

        InfluenceMap.GetInfluenceMap();

        
        ManageCreationOfUnits();

        return BT.NodeState.SUCCESS;
    }

    void ManageCreationOfUnits()
    {

        List<Factory> factorys = aiController.GetAllFactorys();

        factorys[0].RequestUnitBuild(0);
    }
}

