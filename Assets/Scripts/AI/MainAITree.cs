using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;
public class MainAITree : BT.Tree
{
    [SerializeField] private BehaviourTreeGraph btGraph;
    [SerializeField] private InfluenceMap influenceMap;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
        containerTask.AddTask(new CreateUnitTask(GetComponent<AIController>()));
        containerTask.AddTask(new DefenseTask(GetComponent<AIController>(), influenceMap));
        containerTask.AddTask(new MoveUnitTask());
        containerTask.AddTask(new CaptureTask(GetComponent<AIController>()));
        containerTask.AddTask(new AttackTask(GetComponent<AIController>()));
        containerTask.AddTask(new PatrolTask(GetComponent<AIController>()));


        containerTask.AddTask(new GotUnitDecorator(GetComponent<AIController>()));
        containerTask.AddTask(new NeedDefDecorator(GetComponent<AIController>()));
        containerTask.AddTask(new HasPointToCaptureDecorator(GetComponent<AIController>()));
        containerTask.AddTask(new CheckScoreArmy(GetComponent<AIController>(), influenceMap));


        Generate(btGraph);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
