using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ArrowFormationRule", menuName = "FormationRules/Arrow", order = 1)]
public class ArrowFormationRule : FormationRule
{
    
    [SerializeField] float Distance = 2.0f;
    
    override public Vector3 ComputeFormationPosition(Transform refTransform, int index)
    {
        if (index == 0)
            return refTransform.position;

        Vector3 offset = -refTransform.forward;
        offset += (index % 2 == 0) ? refTransform.right : -refTransform.right;

        Vector3 squadPos = refTransform.position + (offset * (Distance * Mathf.FloorToInt((index + 1) / 2)));

        return squadPos;
    }
}
