using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public float motorForce = 1500f;  // İvmelenme gücü
    public float brakeForce = 3000f;  // Frenleme gücü
    public float maxSpeed = 100f;     // Maksimum hız
    public float turnSpeed = 5f;      // Dönüş hızı
    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private float currentSpeed;
    public Transform playerpoint;
    private bool isInVehicle = false; // Player'ın araçta olup olmadığını kontrol eder.
    public GameObject camera;


    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Rigidbody referansını al
        rb.drag = 2f;  // Aracın sürüklenmesini kontrol et

    }

    void Update()
    {
        if (StateManger.Instance.state == gamestate.Car && StateManger.Instance.car == gameObject)
        {
            GetInput();
            HandleMovement();
            HandleSteering();
            camera.SetActive(true);

        }
        else if (StateManger.Instance.car != gameObject) 
        { 
            camera.SetActive(false);    
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void HandleMovement()
    {
        // İvmelenme (motor gücü) uygulama
        currentSpeed = rb.velocity.magnitude;

        if (verticalInput > 0 && currentSpeed < maxSpeed)
        {
            rb.AddForce(transform.forward * verticalInput * motorForce * Time.deltaTime);
        }
        else if (verticalInput < 0 && currentSpeed > 0)
        {
            rb.AddForce(transform.forward * verticalInput * brakeForce * Time.deltaTime); // Frenleme
        }
    }

    private void HandleSteering()
    {
        // Dönüş hareketi (yavaş dönüş için turnSpeed ile kontrol et)
        if (horizontalInput != 0)
        {
            float turnAmount = horizontalInput * turnSpeed * Time.deltaTime;
            transform.Rotate(0, turnAmount, 0);
        }
    }
}
