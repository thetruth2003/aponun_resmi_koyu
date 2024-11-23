using UnityEngine;

public class DynamicGridManager : MonoBehaviour
{
    public GameObject gridCellPrefab;  // Hücre prefab'ı (örneğin toprak modeliniz)
    public int gridWidth = 100;        // Grid'in genişliği
    public int gridHeight = 100;       // Grid'in yüksekliği
    public float cellSize = 1f;        // Hücre boyutu
    public int renderDistance = 10;    // Oyuncunun görebileceği mesafe (hücre birimi)
    public Transform player;           // Oyuncu objesi
    public GameObject[,] gridCells;   // Tüm hücreler

    void Start()
    {
        // Oyuncu objesini bul
        player = GameObject.FindWithTag("Player").transform;

        // Grid oluştur
        CreateGrid();
    }

    void Update()
    {
        // Grid'in görünürlüğünü oyuncunun konumuna göre güncelle
        UpdateGridVisibility();
    }

    // Prefab'ı hücre boyutuna göre ölçeklendiren fonksiyon
    private void ScalePrefabToCell(GameObject prefab)
    {
        Renderer prefabRenderer = prefab.GetComponent<Renderer>();
        if (prefabRenderer != null)
        {
            Vector3 prefabSize = prefabRenderer.bounds.size; // Mevcut prefab boyutları
            Vector3 cellSizeVector = new Vector3(cellSize, prefabSize.y, cellSize); // Y ekseni orijinal kalıyor

            // Prefab'ı yalnızca X ve Z eksenlerinde ölçeklendir
            float scaleX = cellSizeVector.x / prefabSize.x;
            float scaleZ = cellSizeVector.z / prefabSize.z;

            prefab.transform.localScale = new Vector3(scaleX, prefab.transform.localScale.y, scaleZ);
        }
    }


    // Prefab'ı hücre pozisyonuna yerleştir
    private void AlignPrefabToCell(GameObject prefab, Vector3 cellPosition)
    {
        // Yükseklik sadece 0 olarak ayarlanabilir, çünkü prefab'ın y pozisyonu yükseklik ile belirlenecek
        prefab.transform.position = new Vector3(cellPosition.x, prefab.transform.position.y, cellPosition.z);
    }

    // Grid'i oluşturma fonksiyonu
    private void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];  // Grid hücrelerini tutacak 2D dizi

        // Tüm hücreleri oluştur
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Hücre pozisyonunu hesapla
                Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);

                // Prefab'dan yeni hücre oluştur
                GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);

                // Hücreyi 2D dizisine kaydet
                gridCells[x, z] = newCell;

                // Hücrenin adını belirle
                newCell.name = $"GridCell_{x}_{z}";

                // Prefab'ı hücre boyutuna göre ölçeklendir
                ScalePrefabToCell(newCell);

                // Prefab'ı hücre pozisyonuna yerleştir
                AlignPrefabToCell(newCell, cellPosition);

                // Hücreye renk ver (görsel kontrol)
                Renderer renderer = newCell.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(Random.value, Random.value, Random.value); // Renk rastgele
                }
            }
        }

        Debug.Log($"{gridWidth * gridHeight} hücre oluşturuldu!");
    }

    // Grid'in görünürlüğünü oyuncunun konumuna göre güncelleme fonksiyonu
    private void UpdateGridVisibility()
    {
        if (player == null) return;

        // Tüm hücreleri kontrol et
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GameObject cell = gridCells[x, z];

                // Oyuncuya olan mesafeyi hesapla
                float distance = Vector3.Distance(player.position, cell.transform.position);

                // Hücreyi aktif/gizle
                cell.SetActive(distance <= renderDistance * cellSize);
            }
        }
    }
}
