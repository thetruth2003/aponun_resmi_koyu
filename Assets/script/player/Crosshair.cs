using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Camera playerCamera; // Oyuncunun kamerası
    public float maxDistance = 100f; // Maksimum atış mesafesi
    public LayerMask interactableLayer; // Etkileşimde bulunulacak katman
    public GameObject player; // Oyuncu karakteri

    private void Update()
    {
        // Fare tıklaması ile etkileşime gir
        if (Input.GetMouseButtonDown(0))
        {
            ShootRay();
        }
    }

    private void ShootRay()
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
 
}


