using UnityEngine;

public class DynamicGridManager : MonoBehaviour
{
    public GameObject gridCellPrefab; // Hücre prefab'ı
    public int gridWidth = 50; // Grid genişliği
    public int gridHeight = 50; // Grid yüksekliği
    public float cellSize = 2.5f; // Hücre boyutu
    public int renderDistance = 10; // Oyuncunun görebileceği mesafe
    public Transform player; // Oyuncu objesi
    public GameObject[,] gridCells; // Tüm hücreler
    public float updateInterval = 0.1f; // Güncelleme aralığı
    public float timeSinceLastUpdate = 0f;

    public Crosshair crosshair; // Çarpı işareti (Nişangah) referansı

    void Start()
    {
        if (!gridCellPrefab)
        {
            Debug.LogError("Grid hücre prefab'ı atanmadı!");
            return;
        }

        if (!player)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (!player)
            {
                Debug.LogError("Oyuncu objesi bulunamadı! Lütfen bir 'Player' objesi ekleyin ve tag atayın.");
                return;
            }
        }

        CreateGrid();
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateGridVisibility();
            timeSinceLastUpdate = 0f;
        }

        if (Input.GetMouseButtonDown(0)) // Sol tık ile hücre değişimi
        {
            ChangeCell();
        }
    }

    // Prefab'ı hücre boyutuna göre ölçeklendiren fonksiyon
    private void ScalePrefabToCell(GameObject prefab)
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

    private void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];
        Vector3 gridOrigin = player.position - new Vector3((gridWidth / 2) * cellSize, 0, (gridHeight / 2) * cellSize);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 cellPosition = new Vector3(gridOrigin.x + x * cellSize,1, gridOrigin.z + z * cellSize);

                // Raycast ile yüzey yüksekliği ve normal alınır
                if (Physics.Raycast(cellPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20f))
                {
                    cellPosition.y = hit.point.y;
                }

                GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);

                // Hücreyi yüzeye hizalama
                AlignToSurface(newCell, hit);
                ScalePrefabToCell(newCell);
                gridCells[x, z] = newCell;
            }
        }

        Debug.Log($"{gridWidth * gridHeight} hücre oluşturuldu!");
    }


    // Hücreyi yüzeye hizalayan fonksiyon
    private void AlignToSurface(GameObject cell, RaycastHit hit)
    {
        if (hit.collider != null)
        {
            cell.transform.position = hit.point;
            cell.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }


    // Grid görünürlüğünü güncelleme
    private void UpdateGridVisibility()
    {
        if (!player) return;

        int halfWidth = gridWidth / 2;
        int halfHeight = gridHeight / 2;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GameObject cell = gridCells[x, z];
                if (!cell) continue;

                float distance = Vector3.Distance(player.position, cell.transform.position);

                if (distance <= renderDistance * cellSize)
                {
                    cell.SetActive(true);
                }
                else
                {
                    cell.SetActive(false);
                }
            }
        }
    }

    // Hücre değiştirme işlemi
    public void ChangeCell()
    {
        // Çarpı işareti üzerinden bir ray gönder
        Ray ray = crosshair.playerCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast ile hedef yüzeye çarpma kontrolü
        if (Physics.Raycast(ray, out RaycastHit hit, crosshair.maxDistance, crosshair.interactableLayer))
        {
            GameObject clickedCell = hit.collider.gameObject;

            // Hücrenin katmanını kontrol et
            if (clickedCell.layer != LayerMask.NameToLayer("ground"))
            {
                Debug.Log("Sadece 'ground' katmanındaki nesneler için geçerlidir.");
                return;
            }

            // Tıklanan hücrenin pozisyon ve ölçek bilgilerini al
            Vector3 cellPosition = clickedCell.transform.position;
            Quaternion cellRotation = clickedCell.transform.rotation;
            Vector3 cellScale = clickedCell.transform.localScale;

            // Eski hücreyi yok et
            Destroy(clickedCell);

            // Yeni hücreyi oluştur
            GameObject newCell = Instantiate(crosshair.replacementPrefab, cellPosition, cellRotation);

            // Yeni hücrenin ölçeğini ayarla
            newCell.transform.localScale = cellScale;

            Debug.Log("Hücre başarıyla değiştirildi.");
        }
    }

}