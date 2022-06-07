using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEditor;
using UnityEngine;

// $$$ TO DO :)

public sealed class AIController : UnitController
{
    public float maxCaptureDistance = 0;

    private static ETeam aiControllerTeam;
    
    #region MonoBehaviour methods

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        aiControllerTeam = Team;
    }

    protected override void Update()
    {
        base.Update();
    }

    public List<Unit> GetAllUnitsAvailable()
    {
        List<Unit> availableUnits = new List<Unit>();
        foreach (var unit in GetAllUnits())
        {
            if (!unit.isWorking)
            {
                availableUnits.Add(unit);
            }
        }

        return availableUnits;
    }

    public int CountUnitInWorkMode()
    {
        int result = 0;

        foreach (Unit unit in GetAllUnits())
            if (unit.isWorking)
                result++;

        return result;
    }

    public static ETeam GetAiControllerTeam()
    {
        return aiControllerTeam;
    }

    #endregion
}
