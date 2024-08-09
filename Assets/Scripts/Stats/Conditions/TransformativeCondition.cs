using UnityEngine;

public class TransformativeCondition : StatCondition
{
    [SerializeReference, SerializeReferenceMenu] 
    public Stat referenceStatType;
    public override float CheckCondition(GameContext context)
    {
        return (referenceStatType == null) ? 0f : 1f;
    }
}