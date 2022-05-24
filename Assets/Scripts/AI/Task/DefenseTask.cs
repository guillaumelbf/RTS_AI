using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;

public class DefenseTask : BT.Node
{
    struct DefenseData
    {
        public Vector3 defPosition;
        public List <Unit> playerUnits;
        public Unit leadPlayerUnit;
        public float score;
        public UnitSquad unitSquad;
        public int randUnitSup;
    }

    private AIController aiController;
    private UnitController playerController;

    [SerializeField] 
    private float distanceAttack = 50.0f;

    static private List<DefenseData> listDefData;

    private InfluenceMap scriptInfluenceMap;
    private ETeam playerTeam;


    public DefenseTask(AIController _aiController,InfluenceMap mapInflu)
    {
        scriptInfluenceMap = mapInflu;
        aiController = _aiController;
        listDefData = new List<DefenseData>();

        playerTeam = aiController.GetTeam() == ETeam.Blue ? ETeam.Red : ETeam.Blue;
        playerController = GameServices.GetControllerByTeam(playerTeam);
    }

    public override BT.NodeState Evaluate()
    {
        if (aiController.GetAllUnits().Count == 0)
            return BT.NodeState.SUCCESS;

        foreach (Factory factory in aiController.GetAllFactorys())
            foreach (Unit playerUnit in playerController.GetAllUnits())
                if ((playerUnit.transform.position - factory.transform.position).magnitude < distanceAttack)
                    CheckUnitAndCreateDef(playerUnit);

        //Unit receiveUnitAttack = (Unit)GetData("playerUnit");

        ManageDefense();

        foreach (Unit aiUnit in aiController.GetAllUnits())
        {
            foreach (DefenseData enemyAttackData in listDefData)
            {
                foreach (Unit playerUnit in enemyAttackData.playerUnits)
                {
                    if (playerUnit == null)
                        continue;

                    if ((playerUnit.transform.position - aiUnit.transform.position).magnitude <= aiUnit.GetUnitData.AttackDistanceMax)
                        aiUnit.SetAttackTarget(playerUnit);
                }
            }
        }

        return BT.NodeState.SUCCESS;
    }

    void ManageDefense()
    {
        for(int i = 0; i < listDefData.Count; i++)
        {
            addUnitInSquadIfPossible(listDefData[i]);

            if (listDefData[i].unitSquad.members.Count <= 0)
                continue;

            listDefData[i].unitSquad.MoveSquad(listDefData[i].defPosition);

            DefenseData tmpDD = listDefData[i];

            foreach(Unit aiUnit in listDefData[i].unitSquad.members)
            {
                if (listDefData[i].leadPlayerUnit == null)
                    continue;

                if (((listDefData[i].leadPlayerUnit.transform.position - tmpDD.unitSquad.savePos)).magnitude <  aiUnit.GetUnitData.AttackDistanceMax)
                {
                    tmpDD.defPosition = listDefData[i].leadPlayerUnit.transform.position;
                    listDefData[i] = tmpDD;
                }
            }            
        }
    }

    void addUnitInSquadIfPossible(DefenseData enemyAttackData)
    {
        foreach (Unit unit in aiController.GetAllUnits())
        {
            if (enemyAttackData.unitSquad.members.Count < enemyAttackData.randUnitSup)
            {
                if (!unit.isInSquad)
                    enemyAttackData.unitSquad.AddUnit(unit);
            }
            else
                break;
        }
    }

    void CheckUnitAndCreateDef(Unit playerUnit)
    {
        if (CheckIfUnitIsInListDef(playerUnit))
            return;

        DefenseData defPos = new DefenseData();

        float rad = 15.0f;
        defPos.leadPlayerUnit = playerUnit;
        defPos.playerUnits = GetAllUnitArround(playerUnit, rad);
        defPos.defPosition = playerUnit.transform.position;

        //need score to send the same or more unit
        defPos.score = scriptInfluenceMap.AmountScoreArroundPos(playerUnit.transform.position, rad, playerTeam);
        defPos.randUnitSup = Random.Range(0, 2) + defPos.playerUnits.Count;
        defPos.unitSquad = new UnitSquad();

        listDefData.Add(defPos);
    }

    List<Unit> GetAllUnitArround(Unit currentUnit, float radius)
    {
        List<Unit> units = new List<Unit>();
        //units.Add(currentUnit);

        foreach (Unit unit in playerController.GetAllUnits())
        {
            if ((currentUnit.transform.position - unit.transform.position).magnitude < radius)
                units.Add(unit);
        }

        return units;
    }

    bool CheckIfUnitIsInListDef(Unit unit)
    {
        for (int i = 0; i < listDefData.Count; i++)
        {
            foreach (Unit unitTest in listDefData[i].playerUnits)
                if (unit == unitTest)
                    return true;
        }

        return false;
    }

    // Call in unit dead event, work on it to optimize and find a good solution 
    public static void RemovePlayerUnitFromAllList(Unit unit)
    {
        for(int i = 0; i < listDefData.Count; i++)
        {
            //if ia dead
            listDefData[i].unitSquad.members.Remove(unit);

            //if player dead
            listDefData[i].playerUnits.Remove(unit);

            DefenseData tmpData = listDefData[i];

            if (tmpData.leadPlayerUnit == unit && tmpData.playerUnits.Count != 0)
                tmpData.leadPlayerUnit = tmpData.playerUnits[0];

            if (tmpData.leadPlayerUnit == unit && tmpData.playerUnits.Count == 0)
                tmpData.leadPlayerUnit = null;

            listDefData[i] = tmpData;

            // Sucess Defense
            if(listDefData[i].playerUnits.Count == 0)
            {
                listDefData[i].unitSquad.ClearUnit();
                listDefData.Remove(listDefData[i]);
            }    
        }
    }
}

