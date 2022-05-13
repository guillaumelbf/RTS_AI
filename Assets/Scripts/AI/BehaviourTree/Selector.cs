using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base() {}
        public Selector(Node _node) : base(_node) {}

        public override NodeState Evaluate()
        {

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
