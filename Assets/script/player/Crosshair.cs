using UnityEditor.UIElements;
using UnityEngine;
using System.Collections; // IEnumerator kullanabilmek için gerekli namespace

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
    public GameObject WateringCan_full;
    private bool isMenuOpen = false; // Menü durumu


    public void Update()
    {
        // Fare tıklaması ile etkileşime gir
        if (Input.GetMouseButtonDown(0))
        {
            ShootRay();
            HitTree();
            AddSeed();
            Watering();
        }
        if (Input.GetMouseButtonDown(1))
        {
            ChangeCell();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChestOpen();
            Debug.Log("asd");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            manager.ToggleMenuUI(); // Menü aç
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

            // Tıklanan hücre SeedBox katmanında mı ve seçili öğe "seed" mi kontrol et
            if (clickedCell.layer == LayerMask.NameToLayer("SeedBox") && toolbar.GetSelectedPrefabTag() == "seed")
            {
                string selectedItemUsedPrefab = toolbar.GetSelectedUsedPrefab();

                if (!string.IsNullOrEmpty(selectedItemUsedPrefab))
                {
                    // Resources klasöründen prefab'ı yükle
                    GameObject newItem = Resources.Load<GameObject>($"Prefabs/{selectedItemUsedPrefab}");

                    if (newItem != null)
                    {
                        // Yeni prefab'ı hücrenin merkezine spawnla
                        Vector3 spawnPosition = clickedCell.transform.position; // Hücrenin pozisyonu
                        Quaternion spawnRotation = Quaternion.identity; // Varsayılan rotasyon

                        Instantiate(newItem, spawnPosition, spawnRotation);

                        Debug.Log($"Seed prefab spawned: {selectedItemUsedPrefab} at {spawnPosition}");
                    }
                    else
                    {
                        Debug.LogWarning($"Prefab bulunamadı: {selectedItemUsedPrefab}");
                    }
                }
            }
            else
            {
                // Şartlar sağlanmadığında kullanıcıyı bilgilendir
                Debug.Log("Tıklanan hücre SeedBox değil veya seçili öğe 'seed' değil.");
            }
        }
        else
        {
            Debug.Log("Raycast bir objeye çarpmadı.");
        }
    }
    public void Watering()
    {
        // Nişangah pozisyonuna göre ray oluştur
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast ile tıklanan hücreyi bul
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            GameObject clickedCell = hit.collider.gameObject; // Tıklanan hücreyi al

            // Tıklanan hücre SeedBox katmanında mı ve seçili öğe "seed" mi kontrol et
            if (clickedCell.layer == LayerMask.NameToLayer("SeedBox") && toolbar.GetSelectedPrefab() == "WateringCan_full")
            {
                string selectedItemUsedPrefab = toolbar.GetSelectedUsedPrefab();

                if (!string.IsNullOrEmpty(selectedItemUsedPrefab))
                {
                    // Resources klasöründen prefab'ı yükle
                    GameObject newItem = Resources.Load<GameObject>($"Prefabs/{selectedItemUsedPrefab}");

                    if (newItem != null)
                    {
                        // Yeni prefab'ı hücrenin merkezine spawnla
                        Vector3 spawnPosition = clickedCell.transform.position; // Hücrenin pozisyonu
                        Quaternion spawnRotation = Quaternion.identity; // Varsayılan rotasyon
                        Instantiate(newItem, spawnPosition, spawnRotation);
                        Debug.Log($"Seed prefab spawned: {selectedItemUsedPrefab} at {spawnPosition}");
                        StartCoroutine(waterfall());
                    }
                    else
                    {
                        Debug.LogWarning($"Prefab bulunamadı: {selectedItemUsedPrefab}");
                    }
                }
            }
            else
            {
                // Şartlar sağlanmadığında kullanıcıyı bilgilendir
                Debug.Log("Tıklanan hücre SeedBox değil veya seçili öğe 'seed' değil.");
            }
        }
        else
        {
            Debug.Log("Raycast bir objeye çarpmadı.");
        }
    }
    public IEnumerator waterfall()
    {
        WateringCan_full.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        WateringCan_full.transform.GetChild(0).gameObject.SetActive(true);
    }
}





