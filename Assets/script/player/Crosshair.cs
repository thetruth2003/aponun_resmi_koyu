using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Camera playerCamera; // Oyuncunun kamerası
    public float maxDistance = 100f; // Maksimum atış mesafesi
    public LayerMask interactableLayer; // Etkileşimde bulunulacak katman
    public GameObject player; // Oyuncu karakteri
    public DynamicGridManager gridManager;
    public GameObject replacementPrefab; // Yerine geçecek prefab
    public UI_Manager manager;
    public static bool dragSingle;

    public void Update()
    {
        // Fare tıklaması ile etkileşime gir
        if (Input.GetMouseButtonDown(0))
        {
            ShootRay();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            manager.RefreshAll();
            ChestOpen();
        }
    }
    public void ShootRay()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            // Etkileşimli nesneye ulaşıldıysa
            Debug.Log("Etkileşim: " + hit.collider.name);

            // Collectable bileşeni olup olmadığını kontrol et
            Collectable collectable = hit.collider.GetComponent<Collectable>();

            if (collectable != null)
            {
                // Nesnenin Collect metodunu çağırarak tetikle
                collectable.Collect();
            }
            gridManager.ChangeCell();
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
                manager.ToggleInventoryUI(); // Sandık açıldığında envanteri aç
            }
            else
            {
                manager.ToggleInventoryUI(); // Sandık açıldığında envanteri aç
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
}





