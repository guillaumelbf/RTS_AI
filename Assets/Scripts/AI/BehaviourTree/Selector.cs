using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : Node
    {
        private Node decorator = null;

        public Selector(Node _decorator = null) : base()
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
            
            foreach (var node in childNodes.Values)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        nodeState = NodeState.RUNNING;
                        return nodeState;
                    case NodeState.SUCCESS:
                        nodeState = NodeState.SUCCESS;
                        return nodeState;
                    case NodeState.FAILED:
                        continue;
                    default:
                        continue;
                }
            }

            nodeState = NodeState.FAILED;

            return nodeState;
        }
    }
}
