using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT = BehaviourTree;

public class NeedDefDecorator : BT.Node
{
    private AIController aiController;
    private UnitController playerController;

    private ETeam playerTeam;
    private float distanceAttack = 50.0f;


    public NeedDefDecorator(AIController _aiController)
    {
        aiController = _aiController;

        playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;
        playerController = GameServices.GetControllerByTeam(playerTeam);
    }

    public override BT.NodeState Evaluate()
    {

        foreach (Factory factory in aiController.GetAllFactorys())
        {
            foreach (Unit playerUnit in playerController.GetAllUnits())
            {
                if ((playerUnit.transform.position - factory.transform.position).magnitude < distanceAttack)
                {
                    //SetData("playerUnit", playerUnit);

                    return BT.NodeState.SUCCESS;

                }
            }
        }
        
        return BT.NodeState.FAILED;
    }
}
