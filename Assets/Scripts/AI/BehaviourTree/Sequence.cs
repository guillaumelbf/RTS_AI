using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence() : base() {}
        public Sequence(Node _node) : base(_node) {}

        public override NodeState Evaluate()
        {
            bool childIsRunning = false;

            foreach (var node in childNodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        childIsRunning = true;
                        continue;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.FAILED:
                        nodeState = NodeState.FAILED;
                        return nodeState;
                    default:
                        nodeState = NodeState.SUCCESS;
                        return nodeState;
                }
            }

            nodeState = childIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            
            return nodeState;
        }
    }
}
