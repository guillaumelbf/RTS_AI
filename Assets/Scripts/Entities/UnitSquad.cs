using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSquad
{
    [HideInInspector] public List<Unit> members = new List<Unit>();
    private Formation SquadFormation;
    private float MoveSpeed = 100.0f;

    public UnitSquad()
    {
        SquadFormation = new Formation(this);
    }

    private void Awake()
    {
    }

    public void MoveSquad(Vector3 targetPos)
    {
        SquadFormation.CreateFormation(targetPos);
    }

    public void AddUnit(Unit unit)
    {
        members.Add(unit);
    }

    public void ClearUnit()
    {
        members.Clear();
    }

    public void RemoveUnit(Unit unit)
    {
        if (!members.Remove(unit)) 
            return;
        SquadFormation.UpdateFormationLeader();
        //temp when unit is removed from squad recalculate formation based on the new leader grid position
        MoveSquad(members[0].GridPosition);
    }
    
    public void MoveUnitToPosition()
    {
        SetSquadSpeed();
        foreach (Unit unit in members)
        {
            unit.CurrentMoveSpeed = MoveSpeed;
            unit.SetTargetPos(unit.GridPosition);
        }
    }

    /*
     * The move speed of the squad is the lowest within the squad members
     */
    void SetSquadSpeed()
    {
        foreach (Unit unit in members)
        {
            MoveSpeed = Mathf.Min(MoveSpeed, unit.GetUnitData.Speed);
        }
    }
    
    public void SwitchFormation(E_FORMATION_TYPE newFormationType)
    {
        SquadFormation.SetFormationType = newFormationType;
    }
}
