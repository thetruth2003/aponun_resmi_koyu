﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager;
    private TileManager tileManager;
    public GameManager gameManager;
    public Toolbar_UI toolbar;        // Toolbar referansı
    public GameObject handObject;     // Karakterin elindeki nesne

    private void Start()
    {
        tileManager = GameManager.instance.tileManager;
        if (tileManager == null)
        {
            Debug.LogError("tileManager is not assigned!");
        }
    }

    private void Update()
    {

    }

    public void DropItem(Item item)
    {
        Vector3 spawnLocation = transform.position;
        Vector3 spawnOffset = Random.insideUnitSphere * 1.25f;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        // Rigidbody2D yerine Rigidbody kullandık
        if (droppedItem.rb != null)
        {
            droppedItem.rb.AddForce(spawnOffset * 0.2f, ForceMode.Impulse);
        }
    }

    public void DropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            DropItem(item);
        }
    }
    private bool isUpdating = false;  // İşlem yapıldığını takip edecek bayrak

    public void UpdateHandObject()
    {
        // Eğer elde 1'den fazla çocuk varsa, önceki nesneyi sil
        if (handObject.transform.childCount > 0)
        {
            Destroy(handObject.transform.GetChild(0).gameObject);
        }

        // Toolbar'daki seçili öğenin adını al
        string selectedItemPrefab = toolbar.GetSelectedPrefab();

        if (!string.IsNullOrEmpty(selectedItemPrefab))
        {
            // Yeni bir prefab yükle (Resources klasöründen)
            GameObject newItem = Resources.Load<GameObject>($"Prefabs/{selectedItemPrefab}");
            if (newItem != null)
            {
                // Yeni objeyi elde ekle
                GameObject instantiatedItem = Instantiate(newItem, handObject.transform);
                instantiatedItem.transform.localPosition = Vector3.zero;
                instantiatedItem.transform.localRotation = Quaternion.identity;
                instantiatedItem.transform.localScale = Vector3.one;
                Debug.Log($"Prefab found and added: {selectedItemPrefab}");
            }
            else
            {
                Debug.LogWarning($"Prefab not found for item: {selectedItemPrefab}");
            }
        }
        else
        {
            Debug.Log("Seçili bir item yok.");
        }
    }

}