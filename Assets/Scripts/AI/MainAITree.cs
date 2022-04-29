using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT = BehaviourTree;
public class MainAITree : BT.Tree
{
    [SerializeField] private BehaviourTreeGraph btGraph;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        
        containerTask.AddTask(new CreateUnitTask(GetComponent<AIController>()));
        
        Generate(btGraph);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
