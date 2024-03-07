using UnityEngine;

public class LootBoxOpener : MonoBehaviour
{

    [SerializeField] private RN_LootBoxController lootboxSystem;
    public void OpenLootbox()
    {
        string randomWeapon = lootboxSystem.GetRandomWeapon();
        Debug.Log("You got a: " + randomWeapon);
        // Add your logic to handle the obtained weapon
    }
}