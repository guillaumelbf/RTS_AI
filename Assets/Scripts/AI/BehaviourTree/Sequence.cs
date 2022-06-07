using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        private Node decorator = null;

        public Sequence(Node _decorator = null) : base()
        {
            decorator = _decorator;
        }

        public override NodeState Evaluate()
        {
            if (decorator != null)
            {
                if(decorator.Evaluate() == NodeState.FAILED)
                    return NodeState.FAILED;
            }
            
            bool childIsRunning = false;

            foreach (var node in childNodes.Values)
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
