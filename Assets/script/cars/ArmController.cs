using System.Net.Sockets;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public Transform arm; // Kepçenin kolu
    public Transform bucket; // Kepçenin kolu
    public float rotationSpeed = 10f; // Kolun hareket hızı
    public float minRotation = -50f; // X eksenindeki minimum dönüş açısı
    public float maxRotation = 30f;  // X eksenindeki maksimum dönüş açısı
    private float currentRotationX = 0; // Kolun mevcut X rotasyonu

    void Update()
    {
        // "F" tuşuna basıldığında kolu aşağı indir
        if (Input.GetKey(KeyCode.F))
        {
            if (currentRotationX > minRotation)
            {
                currentRotationX -= rotationSpeed * Time.deltaTime; // X rotasyonunu azalt
                SetArmRotation(currentRotationX);
            }
        }

        // "R" tuşuna basıldığında kolu yukarı kaldır
        if (Input.GetKey(KeyCode.R))
        {
            if (currentRotationX < maxRotation)
            {
                currentRotationX += rotationSpeed * Time.deltaTime; // X rotasyonunu artır
                SetArmRotation(currentRotationX);
            }
        }
        // "T" tuşuna basıldığında kolu aşağı indir
        if (Input.GetKey(KeyCode.T))
        {
            if (currentRotationX > minRotation)
            {
                currentRotationX -= rotationSpeed * Time.deltaTime; // X rotasyonunu azalt
                SetBucketRotation(currentRotationX);
            }
        }

        // "G" tuşuna basıldığında kolu yukarı kaldır
        if (Input.GetKey(KeyCode.G))
        {
            if (currentRotationX < maxRotation)
            {
                currentRotationX += rotationSpeed * Time.deltaTime; // X rotasyonunu artır
                SetBucketRotation(currentRotationX);
            }
        }
    }

    // Kolun X rotasyonunu ayarlama
    void SetArmRotation(float rotationX)
    {
        arm.localRotation = Quaternion.Euler(rotationX, arm.localRotation.eulerAngles.y, arm.localRotation.eulerAngles.z);
    }

    // Kolun X rotasyonunu ayarlama
    void SetBucketRotation(float rotationX)
    {
        bucket.localRotation = Quaternion.Euler(rotationX, bucket.localRotation.eulerAngles.y, bucket.localRotation.eulerAngles.z);
    }
}
