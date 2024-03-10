using UnityEngine;

public class LootBoxOpener : MonoBehaviour
{

    [SerializeField] private RN_LootBoxController lootboxSystem;
    [SerializeField] private Animation lootbox;

    [ContextMenu("Open Lootbox")]
    public void OpenLootbox()
    {
        LootBoxItem randomWeapon = lootboxSystem.GetRandomItem();
        Debug.Log("You got a: " + randomWeapon.itemName);
        // Add your logic to handle the obtained weapon
        lootbox.Play();
    }
}