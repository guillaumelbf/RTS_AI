using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILED
    }

    public class Node
    {
        protected NodeState nodeState;
        protected List<Node> childNodes = new List<Node>();

        public Node parent;

        private Dictionary<string, object> data = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node(Node _node)
        {
            Attach(_node);
        }
    
        // Attach Node to another Node
        public void Attach(Node _node)
        {
            _node.parent = this;
            childNodes.Add(_node);
        }
        
        // Default evaluation is FAILED
        public virtual NodeState Evaluate() => NodeState.FAILED;
        
        // Set data to store
        public void SetData(string _key, object _value)
        {
            data[_key] = _value;
        }

        // Get shared data from behaviour tree on branch
        public object GetData(string _key)
        {
            object value = null;

            if (data.TryGetValue(_key, out value))
                return value;

            Node node = parent;

            while (node != null)
            {
                value = node.GetData(_key);
                if (value != null)
                    return value;

                node = node.parent;
            }

            return null;
        }
        
        // Clear shared data from behaviour tree on branch
        public bool ClearData(string _key)
        {
            if (data.ContainsKey(_key))
            {
                data.Remove(_key);
                return true;
            }

            Node node = parent;

            while (node != null)
            {
                bool cleared = node.ClearData(_key);

                if (cleared)
                    return true;
                
                node = node.parent;
            }
            
            return false;
        }
    }
}
