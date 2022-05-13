using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class DefenseTask : BT.Node
{
    struct DefensePos
    {
        public Vector3 position;
        public List <Unit> units;
        public int score; 
    }

    private AIController aiController;
    private UnitController playerController;

    [SerializeField] 
    private float distanceAttack = 50.0f;

    private List<DefensePos> listDefPos;
    
    public DefenseTask(AIController _aiController)
    {
        aiController = _aiController;
        listDefPos = new List<DefensePos>();
    }

    public override BT.NodeState Evaluate()
    {
        ETeam playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;
        playerController = GameServices.GetControllerByTeam(playerTeam);
        InfluenceMap.GetInfluenceMap();

        if (aiController.GetAllUnits().Count == 0)
            return BT.NodeState.SUCCESS;

        if (isUnderAttack())
        {
            // Code Movement of ai unit 
            foreach (Unit unit in aiController.GetAllUnits())
                unit.SetTargetPos(listDefPos[0].position);

            return BT.NodeState.FAILED;
        }

        return BT.NodeState.SUCCESS;
    }

    bool isUnderAttack()
    {
        //if(listDefPos.Count != 0)
        //    listDefPos.Clear();

        foreach (Factory factory in aiController.GetAllFactorys())
        {
            foreach (Unit unit in playerController.GetAllUnits())
            {
                if ((unit.transform.position - factory.transform.position).magnitude < distanceAttack)
                {
                    Debug.Log("on nous attaque chef");

                    if (CheckIfUnitIsInListDef(unit))
                        continue;

                    DefensePos defPos = new DefensePos();
                    defPos.units = GetAllUnitArround(unit,20);
                    // not sure of that, to find midlle of two point ...
                    defPos.position = (unit.transform.position + factory.transform.position) / 2.0f;
                    listDefPos.Add(defPos);
                }
            }
        }

        if (listDefPos.Count != 0)
            return true;
        
        return false;
    }

    List<Unit> GetAllUnitArround(Unit currentUnit, float radius)
    {
        List<Unit> units = new List<Unit>();
        units.Add(currentUnit);

        foreach (Unit unit in playerController.GetAllUnits())
        {
            if ((currentUnit.transform.position - unit.transform.position).magnitude < radius)
                units.Add(unit);
        }

        return units;
    }

    bool CheckIfUnitIsInListDef(Unit unit)
    {
        for (int i = 0; i < listDefPos.Count; i++)
        {
            foreach (Unit unitTest in listDefPos[i].units)
                if (unit == unitTest)
                    return true;
        }

        return false;
    }
}

