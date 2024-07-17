using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    public CombinedStatBlock combinedStatBlock;
    public StatBlock statBlock;
    public StatBlock statBlock2;

    private void Start()
    {
        List<StatBlock> statBlocks = new List<StatBlock>();
        statBlocks.Add(statBlock);
        statBlocks.Add(statBlock2);

        combinedStatBlock = new CombinedStatBlock();

        combinedStatBlock.UpdateSources(statBlocks);
    }
}
