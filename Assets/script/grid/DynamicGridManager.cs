﻿using UnityEngine;

public class DynamicGridManager : MonoBehaviour
{
    [SerializeField] private GameObject gridCellPrefab; // Hücre prefab'ı (önceden oluşturulmuş şablon)
    [SerializeField] private int gridWidth = 50; // Grid genişliği
    [SerializeField] private int gridHeight = 50; // Grid yüksekliği
    [SerializeField] private float cellSize = 2.5f; // Hücre boyutu
    [SerializeField] private int renderDistance = 10; // Görünürlük mesafesi
    [SerializeField] private Transform player; // Oyuncu objesi
    [SerializeField] private GameObject[,] gridCells; // Hücrelerin dizisi
    [SerializeField] private float updateInterval = 0.1f; // Güncelleme aralığı
    private float timeSinceLastUpdate = 0f; // Son güncelleme zamanı
    [SerializeField] private Crosshair crosshair; // Crosshair (nişangah) nesnesi
    private GameObject selectedCell; // Bu değişken, seçilen hücreyi tutacak


    void Start()
    {
        // Eğer gridCellPrefab atanmadıysa hata verir
        if (!gridCellPrefab)
        {
            Debug.LogError("Grid hücre prefab'ı atanmadı!");
            return;
        }

        // Eğer oyuncu objesi atanmadıysa, tag kullanarak bulmaya çalışır
        if (!player)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (!player)
            {
                Debug.LogError("Oyuncu objesi bulunamadı!");
                return;
            }
        }

        CreateGrid(); // Grid'i oluştur
    }

    // Her frame'de grid'i güncellemek için
    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime; // Son güncellemeden geçen süreyi artır
        if (timeSinceLastUpdate >= updateInterval) // Güncelleme aralığına ulaşıldıysa
        {
            UpdateGridVisibility(); // Grid'in görünürlüğünü güncelle
            timeSinceLastUpdate = 0f; // Zamanı sıfırla
        }

        // Sağ tıklama ile hücreyi etkinleştir
        if (Input.GetMouseButtonDown(1))
        {
            ActivateCellAtMousePosition(); // Fare pozisyonundaki hücreyi etkinleştir
        }
    }

    // Grid'i oluşturur
    private void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight]; // Grid hücrelerini başlat
        Vector3 gridOrigin = player.position - new Vector3((gridWidth / 2) * cellSize, 0, (gridHeight / 2) * cellSize); // Grid'in başlangıç noktasını hesapla
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 cellPosition = new Vector3(gridOrigin.x + x * cellSize,2, gridOrigin.z + z * cellSize); // Hücrenin pozisyonunu hesapla
                if (Physics.Raycast(cellPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20f)) // Raycast ile zemin yüksekliğini al
                {
                    cellPosition.y = hit.point.y; // Yüksekliği güncelle
                }

                // Hücreyi instantiate et
                GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                AlignToSurface(newCell, hit); // Hücreyi yüzeye hizala
                gridCells[x, z] = newCell; // Hücreyi gridCells dizisine ekle
            }
        }
    }

    // Hücreyi yüzeyle hizalar
    private void AlignToSurface(GameObject cell, RaycastHit hit)
    {
        if (hit.collider != null)
        {
            cell.transform.position = hit.point; // Hücrenin pozisyonunu hizala
            cell.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // Hücrenin rotasını hizala
        }
    }

    // Grid'in görünürlüğünü günceller
    private void UpdateGridVisibility()
    {
        if (!player) return; // Eğer oyuncu objesi yoksa hiçbir şey yapma

        int halfWidth = gridWidth / 2;
        int halfHeight = gridHeight / 2;

        // Her hücreyi kontrol et ve oyuncuya ne kadar yakın olduğunu hesapla
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GameObject cell = gridCells[x, z];
                if (cell == null) continue;

                float distance = Vector3.Distance(player.position, cell.transform.position); // Oyuncuya olan mesafeyi hesapla
                cell.SetActive(distance <= renderDistance * cellSize); // Mesafe, renderDistance ile karşılaştır ve aktifliğini ayarla
            }
        }
    }

    // Fare ile tıklanarak hücre değiştirilir
    public void ChangeCell()
    {
        Ray ray = crosshair.playerCamera.ScreenPointToRay(Input.mousePosition); // Nişangah pozisyonuna göre ray oluştur
        if (Physics.Raycast(ray, out RaycastHit hit, crosshair.maxDistance, crosshair.interactableLayer)) // Raycast ile tıklanan hücreyi bul
        {
            GameObject clickedCell = hit.collider.gameObject; // Tıklanan hücreyi al
            if (clickedCell.layer != LayerMask.NameToLayer("ground")) // Eğer zemin katmanına ait değilse
            {
                Debug.Log("Sadece 'ground' katmanındaki nesneler için geçerlidir.");
                return;
            }

            // Hücreyi sil ve yerine yeni hücreyi instantiate et
            Vector3 cellPosition = clickedCell.transform.position;
            Quaternion cellRotation = clickedCell.transform.rotation;
            Vector3 cellScale = clickedCell.transform.localScale;

            Destroy(clickedCell); // Eski hücreyi yok et
            GameObject newCell = Instantiate(crosshair.replacementPrefab, cellPosition, cellRotation); // Yeni hücreyi oluştur
            newCell.transform.localScale = cellScale; // Yeni hücrenin boyutunu eski hücreyle aynı yap

            Debug.Log("Hücre başarıyla değiştirildi.");
        }
    }
    // Fare tıklama ile seçilen hücrenin rengini değiştirir ve aktif hale getirir

    private void ActivateCellAtMousePosition()
    {
        Ray ray = crosshair.playerCamera.ScreenPointToRay(Input.mousePosition); // Nişangahın ekran üzerindeki pozisyonundan ray oluştur
        if (Physics.Raycast(ray, out RaycastHit hit, crosshair.maxDistance, crosshair.interactableLayer)) // Raycast ile vurulan nesneyi bul
        {
            GameObject clickedCell = hit.collider.gameObject; // Vurulan hücreyi al

            // Eğer hücre zemin katmanına aitse
            if (clickedCell.layer == LayerMask.NameToLayer("ground"))
            {
                clickedCell.transform.GetChild(0).gameObject.SetActive(true); // Child objeyi aktif yap
            }
        }
    }

    // Hücrenin collider'ını etkinleştirir
    private bool isColliderVisible = false; // Collider'ın görünür olup olmadığını kontrol eden değişken

    private void ActivateCellCollider(GameObject clickedCell)
    {
        Collider clickedCollider = clickedCell.GetComponent<Collider>(); // Collider'ı al
        if (clickedCollider != null)
        {
            clickedCollider.enabled = true; // Collider'ı etkinleştir
            isColliderVisible = true; // Collider'ı görünür yap
            clickedCell.SetActive(true); // Hücreyi aktif hale getir
            Debug.Log("Seçilen hücre kırmızıya boyandı.");
        }
    }
}
