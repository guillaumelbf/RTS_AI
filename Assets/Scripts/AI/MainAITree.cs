using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;
public class MainAITree : BT.Tree
{
    [SerializeField] private BehaviourTreeGraph btGraph;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
        containerTask.AddTask(new CreateUnitTask(GetComponent<AIController>()));
        containerTask.AddTask(new MoveUnitTask());

        Generate(btGraph);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
