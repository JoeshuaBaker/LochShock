using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplaceItemStats : MonoBehaviour
{
    [MenuItem("Utilities/ReplaceAllItemStats")]
    static void ReplaceAllItemStats()
    {
        Item[] items = Resources.LoadAll<Item>("Prefabs");

        foreach (Item foundScript in items)
        {
            Debug.Log("Found the script in: " + foundScript.gameObject);

            //foundScript.TransferStats();

            UnityEditor.EditorUtility.SetDirty(foundScript);
        }
    }
}
