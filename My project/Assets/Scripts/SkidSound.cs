using UnityEngine;

public class WheelSkidSound : MonoBehaviour
{
    [Header("Настройки колес")]
    public WheelCollider[] wheelColliders; // Перетащите сюда ваши WheelColliders

    [Header("Настройки звука")]
    public AudioSource audioSource;
    public float slipThreshold = 0.4f;     // Порог, после которого начинается звук
    public float fadeSpeed = 5f;          // Скорость изменения громкости
    public float maxVolume = 0.8f;         // Максимальная громкость

    void Update()
    {
        bool isSkidding = false;

        // Проверяем каждое колесо на пробуксовку или занос
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.GetGroundHit(out WheelHit hit))
            {
                // forwardSlip — пробуксовка/блокировка, sidewaysSlip — боковой занос
                if (Mathf.Abs(hit.forwardSlip) > slipThreshold || Mathf.Abs(hit.sidewaysSlip) > slipThreshold)
                {
                    isSkidding = true;
                    break; // Если хоть одно колесо буксует, активируем звук
                }
            }
        }

        // Плавно управляем громкостью звука
        if (isSkidding)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, maxVolume, fadeSpeed * Time.deltaTime);
        }
        else
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, fadeSpeed * Time.deltaTime);
        }
    }
}
