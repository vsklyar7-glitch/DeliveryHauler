using UnityEngine;
using TMPro; // Нужен для TextMeshPro. Если используете обычный текст, замените на UnityEngine.UI;

public class CargoCollection : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public TextMeshProUGUI counterText; // Сюда перетащить CargoCounterText из Hierarchy

    private int totalCargoCount = 0;   // Сколько всего коробок заспавнилось на уровне
    private int collectedCargoCount = 0; // Сколько коробок игрок уже собрал

    void Start()
    {
        // Ждем 0.1 секунды, чтобы CargoSpawnManager успел создать все коробки на сцене
        Invoke(nameof(InitializeCounter), 0.1f);
    }

    void InitializeCounter()
    {
        // Находим, сколько всего коробок с тегом "Cargo" сейчас есть на сцене
        totalCargoCount = GameObject.FindGameObjectsWithTag("Cargo").Length;

        UpdateUI();
    }

    // Этот метод Unity вызывает автоматически, когда машина пересекает триггер коробки
    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что мы столкнулись именно с объектом, у которого тег "Cargo"
        if (other.CompareTag("Cargo"))
        {
            // Прибавляем собранную коробку
            collectedCargoCount++;

            // Обновляем текст на экране
            UpdateUI();

            // Уничтожаем коробку, чтобы она исчезла со сцены
            Destroy(other.gameObject);

            // Проверяем условия победы
            if (collectedCargoCount >= totalCargoCount)
            {
                OnAllCargoCollected();
            }
        }
    }

    void UpdateUI()
    {
        if (counterText != null)
        {
            counterText.text = $"Груз: {collectedCargoCount} / {totalCargoCount}";
        }
    }

    void OnAllCargoCollected()
    {
        Debug.Log("Все коробки собраны. Доставьте их на места назначения.");
        // Здесь в будущем можно включить экран победы или загрузить следующий уровень:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("VictoryScene");
    }
}
