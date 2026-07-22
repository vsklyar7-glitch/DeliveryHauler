using UnityEngine;
using System.Collections;

public class CargoManager : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public GameObject cargoPrefab;
    public Transform cargoSpawnPoint;

    [Tooltip("Перетащи сюда объект DropoffVisual (дочерний объект зоны выгрузки)")]
    public GameObject dropoffVisual;

    [Header("Настройки утери груза")]
    public float lossDistance = 2.5f;

    [Header("Защита от багов")]
    public float spawnCooldown = 2f;

    private GameObject currentCargo;
    private FixedJoint temporaryJoint;
    private bool isCooldown = false;
    private Rigidbody truckRigidbody;

    private void Start()
    {
        truckRigidbody = GetComponent<Rigidbody>();

       
        if (dropoffVisual != null)
        {
            dropoffVisual.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup") && currentCargo == null && !isCooldown)
        {
            StartCoroutine(SpawnCargoRoutine());
        }
        else if (other.CompareTag("Dropoff") && currentCargo != null)
        {
            DeliverCargo();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup") && currentCargo != null)
        {
            if (temporaryJoint != null)
            {
                Destroy(temporaryJoint);
                Debug.Log("⚠️  Вы покинули базу! Крепления сняты.");
            }
        }
    }

    private IEnumerator SpawnCargoRoutine()
    {
        isCooldown = true;

        currentCargo = Instantiate(cargoPrefab, cargoSpawnPoint.position, cargoSpawnPoint.rotation);

        Rigidbody cargoRb = currentCargo.GetComponent<Rigidbody>();
        cargoRb.isKinematic = false;

        temporaryJoint = currentCargo.AddComponent<FixedJoint>();
        temporaryJoint.connectedBody = truckRigidbody;
        temporaryJoint.breakForce = Mathf.Infinity;
        temporaryJoint.breakTorque = Mathf.Infinity;
        temporaryJoint.enableCollision = true;

        // ВАЖНО: Включаем подсветку зоны выгрузки, так как груз взят!
        if (dropoffVisual != null)
        {
            dropoffVisual.SetActive(true);
        }

        Debug.Log("📦 Груз загружен. Зона выгрузки подсвечена!");

        yield return new WaitForSeconds(spawnCooldown);
        isCooldown = false;
    }

    private void Update()
    {
        if (currentCargo == null) return;

        float currentDistance = Vector3.Distance(cargoSpawnPoint.position, currentCargo.transform.position);

        if (currentDistance > lossDistance)
        {
            LoseCargo();
        }
    }

    private void LoseCargo()
    {
        Debug.Log("Груз выпал!");
        currentCargo = null;

        // ВАЖНО: Выключаем подсветку зоны выгрузки, ведь везти больше нечего
        if (dropoffVisual != null)
        {
            dropoffVisual.SetActive(false);
        }
    }

    private void DeliverCargo()
    {
        Destroy(currentCargo);
        currentCargo = null;

        // ВАЖНО: Выключаем подсветку после успешной доставки
        if (dropoffVisual != null)
        {
            dropoffVisual.SetActive(false);
        }

        Debug.Log("Груз доставлен!");
    }
}