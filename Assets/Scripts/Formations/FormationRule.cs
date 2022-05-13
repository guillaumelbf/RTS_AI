using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FormationRule : ScriptableObject
{
    virtual public Vector3 ComputeFormationPosition(Transform refTransform, int index)
    {
        return Vector3.zero;
    }
}
