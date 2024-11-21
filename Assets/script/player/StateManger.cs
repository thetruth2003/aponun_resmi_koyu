using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CarController;

public enum gamestate { player, Car}
public class StateManger : MonoBehaviour
{
    public Camera playerCamera; // Oyuncunun kamerası
    public float maxDistance = 100f; // Maksimum atış mesafesi
    public LayerMask interactableLayer; // Etkileşimde bulunulacak katman
    public GameObject player; // Oyuncu karakteri
    public static StateManger Instance;
    public GameObject car;
    public gamestate state;
    private void Awake()
    {
        if (Instance == null)  
            Instance = this; 
        else
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && state == gamestate.player)
        {
            EnterCar();
        }
        // E tuşuna basıldığında aracın kontrolünü al
        else if (Input.GetKeyDown(KeyCode.E) && state == gamestate.Car)
        {
            ExitCar();
        }
    }
    private void ExitCar()
    {
        player.transform.parent = null;
        state = gamestate.player;
        player.SetActive(true);
        car = null;
    }
    private void EnterCar()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            // E tuşuna basıldığında oyuncuyu devre dışı bırak ve araca geç
            if (hit.collider.CompareTag("Car"))
            {
                car = hit.collider.gameObject;
                player.SetActive(false);
                player.transform.parent = car.GetComponent<CarController>().playerpoint;
                player.transform.localPosition = Vector3.zero;
                state = gamestate.Car;

                // Araç türünü belirle
                CarController carController = hit.collider.GetComponent<CarController>();
            }
        }
    }

}
