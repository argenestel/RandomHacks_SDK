using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(RN_LootBoxController))]
public class RN_DynamicProbability : MonoBehaviour
{
    public float probabilityIncreasePercent = 1f;
    private RN_LootBoxController lootBoxController;
    private int callCount = 0;
    void OnEnable()
    {
        RN_LootBoxController.counterRandom += GetRandomItem;
    }
    void OnDisable()
    {
        RN_LootBoxController.counterRandom -= GetRandomItem;
    }

    private void Awake()
    {
        lootBoxController = GetComponent<RN_LootBoxController>();
        if (lootBoxController == null)
        {
            lootBoxController = gameObject.AddComponent<RN_LootBoxController>();
        }
    }

    private void Start()
    {
        AdjustProbabilities();
    }

    public void GetRandomItem()
    {
        callCount++;
        AdjustProbabilities();
    }

    private void AdjustProbabilities()
    {
        float totalProbability = 0f;
        foreach (LootBoxSeries series in lootBoxController.lootBoxSeries)
        {
            totalProbability += series.probability;
        }

        float probabilityIncreaseAmount = totalProbability * (probabilityIncreasePercent / 100f);
        float cumulativeProbability = 0f;

        for (int i = 0; i < lootBoxController.lootBoxSeries.Count; i++)
        {
            LootBoxSeries series = lootBoxController.lootBoxSeries[i];
            float adjustedProbability = series.probability + (probabilityIncreaseAmount * (i + 1));
            series.probability = adjustedProbability;
            cumulativeProbability += adjustedProbability;
        }

        float normalizationFactor = totalProbability / cumulativeProbability;
        foreach (LootBoxSeries series in lootBoxController.lootBoxSeries)
        {
            series.probability *= normalizationFactor;
        }
    }
}