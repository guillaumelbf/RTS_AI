using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Tree : MonoBehaviour
    {
        public Node root = null;
        
        // Start is called before the first frame update
        void Start()
        {
            root = new Node();
        }

        // Update is called once per frame
        void Update()
        {
            if (root != null)
                root.Evaluate();
        }
    }
}

