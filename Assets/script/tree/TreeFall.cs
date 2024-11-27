using UnityEngine;
using System.Collections; // IEnumerator kullanabilmek için gerekli namespace

public class TreeFall : MonoBehaviour
{
    public Rigidbody treeRigidbody;
    public bool isFalling = false; // Ağaç zaten devriliyorsa birden fazla işlem yapılmasın
    public float shakeForce = 5f;  // Sallanma kuvveti
    public float fallForce = 500f; // Devrilme kuvveti
    public float shakeDuration = 1f; // Sallanma süresi
    void Start()
    {
        // Ağacın Rigidbody bileşenini al
        treeRigidbody = GetComponent<Rigidbody>();
        treeRigidbody.isKinematic = true; // Fizik başlangıçta devre dışı
    }

    public IEnumerator ShakeAndFall()
    {
        isFalling = true;
        treeRigidbody.isKinematic = false; // Fizik motorunu aktif et 

        float elapsedTime = 0f;

        // Sallanma hareketi
        while (elapsedTime < shakeDuration)
        {
            treeRigidbody.AddTorque(Vector3.forward * shakeForce, ForceMode.Impulse); // Bir yöne dönme kuvveti uygula
            yield return new WaitForSeconds(0.1f); // Kısa bekleme
            treeRigidbody.AddTorque(Vector3.back * shakeForce, ForceMode.Impulse); // Diğer yöne dönme kuvveti uygula
            elapsedTime += 0.1f;
        }

        // Devrilme kuvveti uygula (örneğin sağa doğru devrilme)
        treeRigidbody.AddForce(Vector3.right * fallForce, ForceMode.Impulse);

        yield return null;
    }

}
