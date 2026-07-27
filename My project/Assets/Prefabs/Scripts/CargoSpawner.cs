using UnityEngine;

public class CargoSpawner : MonoBehaviour
{
    [Header("Настройки груза")]
    public GameObject cargoPrefab;      // Префаб коробки (из папки Project)
    public int cargoToSpawnCount = 5;    // Сколько коробок нужно создать за раз

    [Header("Точки появления коробок")]
    public Transform[] cargoSpawnPoints; // Список точек, где могут быть коробки

    void Start()
    {
        SpawnCargo();
    }

    public void SpawnCargo()
    {
        // Проверки на ошибки заполнения в Инспекторе
        if (cargoPrefab == null)
        {
            Debug.LogError("Не назначен префаб коробки (Cargo Prefab)!");
            return;
        }
        if (cargoSpawnPoints == null || cargoSpawnPoints.Length == 0)
        {
            Debug.LogError("Список точек для коробок пуст!");
            return;
        }

        // Создаем копию списка точек, чтобы не спавнить две коробки в одно место
        var availablePoints = new System.Collections.Generic.List<Transform>(cargoSpawnPoints);

        // Ограничиваем количество, если точек меньше, чем заказано коробок
        int finalCount = Mathf.Min(cargoToSpawnCount, availablePoints.Count);

        for (int i = 0; i < finalCount; i++)
        {
            // Выбираем случайный индекс из оставшихся доступных точек
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];

            // Спавним коробку в этой точке
            Instantiate(cargoPrefab, selectedPoint.position, selectedPoint.rotation);

            // Удаляем эту точку из списка доступных, чтобы следующая коробка туда не встала
            availablePoints.UniqueRemoveAt(randomIndex);
        }
    }
}

// Маленький хелпер для безопасного удаления из списка (допишите ниже или используйте обычный RemoveAt)
public static class ListExtensions
{
    public static void UniqueRemoveAt<T>(this System.Collections.Generic.List<T> list, int index)
    {
        if (index >= 0 && index < list.Count)
        {
            list.RemoveAt(index);
        }
    }
}

