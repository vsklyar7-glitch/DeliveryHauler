using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("Настройки спавна зон")]
    public Transform[] spawnPoints;
    public GameObject zonePrefab;

    [Header("Навигация и Груз")]
    public Transform arrow;
    public GameObject physicsCargoPrefab; // Префаб ФИЗИЧЕСКОГО ящика
    public Transform truckCargoPoint;     // Пустышка CargoPoint в кузове грузовика

    [Header("Цвета зон")]
    public Color loadZoneColor = Color.green;
    public Color unloadZoneColor = Color.yellow;

    [Header("Интерфейс (UI)")]
    public TMP_Text taskText;
    public TMP_Text moneyText;

    private GameObject currentZone;
    private Transform currentTargetPoint;
    private bool hasCargo = false;
    private int score = 0;

    // Сюда мы будем сохранять ссылку на созданный ящик, чтобы следить за ним
    private GameObject activeCargo;

    void Start()
    {
        UpdateUI();
        SpawnNewZone();
    }

    void Update()
    {
        if (currentTargetPoint != null && arrow != null)
        {
            Vector3 direction = currentTargetPoint.position - arrow.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                arrow.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public void ZoneReached()
    {
        if (!hasCargo)
        {
            // --- ПОГРУЗКА ---
            hasCargo = true;

            // Создаем физический ящик в координатах кузова!
            activeCargo = Instantiate(physicsCargoPrefab, truckCargoPoint.position, truckCargoPoint.rotation);

            UpdateUI();
            SpawnNewZone();
        }
        else
        {
            // --- ВЫГРУЗКА ---
            // Сначала проверяем, существует ли ящик (не уничтожился ли он случайно)
            if (activeCargo != null)
            {
                // Измеряем расстояние от кузова до ящика
                float distance = Vector3.Distance(truckCargoPoint.position, activeCargo.transform.position);

                if (distance <= 5f) // Если ящик ближе 5 метров (т.е. он все еще в кузове)
                {
                    hasCargo = false;
                    Destroy(activeCargo); // Забираем груз
                    score += 100;
                    Debug.Log("Успех!");
                }
                else
                {
                    // Груз выпал по дороге! Штрафуем игрока.
                    hasCargo = false;
                    Destroy(activeCargo); // Удаляем валяющийся где-то ящик
                    score -= 50;
                    Debug.Log("Вы приехали пустым! Груз потерян.");
                }
            }

            UpdateUI();
            SpawnNewZone();
        }
    }

    private void SpawnNewZone()
    {
        if (currentZone != null) Destroy(currentZone);

        int randomIndex = Random.Range(0, spawnPoints.Length);
        currentTargetPoint = spawnPoints[randomIndex];

        currentZone = Instantiate(zonePrefab, currentTargetPoint.position, Quaternion.identity);

        ZoneTrigger trigger = currentZone.GetComponent<ZoneTrigger>();
        if (trigger != null) trigger.manager = this;

        Renderer zoneRenderer = currentZone.GetComponent<Renderer>();
        if (zoneRenderer != null)
        {
            zoneRenderer.material.color = hasCargo ? unloadZoneColor : loadZoneColor;
        }
    }

    private void UpdateUI()
    {
        if (taskText != null)
        {
            taskText.text = hasCargo ? "Задача: Довезите груз аккуратно!" : "Задача: Заберите груз на складе!";
        }
        if (moneyText != null)
        {
            moneyText.text = "Баланс: " + score + " $";
        }
    }
}