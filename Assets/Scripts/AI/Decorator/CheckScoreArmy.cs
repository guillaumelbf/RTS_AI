using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class CheckScoreArmy : BT.Node
{
    private AIController aiController;
    private InfluenceMap influenceMap;

    private ETeam playerTeam;

    private float valuePrctAdd = 30.0f;

    public CheckScoreArmy(AIController _aiController, InfluenceMap mapInflu)
    {
        aiController = _aiController;
        influenceMap = mapInflu;

        playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;

    }

    public override BT.NodeState Evaluate()
    {
        float scorePlayerArmy = influenceMap.GetScoreArmy(playerTeam);
        float scoreIAArmy = influenceMap.GetScoreArmy(aiController.GetTeam());

        float bonus = scoreIAArmy / valuePrctAdd;

        if (scoreIAArmy > scorePlayerArmy + bonus)
        {
            return BT.NodeState.SUCCESS;
        }

        return BT.NodeState.FAILED;
    }
}
