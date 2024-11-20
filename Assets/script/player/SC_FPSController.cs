using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 3.0f;  // Yürüme hızı
    public float runningSpeed = 6.0f;  // Koşma hızı
    public float jumpSpeed = 8.0f;     // Zıplama hızı
    public float gravity = 20.0f;      // Yerçekimi kuvveti
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // İleri/geri hareket için yön hesapla
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Koşma için tuş kontrolü
        bool isRunning = Input.GetKey(KeyCode.LeftShift);  // Shift'e basınca koşacak

        // Yönlere bağlı hareket hızı
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;

        // Yükseklik hareketini (zıplama) koru
        float movementDirectionY = moveDirection.y;

        // Yere temas ettiğinde hareket yönünü güncelle
        if (characterController.isGrounded)
        {
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            // Zıplama tuşuna basıldıysa yukarı doğru hız ver
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else
        {
            // Havadayken yerçekimini uygula
            moveDirection.y = movementDirectionY - (gravity * Time.deltaTime);
        }

        // Karakteri hareket ettir
        characterController.Move(moveDirection * Time.deltaTime);

        // Speed'i hesapla
        float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

        // Speed parametresini ve koşma durumunu animator'a gönder
        animator.SetFloat("Speed", speed);
        animator.SetBool("isRunning", isRunning);

        // Player ve Kamera dönüşlerini kontrol et
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
