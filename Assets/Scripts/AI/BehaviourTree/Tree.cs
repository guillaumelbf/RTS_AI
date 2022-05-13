using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    public class Tree : MonoBehaviour
    {
        public Node root = null;
        protected ContainerTask containerTask;
        
        // Start is called before the first frame update
        public virtual void Start()
        {
            root = new Selector();
            containerTask = gameObject.AddComponent<ContainerTask>();
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (root != null)
                root.Evaluate();
        }

        public void Generate(BehaviourTreeGraph _btGraph)
        {
            XNode.Node xRoot = _btGraph.GetRootNode();

            GenerateChild(xRoot, root);
        }
    
        //Recursively attach each node together 
        private void GenerateChild(XNode.Node _parentXNode, Node _parentNode)
        {
            if(_parentXNode == null || _parentNode == null || !_parentXNode.Outputs.Any())
                return;

            foreach (var node in _parentXNode.Outputs.First().GetConnections())
            {
                Node currNode = null;
                switch (node.node.GetType().Name)
                {
                    case nameof(SelectorNode):
                        SelectorNode selectorNode = (SelectorNode)node.node;
                        currNode = _parentNode.Attach(selectorNode.order,new Selector());
                        break;
                    case nameof(SequenceNode):
                        SequenceNode sequenceNode = (SequenceNode)node.node;
                        currNode = _parentNode.Attach(sequenceNode.order,new Sequence());
                        break;
                    case nameof(TaskNode):
                        TaskNode taskNode = (TaskNode)node.node;
                        currNode = _parentNode.Attach(taskNode.order,containerTask.GetTask(((TaskNode)node.node).taskName));
                        break;
                }
                
                GenerateChild(node.node,currNode);
            }
        }
    }
}

