using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Transform centerOfMassObject; // Создай пустой объект внизу грузовика и перетащи сюда
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (centerOfMassObject != null)
        {
            rb.centerOfMass = centerOfMassObject.localPosition;
        }
    }
}