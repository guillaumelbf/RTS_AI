using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_FORMATION_TYPE
{
    Circle,
    Square,
    Arrow,
    None,
    Custom
}

/*
 * Class that calculate the position of squad members base of the type of formation selected
 */
public class Formation
{
    private E_FORMATION_TYPE FormationType;
    private float Radius = 5.0f;

    private UnitSquad Squad;

    private float GridDistance = 5.0f;
    private Vector3 OldLeaderPos;

    private Unit FormationLeader;

    

    // Square rules
    private int UnitsPerLine = 4;


    //Like nb of parallel line in the formation
    private int SpecialFormationValue = 3;

    public E_FORMATION_TYPE SetFormationType
    {
        set => FormationType = value;
    }

    public Formation(UnitSquad unitS)
    {
        Squad = unitS;
        //For testing
        //FormationType = E_FORMATION_TYPE.Circle;
        //FormationType = E_FORMATION_TYPE.Arrow;
        FormationType = E_FORMATION_TYPE.Square;
        
    }

    public void UpdateFormationLeader()
    {
        if (Squad.members.Count != 0)
            FormationLeader = Squad.members[0];
    }

    public void CreateFormation(Vector3 targetPos)
    {
        if (Squad.members.Count == 0)
            return;

        FormationLeader = Squad.members[0];

        switch (FormationType)
        {
            case E_FORMATION_TYPE.Circle:
                CreateCircleFormation(targetPos);
                break;
            case E_FORMATION_TYPE.Square:
                CreateSquareFormation(targetPos);
                break;
            case E_FORMATION_TYPE.Arrow:
                CreateArrowFormation(targetPos);
                break;
            case E_FORMATION_TYPE.Custom:
                break;
        }
    }

    void CreateCircleFormation(Vector3 targetPos)
    {
        int numberOfSectors = Squad.members.Count;
        float radius = numberOfSectors * GridDistance / Mathf.PI;

        FormationLeader.GridPosition = targetPos;
        float rotY = FormationLeader.transform.eulerAngles.y;

        for (int i = 1; i < Squad.members.Count; i++)
        {
            Unit currentMember = Squad.members[i];
            Vector3 sizeOffset = new Vector3(currentMember.UnitSize, currentMember.UnitSize, currentMember.UnitSize);
            float angle = i * 2 * Mathf.PI / numberOfSectors;
            Vector3 positionOffset = new Vector3(radius * Mathf.Sin(angle), 0, -radius + radius * Mathf.Cos(angle));
            Vector3 rotationOffset = Quaternion.Euler(0, rotY, 0) * positionOffset;

            currentMember.GridPosition = FormationLeader.GridPosition + sizeOffset + rotationOffset;
        }

        Squad.MoveUnitToPosition();
    }

    void CreateArrowFormation(Vector3 targetPos)
    {
        
        
        FormationLeader.GridPosition = targetPos;
        Transform refTransform = FormationLeader.transform;

        for (int i = 1; i < Squad.members.Count; i++)
        {
            Unit currentMember = Squad.members[i];
            Vector3 sizeOffset = new Vector3(currentMember.UnitSize, currentMember.UnitSize, currentMember.UnitSize);
            Vector3 offset = -refTransform.forward;
            offset += (i % 2 == 0) ? refTransform.right : -refTransform.right;
            
            Squad.members[i].GridPosition = sizeOffset + FormationLeader.GridPosition + (offset * (GridDistance * Mathf.FloorToInt((i + 1) / 2)));

            //Vector3 squadPos = refTransform.position + (offset * (GridDistance * Mathf.FloorToInt((i + 1) / 2)));

        }
        
        Squad.MoveUnitToPosition();
    }

    void CreateSquareFormation(Vector3 targetPos)
    {
        
        FormationLeader.GridPosition = targetPos;
        Transform refTransform = FormationLeader.transform;
        
        // set position in line
        for (int i = 1; i < Squad.members.Count; i++)
        {
            Unit currentMember = Squad.members[i];
            Vector3 sizeOffset = new Vector3(currentMember.UnitSize, currentMember.UnitSize, currentMember.UnitSize);
            int rowIndex = i % UnitsPerLine;
            int lineIndex = Mathf.FloorToInt(i / UnitsPerLine);
            Vector3 offset = refTransform.right * Mathf.FloorToInt((rowIndex + 1) / 2) * (GridDistance + Squad.members[i].UnitSize);
            // set to right or left from leader position
            if (i % 2 == 0)
                offset *= -1.0f;

            // add offset for each new line
            if (lineIndex > 0)
                offset -= refTransform.forward * lineIndex * GridDistance ;
            
            Squad.members[i].GridPosition = FormationLeader.GridPosition + offset + sizeOffset;
        }
        
        Squad.MoveUnitToPosition();
    }
    void CalculatePosDuringMove()
    {
        Vector3 pos;

    }

    /*
     * Choose the leader when a move order is issue
     * The leader is the unit closest to the destination
     */
    void ChooseLeader(Vector3 pos)
    {
        float distance = Vector3.Distance(FormationLeader.transform.position, pos);
        foreach (Unit unit in Squad.members)
        {
            if (Vector3.Distance(unit.transform.position, pos) < distance)
                FormationLeader = unit;
        }
    }
}