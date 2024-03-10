using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PityLootBoxController : RN_LootBoxController
{
    public int maxPityCounter = 10;
    public LootBoxSeries guaranteedSeries;
    public LootBoxItem guaranteedItem;
    private int callCount = 0;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        RN_LootBoxController.counterRandom += OnPityCounter;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        RN_LootBoxController.counterRandom -= OnPityCounter;
    }

    private void OnPityCounter()
    {
        callCount++;
    }

    public override LootBoxItem GetRandomItem()
    {
        if (callCount >= maxPityCounter)
        {
            callCount = 0;

            if (guaranteedSeries != null)
            {
                int randomItemIndex = UnityEngine.Random.Range(0, guaranteedSeries.items.Count);
                return guaranteedSeries.items[randomItemIndex];
            }
            else if (guaranteedItem != null)
            {
                return guaranteedItem;
            }
        }

        return base.GetRandomItem();
    }
}