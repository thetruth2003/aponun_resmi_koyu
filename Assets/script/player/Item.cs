using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Item : MonoBehaviour
{
    public ItemData data;

    [HideInInspector]
    public Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Başlangıçta kinematik olarak ayarlanır
    }

    public void EnablePhysics()
    {
        rb.isKinematic = false; // Fiziksel kuvvetleri etkinleştir
    }
}
