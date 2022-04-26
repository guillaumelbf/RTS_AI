using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

// $$$ TO DO :)

public sealed class AIController : UnitController
{
    [SerializeField] private BehaviourTreeGraph btGraph;
    private BehaviourTree.Tree btTree = new BehaviourTree.Tree();
    
    #region MonoBehaviour methods

    protected override void Awake()
    {
        base.Awake();
        btTree.Generate(btGraph);
    }

    protected override void Start()
    {
        base.Start();
        
        btTree.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        btTree.Update();
    }

    #endregion
}
