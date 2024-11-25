using UnityEngine;

public class DynamicGridManager : MonoBehaviour
{
    public GameObject gridCellPrefab;  // Hücre prefab'ı
    public int gridWidth = 50;         // Grid genişliği
    public int gridHeight = 50;        // Grid yüksekliği
    public float cellSize = 1f;        // Hücre boyutu
    public int renderDistance = 10;    // Oyuncunun görebileceği mesafe
    public Transform player;           // Oyuncu objesi
    public GameObject[,] gridCells;    // Tüm hücreler
    private float updateInterval = 0.1f; // Her 0.1 saniyede bir güncelleme
    private float timeSinceLastUpdate = 0f;
    public Crosshair crosshair;

    void Start()
    {
        // Oyuncu objesini bul
        player = GameObject.FindWithTag("Player").transform;

        // Grid oluştur
        CreateGrid();
    }

    void Update()
    {
        // Zamanlayıcı ile güncelleme yap
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateGridVisibility();
            timeSinceLastUpdate = 0f;
        }
    }

    // Prefab'ı hücre boyutuna göre ölçeklendiren fonksiyon
    public void ScalePrefabToCell(GameObject prefab)
    {
        Renderer prefabRenderer = prefab.GetComponent<Renderer>();
        if (prefabRenderer != null)
        {
            Vector3 prefabSize = prefabRenderer.bounds.size;
            Vector3 cellSizeVector = new Vector3(cellSize, prefabSize.y, cellSize);

            float scaleX = cellSizeVector.x / prefabSize.x;
            float scaleZ = cellSizeVector.z / prefabSize.z;

            prefab.transform.localScale = new Vector3(scaleX, prefab.transform.localScale.y, scaleZ);
        }
    }

    // Prefab'ı hücre pozisyonuna yerleştir
    private void AlignPrefabToCell(GameObject prefab, Vector3 cellPosition)
    {
        prefab.transform.position = new Vector3(cellPosition.x, prefab.transform.position.y, cellPosition.z);
    }

    // Grid oluşturma fonksiyonu
    private void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];

        // Grid'in merkezi
        Vector3 gridCenter = player != null ? player.position : Vector3.zero;

        int halfWidth = gridWidth / 2;
        int halfHeight = gridHeight / 2;

        // Grid hücrelerini oluştur
        for (int x = -halfWidth; x < halfWidth; x++)
        {
            for (int z = -halfHeight; z < halfHeight; z++)
            {
                // Hücrenin pozisyonunu hesapla
                Vector3 cellPosition = new Vector3(gridCenter.x + x * cellSize, 0, gridCenter.z + z * cellSize);

                // Hücre oluştur ve scale ayarla
                GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);

                gridCells[x + halfWidth, z + halfHeight] = newCell;

                ScalePrefabToCell(newCell);
                AlignPrefabToCell(newCell, cellPosition);
            }
        }

        Debug.Log($"{gridWidth * gridHeight} hücre oluşturuldu!");
    }

    // Grid görünürlüğünü güncelleme
    private void UpdateGridVisibility()
    {
        if (player == null) return;

        int halfWidth = gridWidth / 2;
        int halfHeight = gridHeight / 2;

        // Görünürlük ve collider yönetimi
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GameObject cell = gridCells[x, z];
                Vector3 cellPosition = cell.transform.position;

                // Oyuncuya olan mesafeyi hesapla
                float distance = Vector3.Distance(player.position, cellPosition);

                // Eğer mesafe renderDistance'tan küçükse
                if (distance <= renderDistance * cellSize)
                {
                    cell.SetActive(true);  // Görünür
                    Collider cellCollider = cell.GetComponent<Collider>();
                    if (cellCollider != null)
                    {
                        cellCollider.enabled = true;  // Collider aktif
                    }
                }
                // Eğer mesafe 5*5 mesafesinden daha uzakta ise
                else if (distance > renderDistance * cellSize && distance <= (renderDistance + 5) * cellSize)
                {
                    cell.SetActive(true);  // Görünür
                    Collider cellCollider = cell.GetComponent<Collider>();
                    if (cellCollider != null)
                    {
                        cellCollider.enabled = false;  // Collider devre dışı
                    }
                }
                else
                {
                    cell.SetActive(false);  // Görünmez
                    Collider cellCollider = cell.GetComponent<Collider>();
                    if (cellCollider != null)
                    {
                        cellCollider.enabled = false;  // Collider devre dışı
                    }
                }
            }
        }
    }
    public void ChangeCell()
    {
        Ray ray = crosshair.playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, crosshair.maxDistance, crosshair.interactableLayer))
        {
            // Tıklanan hücreyi al
            GameObject clickedCell = hit.collider.gameObject;

            // Tıklanan nesnenin layer'ını kontrol et
            if (clickedCell.layer != LayerMask.NameToLayer("ground"))
            {
                Debug.Log("Bu işlem yalnızca 'ground' katmanındaki nesneler için çalışır.");
                return;
            }

            Debug.Log("Tıklanan Hücre: " + clickedCell.name);

            // Eski hücreyi kaldır
            Vector3 cellPosition = clickedCell.transform.position; // Pozisyon bilgisi
            Quaternion cellRotation = clickedCell.transform.rotation; // Rotasyon bilgisi
            Destroy(clickedCell); // Mevcut hücreyi yok et

            // Yeni prefab oluştur
            GameObject newCell = Instantiate(crosshair.replacementPrefab, cellPosition, cellRotation);

            // Yeni prefab'ı hücre boyutuna göre ölçekle ve hizala
            ScalePrefabToCell(newCell);

            Debug.Log("Hücre değiştirildi ve yeniden boyutlandırıldı: " + newCell.name);
        }
    }
}
