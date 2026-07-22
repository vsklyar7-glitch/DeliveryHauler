using UnityEngine;
using TMPro; // Нужен для работы с TextMeshPro. Если используете обычный текст, замените на UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public Rigidbody vehicleRigidbody; // Сюда перетащить МАШИНУ из Hierarchy

    private TextMeshProUGUI speedText; // Ссылка на компонент текста на этом же объекте

    void Start()
    {
        // Автоматически получаем текстовый компонент, который висит вместе со скриптом
        speedText = GetComponent<TextMeshProUGUI>();

        // Если вы используете обычный UI Text (не TextMeshPro), 
        // замените строку выше и объявление переменной на:
        // private Text speedText;
        // speedText = GetComponent<Text>();

        if (vehicleRigidbody == null)
        {
            Debug.LogError("Спидометр: Не назначена ссылка на Rigidbody машины в Инспекторе!");
        }
    }

    void Update()
    {
        if (vehicleRigidbody == null || speedText == null) return;

        // 1. Получаем скорость в метрах в секунду (m/s) из физического тела машины.
        // Переменная linearVelocity в Unity 6 отвечает за текущую скорость по осям.
        // .magnitude возвращает общую длину этого вектора (чистую скорость).
        float speedInMetersPerSecond = vehicleRigidbody.linearVelocity.magnitude;

        // В старых версиях Unity использовалось: vehicleRigidbody.velocity.magnitude;
        // В Unity 6 правильно использовать: vehicleRigidbody.linearVelocity.magnitude;

        // 2. Переводим метры в секунду в Километры в час (умножаем на 3.6)
        float speedKmH = speedInMetersPerSecond * 3.6f;

        // Если вам нужны мили в час, используйте: float speedMph = speedInMetersPerSecond * 2.237f;

        // 3. Округляем до целого числа, чтобы цифры не мельтешили долями
        int finalSpeed = Mathf.RoundToInt(speedKmH);

        // 4. Выводим текст на экран
        speedText.text = "Скорость: " + finalSpeed.ToString() + " км/ч";
    }
}
