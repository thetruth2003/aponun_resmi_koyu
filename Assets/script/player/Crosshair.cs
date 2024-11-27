﻿using UnityEditor.UIElements;
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
    public TreeFall TreeFall;
    public Toolbar_UI toolbar;
    public TreeFall tree;

    public void Update()
    {
        // Fare tıklaması ile etkileşime gir
        if (Input.GetMouseButtonDown(0))
        {
            ShootRay();
            HitTree();
        }
        if (Input.GetMouseButtonDown(1))
        {
            ChangeCell();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
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
    public void HitTree()
    {
        // Nişangah pozisyonuna göre ray oluştur
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast ile tıklanan hücreyi bul
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            GameObject clickedCell = hit.collider.gameObject; // Tıklanan hücreyi al

            // Katman kontrolü ve seçili öğe adı kontrolü
            if (clickedCell.layer == LayerMask.NameToLayer("Tree") && toolbar.GetSelectedPrefab() == "axe")
            {
                // TreeFall bileşenini tıklanan objeden al
                TreeFall tree = clickedCell.GetComponent<TreeFall>();

                if (tree != null && !tree.isFalling)
                {
                    // Ağacı devirmek için ShakeAndFall coroutine'ini başlat
                    StartCoroutine(tree.ShakeAndFall());
                }
                else
                {
                    Debug.Log("Bu ağaç zaten devrilmiş.");
                }
            }
            else
            {
                // Şartlar sağlanmadığında kullanıcıyı bilgilendir
                Debug.Log("Ağaç değil veya elinde balta yok");
            }
        }
    }


    // Fare ile tıklanarak hücre değiştirilir
    public void ChangeCell()
    {
        // Nişangah pozisyonuna göre ray oluştur
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast ile tıklanan hücreyi bul
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            GameObject clickedCell = hit.collider.gameObject; // Tıklanan hücreyi al

            // Katman kontrolü ve seçili öğe adı kontrolü
            if (clickedCell.layer == LayerMask.NameToLayer("ground") && toolbar.GetSelectedPrefab() == "Hoe")
            {
                // Hücreyi sil ve yerine yeni hücre oluştur
                Vector3 cellPosition = clickedCell.transform.position;
                Quaternion cellRotation = clickedCell.transform.rotation;
                Vector3 cellScale = clickedCell.transform.localScale;

                // Yeni hücreyi oluştur
                GameObject newCell = Instantiate(replacementPrefab, cellPosition, cellRotation);
                newCell.transform.localScale = cellScale;

                // Eski hücreyi yok et
                Destroy(clickedCell);

                Debug.Log("Hücre başarıyla değiştirildi.");
            }
            else
            {
                // Şartlar sağlanmadığında kullanıcıyı bilgilendir
                Debug.Log("katman ground değil veya elinde hoe yok");
            }
        }
    }

    // Fare tıklama ile seçilen hücrenin rengini değiştirir ve aktif hale getirir

    public void ActivateCellAtMousePosition()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Nişangahın ekran üzerindeki pozisyonundan ray oluştur
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer)) // Raycast ile vurulan nesneyi bul
        {
            GameObject clickedCell = hit.collider.gameObject; // Vurulan hücreyi al

            // Eğer hücre zemin katmanına aitse
            if (clickedCell.layer == LayerMask.NameToLayer("groundcell") && toolbar.GetSelectedPrefab() == "Hammer")
            {
                clickedCell.transform.GetChild(0).gameObject.SetActive(true); // Child objeyi aktif yap
            }
        }
    }
    public void AddSeed()
    {
        // Nişangah pozisyonuna göre ray oluştur
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast ile tıklanan hücreyi bul
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            GameObject clickedCell = hit.collider.gameObject; // Tıklanan hücreyi al

            // Katman kontrolü ve seçili öğe adı kontrolü
            if (clickedCell.layer == LayerMask.NameToLayer("SeedBox") && toolbar.GetSelectedPrefabTag() == "seed")
            {
                clickedCell.transform.GetChild(0).gameObject.SetActive(true); // Child objeyi aktif yap
            }
            else
            {
                // Şartlar sağlanmadığında kullanıcıyı bilgilendir
                Debug.Log("seedbox değil yada elinde seed yok");
            }
        }
    }
}





