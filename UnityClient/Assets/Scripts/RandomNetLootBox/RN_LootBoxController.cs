using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LootBoxItem
{
    public string itemName;
    public float probability;
}

[System.Serializable]
public class LootBoxSeries
{
    public string seriesName;
    public float probability;
    public List<LootBoxItem> items = new List<LootBoxItem>();
}

public class RN_LootBoxController : MonoBehaviour
{
    public List<LootBoxSeries> lootBoxSeries = new List<LootBoxSeries>();
    public static UnityAction counterRandom;
    public virtual LootBoxItem GetRandomItem()
    {
        float totalSeriesProbability = 0f;
        foreach (LootBoxSeries series in lootBoxSeries)
        {
            totalSeriesProbability += series.probability;
        }

        float randomSeriesValue = Random.Range(0f, totalSeriesProbability);
        float cumulativeSeriesProbability = 0f;

        foreach (LootBoxSeries series in lootBoxSeries)
        {
            cumulativeSeriesProbability += series.probability;
            if (randomSeriesValue <= cumulativeSeriesProbability)
            {
                int randomItemIndex = Random.Range(0, series.items.Count);
                counterRandom?.Invoke();
                return series.items[randomItemIndex];
            }
        }

        return null;
    }
}