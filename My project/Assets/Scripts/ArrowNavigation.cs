using UnityEngine;
using UnityEngine.AI;

public class ArrowNavigation : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject warningTextObject;

    [Header("Ссылки на 3D модели стрелки")]
    public GameObject arrowVisualContainer;

    private NavMeshPath path;
    private Transform closestCargo;
    private int roadAreaMask;
    private int roadAreaIndex;
    private bool isCurrentlyOnRoad = true;

    [Header("Настройки")]
    [SerializeField] private float updateTimer = 0.3f;
    private float currentTimer = 0f;
    [SerializeField] private float rotationSpeed = 8f;

    void Start()
    {
        path = new NavMeshPath();

        if (warningTextObject != null) warningTextObject.SetActive(false);

        // Ищем индекс зоны "Road"
        roadAreaIndex = NavMesh.GetAreaFromName("Road");
        if (roadAreaIndex != -1)
        {
            roadAreaMask = 1 << roadAreaIndex;
        }
        else
        {
            Debug.LogError("КРИТИЧЕСКАЯ ОШИБКА: Зона с именем 'Road' не найдена в Window -> AI -> Navigation -> Areas! Перепроверьте имя.");
            roadAreaMask = NavMesh.AllAreas;
        }

        // Сразу проверяем стартовую позицию
        ToggleRoadState(CheckIfPlayerIsOnRoad());
    }

    void Update()
    {
        bool isOnRoad = CheckIfPlayerIsOnRoad();

        if (isOnRoad != isCurrentlyOnRoad)
        {
            ToggleRoadState(isOnRoad);
        }

        if (!isOnRoad) return;

        // ЛОГИКА НАВИГАЦИИ К КОРОБКЕ
        currentTimer += Time.deltaTime;
        if (currentTimer >= updateTimer)
        {
            FindClosestCargo();
            currentTimer = 0f;
        }

        if (closestCargo == null) return;

        NavMesh.CalculatePath(transform.position, closestCargo.position, roadAreaMask, path);

        if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length > 1)
        {
            Vector3 nextPointOnRoad = path.corners[1];

            // Рисуем зеленую линию пути в окне Scene
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.green);
            }

            Vector3 targetDirection = nextPointOnRoad - transform.position;
            targetDirection.y = 0;

            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            Vector3 directDirection = closestCargo.position - transform.position;
            directDirection.y = 0;
            if (directDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * (rotationSpeed * 0.5f));
            }
        }
    }

    void ToggleRoadState(bool isOnRoad)
    {
        isCurrentlyOnRoad = isOnRoad;
        if (arrowVisualContainer != null) arrowVisualContainer.SetActive(isOnRoad);
        if (warningTextObject != null) warningTextObject.SetActive(!isOnRoad);
    }

    bool CheckIfPlayerIsOnRoad()
    {
        // ВАЖНО: Стрелка висит НАД машиной. Чтобы проверить дорогу под КОЛЕСАМИ,
        // мы пускаем луч из позиции стрелки строго вниз на землю.
        Vector3 groundPosition = transform.position;

        RaycastHit rayHit;
        // Пускаем физический луч вниз на 10 метров, чтобы найти настоящую землю под машиной
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, 10f))
        {
            groundPosition = rayHit.point; // Переключаем точку проверки на асфальт
        }

        // Рисуем красный крестик в окне Scene там, где скрипт ищет NavMesh (для диагностики)
        Debug.DrawLine(groundPosition + Vector3.left, groundPosition + Vector3.right, Color.red);
        Debug.DrawLine(groundPosition + Vector3.forward, groundPosition + Vector3.back, Color.red);

        NavMeshHit navHit;
        // Ищем NavMesh в радиусе 3 метров от земли под машиной
        if (NavMesh.SamplePosition(groundPosition, out navHit, 3.0f, NavMesh.AllAreas))
        {
            if (roadAreaIndex != -1 && (navHit.mask & (1 << roadAreaIndex)) != 0)
            {
                return true; // Мы на асфальте зоны Road!
            }
        }
        return false;
    }

    void FindClosestCargo()
    {
        GameObject[] cargos = GameObject.FindGameObjectsWithTag("Cargo");
        if (cargos.Length == 0) { closestCargo = null; return; }

        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (GameObject cargo in cargos)
        {
            if (cargo == null) continue;
            float distance = Vector3.Distance(transform.position, cargo.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = cargo.transform;
            }
        }
        closestCargo = bestTarget;
    }
}
