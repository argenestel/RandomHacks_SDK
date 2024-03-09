using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RN_LootBoxController : MonoBehaviour
{
    //<summary>: This class is used to generate a random weapon from a list of weapons with different probabilities. The class has a list of WeaponProbability objects, each of which contains the name of the weapon and its probability. The GetRandomWeapon() method returns the name of a random weapon from the list, based on the probabilities of each weapon.
    //</summary> 
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
