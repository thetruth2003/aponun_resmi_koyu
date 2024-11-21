using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, Inventory_UI> inventoryUIByName = new Dictionary<string, Inventory_UI>(); // Envanterleri saklamak için sözlük
    public List<Inventory_UI> inventoryUIs; // Envanterlerin listesi
    public GameObject inventoryPanel; // Envanter paneli
    public Camera playerCamera; // Oyuncunun kamerası
    public float maxDistance = 100f; // Raycast mesafesi
    public GameObject player; // Oyuncu karakteri
    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        ToggleInventoryUI(); // Envanteri açma/kapama
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Ekrandan ray oluştur
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // Raycast'in vurduğu objenin tag'ini kontrol et
                if (hit.collider.CompareTag("Chest"))
                {
                    ToggleInventoryUI(); // Sandık açıldığında envanteri aç
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            dragSingle = true; // Shift tuşu ile sürükleme tekli yapılacak
        }
        else
        {
            dragSingle = false;
        }
    }
    // Envanteri açma/kapama fonksiyonu
    public void ToggleInventoryUI()
    {
        if (inventoryPanel != null)
        {
            if (!inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(true); // Envanter açılacaksa aktif et
                RefreshInventoryUI("chest"); // Sandık envanterini yenile

                // Mouse imlecini serbest bırak
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                inventoryPanel.SetActive(false); // Envanter kapalıysa, envanteri kapat

                // Envanter kapatıldığında imleci yeniden kilitle
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    // Envanter UI'sını yenileme fonksiyonu
    public void RefreshInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            inventoryUIByName[inventoryName].Refresh(); // Envanteri yenile
        }
    }

    // Tüm envanterleri yenileme
    public void RefreshAll()
    {
        foreach (KeyValuePair<string, Inventory_UI> keyValuePair in inventoryUIByName)
        {
            keyValuePair.Value.Refresh(); // Her envanteri yenile
        }
    }

    // Envanteri al
    public Inventory_UI GetInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            return inventoryUIByName[inventoryName]; // Envanter UI'sini al
        }

        return null;
    }

    // Envanter UI'lerini başlat
    private void Initialize()
    {
        foreach (Inventory_UI ui in inventoryUIs)
        {
            if (!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui); // Envanter UI'lerini sözlüğe ekle
            }
        }
    }
}
