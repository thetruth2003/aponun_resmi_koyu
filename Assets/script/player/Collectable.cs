using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    // Nesneyi tetikleyici olarak ayarlayalım
    private void Awake()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    // Raycast ile çalışacak Collect() metodu
    public void Collect()
    {
        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            Item item = GetComponent<Item>();

            if (item != null)
            {
                // Eşyayı envantere ekle ve nesneyi yok et
                player.inventoryManager.Add("backpack", item);
                Debug.Log($"{gameObject.name} toplandı!");
                Destroy(item.gameObject);
            }
        }
    }
}
