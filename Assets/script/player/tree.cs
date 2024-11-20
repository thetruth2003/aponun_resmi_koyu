using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatoin : MonoBehaviour
{
    public float shakeDuration = 0.5f;  // Sarsılma süresi
    public float shakeMagnitude = 0.1f;  // Sarsılma büyüklüğü
    private int hitCount = 0;             // "axe" collideri ile kaç kere çarpıştığını saymak için

    private Vector3 initialPosition;
    private bool isShaking = false;

    private List<apple> apples = new List<apple>(); // Ağaç içindeki elma nesnelerini tutacak liste

    void Start()
    {
        initialPosition = transform.localPosition; // Başlangıç pozisyonunu kaydet

        // Bu ağaç prefab'ının içindeki tüm elma nesnelerini bul
        apples.AddRange(GetComponentsInChildren<apple>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarpan obje "axe" değilse çık
        if (!collision.gameObject.CompareTag("axe")) return;

        // Çarpışma olduğunda sarsılma başlasın
        if (!isShaking)
        {
            StartCoroutine(Shake());
            hitCount++;

            // hitCount 3 veya daha fazla olduğunda
            if (hitCount >= 3)
            {
                DropAllApples(); // Sadece bu ağacın içindeki elma nesnelerini düşür
                hitCount = 0; // Hit sayacını sıfırla
            }
        }
    }

    IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = new Vector3(initialPosition.x + x, initialPosition.y + y, initialPosition.z);

            elapsed += Time.deltaTime;

            yield return null; // Bir frame bekle
        }

        transform.localPosition = initialPosition; // Sarsılma bitince eski pozisyona dön
        isShaking = false;
    }

    private void DropAllApples()
    {
        // Ağaç içindeki elma nesnelerini bul
        foreach (apple apple in apples)
        {
            if (apple != null)
            {
                apple.Drop(); // Düşür
            }
            else
            {
                Debug.LogError("Apple script is missing on an apple object inside the tree!");
            }
        }
    }
}
