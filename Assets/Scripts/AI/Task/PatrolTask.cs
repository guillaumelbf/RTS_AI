using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class PatrolTask : BT.Node
{
    private AIController aiController = null;

    public PatrolTask(AIController _aiController)
    {
        aiController = _aiController;
    }

    public override BT.NodeState Evaluate()
    {
        Debug.Log("Unit free : "+ aiController.GetAllUnitsAvailable().Count);

        foreach (var unit in aiController.GetAllUnitsAvailable())
        {
            if(!unit.IsStopped())
                continue;

            //Random squad def pos around the main factory
            Vector3 position = aiController.GetAllFactorys()[0].transform.position + 
                               new Vector3(Random.Range(7,20)*(Random.Range(0,2)*2-1),
                                         2,
                                         Random.Range(7,20)*(Random.Range(0,2)*2-1));
            
            unit.SetTargetPos(position);
        }

        return BT.NodeState.SUCCESS;
    }
}
