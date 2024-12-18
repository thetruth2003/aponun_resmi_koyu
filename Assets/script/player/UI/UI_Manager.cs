using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, Inventory_UI> inventoryUIByName = new Dictionary<string, Inventory_UI>(); // Envanterleri saklamak için sözlük
    public List<Inventory_UI> inventoryUIs; // Envanterlerin listesi
    public GameObject inventoryPanel; // Envanter paneli
    public GameObject MenuPanel; // menuneli
    public GameObject Stamina; // Envanter paneli
    public Camera playerCamera; // Oyuncunun kamerası
    public float maxDistance = 100f; // Raycast mesafesi
    public GameObject player; // Oyuncu karakteri
    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;
    public SC_FPSController playerMovementScript; 
    public GameObject Crosshair;
    public GameObject CrosshairCanvas;
    private bool isMenuOpen = false; // Menü durumu
    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChestOpen();
            Debug.Log("asd");
        }
        if (Input.GetKey(KeyCode.Q) && !isMenuOpen)
        {
            ToggleMenuUI(); // Menü aç
            isMenuOpen = true;         // Durum güncellenir
        }
        else if (Input.GetKeyUp(KeyCode.Q) && isMenuOpen)
        {
            ToggleMenuUI(); // Menü kapat
            
            isMenuOpen = false;         // Durum güncellenir
        }
    }
    private void ChestOpen()
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
            else
            {
                ToggleInventoryUI(); // Sandık açıldığında envanteri aç
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
        RefreshAll();//bunun yuzunden oyunun basında envanter acılıyor unutma
        Debug.Log("bunun yzuunden oyun basında envanter acılıyor unutma RefreshAll(); uste");
        if (inventoryPanel != null)
        {
            if (!inventoryPanel.activeSelf)
            {
                // Envanter açılacaksa
                inventoryPanel.SetActive(true); // Envanteri aç

                // Kamera ve karakter hareketini durdur
                playerMovementScript.enabled = false;

                // Mouse imlecini serbest bırak
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                RefreshInventoryUI("Chest"); // Sandık envanterini yenile
                // Envanter kapatılacaksa
                inventoryPanel.SetActive(false); // Envanteri kapat

                // Kamera ve karakter hareketini tekrar etkinleştir
                playerMovementScript.enabled = true;

                // Envanter kapatıldığında imleci yeniden kilitle
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    public void ToggleMenuUI()
    {
        if (MenuPanel != null)
        {
            if (!MenuPanel.activeSelf)
            {
                // Envanter açılacaksa
                MenuPanel.SetActive(true); // Envanteri aç
                // Mouse imlecini serbest bırak
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                playerMovementScript.enabled = false;
                Crosshair.SetActive(false);
                CrosshairCanvas.SetActive(false);
            }
            else
            {
                MenuPanel.SetActive(false); // Envanteri kapat
                // Envanter kapatıldığında imleci yeniden kilitle
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerMovementScript.enabled = true;
                Crosshair.SetActive(true);
                CrosshairCanvas.SetActive(true);
                DeactivateMenuAndChildren(MenuPanel);
            }
        }
    }
    void DeactivateMenuAndChildren(GameObject menu)
    {
        // Menü ve tüm child'larını pasif yap

        // Eğer menü altındaki nesneleri manuel olarak da kapatmak isterseniz
        foreach (Transform child in menu.transform)
        {
            child.gameObject.SetActive(false);
            menu.transform.GetChild(0).gameObject.SetActive(true);
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
