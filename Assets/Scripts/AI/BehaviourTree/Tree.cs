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
            if(_parentXNode == null || _parentNode == null)
                return;
            
            foreach (var node in _parentXNode.Outputs.First().GetConnections())
            {
                Node currNode = null;
                switch (node.node.GetType().Name)
                {
                    case nameof(SelectorNode):
                        currNode = _parentNode.Attach(new Selector());
                        break;
                    case nameof(SequenceNode):
                        currNode = _parentNode.Attach(new Sequence());
                        break;
                    case nameof(TaskNode):
                        currNode = _parentNode.Attach(containerTask.GetTask(((TaskNode)node.node).taskName));
                        break;
                }
                
                GenerateChild(node.node,currNode);
            }
        }
    }
}

