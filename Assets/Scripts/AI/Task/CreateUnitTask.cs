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

        List<Factory> factorys =  aiController.GetAllFactorys();

        if(factorys.Count <= 0)
            return BT.NodeState.FAILED;

        factorys[0].RequestUnitBuild(Random.Range(0,3));

        return BT.NodeState.SUCCESS;
    }
}
