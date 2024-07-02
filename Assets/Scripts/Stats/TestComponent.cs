using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    public CombinedStatBlock combinedStatBlock;
    public NewStatBlock statBlock;
    public NewStatBlock statBlock2;

    private void Start()
    {
        List<NewStatBlock> statBlocks = new List<NewStatBlock>();
        statBlocks.Add(statBlock);
        statBlocks.Add(statBlock2);

        combinedStatBlock = new CombinedStatBlock();

        combinedStatBlock.UpdateSources(statBlocks);
    }
}
