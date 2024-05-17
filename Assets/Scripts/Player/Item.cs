using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public StatBlock[] itemStats = new StatBlock[]
    {
        new StatBlock(StatBlock.BlockType.Additive),
        new StatBlock(StatBlock.BlockType.Multiplicative)
    };
}
