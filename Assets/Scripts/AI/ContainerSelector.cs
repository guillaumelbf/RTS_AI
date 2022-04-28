using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSelector : MonoBehaviour
{
    public BehaviourTree.Selector selector;
    // Start is called before the first frame update

    public BehaviourTree.Selector GetSelectorByIndex(int index)
    {
        

        return selector;
    }

    BehaviourTree.Node myFirstComportement()
    {
        BehaviourTree.Node nom = new BehaviourTree.Node();

        // Voir comment coder le comportement par la suite

        return nom;
    }
  
}
