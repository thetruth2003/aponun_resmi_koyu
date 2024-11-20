using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar_UI : MonoBehaviour
{
    public Movement movement;
    public List<Slot_UI> toolbarSlots = new List<Slot_UI>();
    private Slot_UI selectedSlot;

    public GameObject handObject; // Karakterin elindeki boş GameObject
    public Transform handTransform; // Elin pozisyonunu ayarlamak için

    private void Start()
    {
        SelectSlot(0); // Oyuna başladığında ilk slot seçili olsun
    }

    private void Update()
    {
        CheckAlphaNumericKeys();
    }

    public void SelectSlot(Slot_UI slot)
    {
        SelectSlot(slot.slotID);
    }

    public string GetSelectedPrefab()
    {
        return selectedSlot != null && selectedSlot.inventorySlot != null ? selectedSlot.inventorySlot.itemPrefab.name : null;
    }

    public void SelectSlot(int index)
    {
        if (toolbarSlots.Count == 9)
        {
            if (selectedSlot != null)
            {
                selectedSlot.SetHighlight(false);
            }

            selectedSlot = toolbarSlots[index];
            selectedSlot.SetHighlight(true);
            GameManager.instance.player.UpdateHandObject();

            // Seçili slotta bir eşya varsa el nesnesini güncelle
            if (selectedSlot.inventorySlot != null)
            {
                Debug.Log("Selected item: " + selectedSlot.inventorySlot.itemName); // İtem adını kontrol için
                Debug.Log("Selected item: " + selectedSlot.inventorySlot.itemPrefab); // İtem adını kontrol için
            }

            GameManager.instance.player.inventoryManager.toolbar.SelectSlot(index);
        }
    }

    private void CheckAlphaNumericKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSlot(8);
    }

}
