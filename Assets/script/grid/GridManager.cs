using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;       // Grid'in genişliği
    public int gridHeight = 10;     // Grid'in yüksekliği
    public float cellSize = 1f;     // Hücre boyutu (1x1 kareler için)
    public GameObject gridCellPrefab; // Grid hücrelerinin prefab'ı
    public int renderDistance = 10; // Oyuncu çevresinde kaç hücre oluşturulacak

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        Vector3 playerPosition = Vector3.zero; // Oyuncunun pozisyonunu al (örn. transform.position)

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector3 cellPosition = playerPosition + new Vector3(x * cellSize, 0, z * cellSize);
                GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                newCell.name = $"GridCell_{x}_{z}";
            }
        }
    }

}
