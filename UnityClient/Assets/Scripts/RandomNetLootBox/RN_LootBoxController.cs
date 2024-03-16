using System.Collections;
using System.Collections.Generic;
using Playsphere.RandomSDK.U64;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LootBoxItem
{
    public string itemName;
    public float probability;
    public GameObject itemPrefab;
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
    private ulong ranVal;
    private ulong maxranVal;
    public List<LootBoxSeries> lootBoxSeries = new List<LootBoxSeries>();
    public static UnityAction counterRandom;
    public RNMoveU64ClientController rNMoveU64ClientController;

    void OnEnable()
    {
        u64_ResourcesConverter.RNRandom += OnRNRandom;
    }

    private void OnRNRandom(ulong val, ulong maxval)
    {
        ranVal = val;
        maxranVal = maxval;
        // GetRandomItem();
    }

    public void CallRandomFunction(){
        rNMoveU64ClientController.CallRandomNumber();
    }

    public virtual LootBoxItem GetRandomItem()
    {
        //This is for testing Unity
        rNMoveU64ClientController.CallRandomNumber();
        //

        float totalSeriesProbability = 0f;
        foreach (LootBoxSeries series in lootBoxSeries)
        {
            totalSeriesProbability += series.probability;
        }

        float randomSeriesValue = (float)ranVal/maxranVal * totalSeriesProbability;
        float cumulativeSeriesProbability = 0f;

        foreach (LootBoxSeries series in lootBoxSeries)
        {
            cumulativeSeriesProbability += series.probability;
            if (randomSeriesValue <= cumulativeSeriesProbability)
            {
                int randomItemIndex = (int)((float)ranVal/maxranVal* series.items.Count);
                counterRandom?.Invoke();
                return series.items[randomItemIndex];
            }
        }

        return null;
    }
}