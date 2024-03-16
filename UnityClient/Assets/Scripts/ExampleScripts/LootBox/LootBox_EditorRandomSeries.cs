using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RN_LootBoxController))]
public class LootBox_EditorRandomSeries : Editor
{
    public override void OnInspectorGUI()
    {
        RN_LootBoxController controller = (RN_LootBoxController)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Assign Random Items"))
        {
            AssignRandomItems(controller);
        }
    }

    private void AssignRandomItems(RN_LootBoxController controller)
    {
        controller.lootBoxSeries.Clear();

        int seriesCount = Random.Range(1, 6);

        for (int i = 0; i < seriesCount; i++)
        {
            LootBoxSeries series = new LootBoxSeries();
            series.seriesName = "Series " + (i + 1);
            series.probability = Random.Range(0.1f, 1f);

            int itemCount = Random.Range(1, 11);

            for (int j = 0; j < itemCount; j++)
            {
                LootBoxItem item = new LootBoxItem();
                item.itemName =  "Series " + (i + 1) + "-"+ "Item " + (j + 1);
                item.probability = Random.Range(0.1f, 1f);
                GameObject[] prefabs = Resources.LoadAll<GameObject>("GachaObject");
                item.itemPrefab = prefabs[Random.Range(0, prefabs.Length)]; 
                // item.itemPrefab.name = item.itemName;

                series.items.Add(item);
            }

            controller.lootBoxSeries.Add(series);
        }

        EditorUtility.SetDirty(controller);
    }
}