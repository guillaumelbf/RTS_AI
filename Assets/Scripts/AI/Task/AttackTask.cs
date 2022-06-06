using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class AttackTask : BT.Node
{
    private UnitSquad attackSquad;
    private AIController aiController;
    private UnitController playerController;

    private ETeam playerTeam;

    public AttackTask(AIController _aiController)
    {
        aiController = _aiController;
        attackSquad = new UnitSquad();

        playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;
        playerController = GameServices.GetControllerByTeam(playerTeam);

    }

    public override BT.NodeState Evaluate()
    {

        List<Factory> factorysPLayer = playerController.GetAllFactorys();

        if (aiController.CapturedTargets < 1 && aiController.GetAllUnitsAvailable().Count < 5)
            return BT.NodeState.FAILED;

        if (attackSquad.members.Count >= 0)
            addUnitInSquadIfPossible();

        // attack enemy in road 
        AttackEnemyArround();

        // go to enemy factory
        Vector3 posTargetFactory = factorysPLayer[0].transform.position ;

        if(attackSquad.members.Count <= 0)
            return BT.NodeState.FAILED;

        if ((attackSquad.members[0].transform.position - posTargetFactory).magnitude > attackSquad.members[0].GetUnitData.AttackDistanceMax)
            attackSquad.MoveSquad(posTargetFactory);
        else
        {
            foreach (Unit unit in attackSquad.members)
                unit.SetAttackTarget(factorysPLayer[0]);
        }

        // Check members squad, isAllDead ?
        return BT.NodeState.SUCCESS;
    }

    void AttackEnemyArround()
    {
        foreach (Unit unit in aiController.GetAllUnits())
        {
            foreach (Unit playerEnemy in playerController.GetAllUnits())
            {
                if ((playerEnemy.transform.position - unit.transform.position).magnitude <= unit.GetUnitData.AttackDistanceMax)
                    unit.SetAttackTarget(playerEnemy);
            }
        }
    }


    void addUnitInSquadIfPossible()
    {
        float halfUnitAI = aiController.GetAllUnits().Count / 2.0f;

        foreach (Unit unit in aiController.GetAllUnits())
        {
            if (attackSquad.members.Count < halfUnitAI + Random.Range(0,halfUnitAI))
            {
                if (!unit.isInSquad)
                    attackSquad.AddUnit(unit);
            }
        }
    }
}
