using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RN_LootBoxController : MonoBehaviour
{
    [System.Serializable]
    public class WeaponProbability
    {
        public string weaponName;
        public float probability;
    }

    public List<WeaponProbability> weaponProbabilities = new List<WeaponProbability>();

    public string GetRandomWeapon()
    {
        float totalProbability = 0f;
        foreach (WeaponProbability wp in weaponProbabilities)
        {
            totalProbability += wp.probability;
        }

        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        foreach (WeaponProbability wp in weaponProbabilities)
        {
            cumulativeProbability += wp.probability;
            if (randomValue <= cumulativeProbability)
            {
                return wp.weaponName;
            }
        }

        return string.Empty;
    }
}
