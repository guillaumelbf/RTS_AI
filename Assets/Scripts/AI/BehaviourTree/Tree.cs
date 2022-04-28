using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    public class Tree : MonoBehaviour
    {
        public Node root = null;
        
        // Start is called before the first frame update
        public void Start()
        {
            root = new Node();
        }

        // Update is called once per frame
        public void Update()
        {
            if (root != null)
                root.Evaluate();
        }

        public void Generate(BehaviourTreeGraph _btGraph)
        {
            XNode.Node xRoot = _btGraph.GetRootNode();
            
            if(xRoot == null)
                return;

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
                switch (node.GetType().Name)
                {
                    case nameof(SelectorNode):
                        currNode = _parentNode.Attach(new Selector());
                        break;
                    case nameof(SequenceNode):
                        currNode = _parentNode.Attach(new Sequence());
                        break;
                    case nameof(TaskNode):
                        //currNode = _parentNode.Attach(new Selector());
                        break;
                }
                
                GenerateChild(node.node,currNode);
            }
        }
    }
}

