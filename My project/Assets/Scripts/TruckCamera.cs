using UnityEngine;

public class FixedCameraController : MonoBehaviour
{
    [Header("Объект для слежения")]
    public Transform target; // Сюда перетащи объект машины (Car)

    [Header("Позиция камеры относительно машины")]
    // X = смещение вбок, Y = высота, Z = дистанция сзади (должна быть с минусом)
    public Vector3 offset = new Vector3(0f, 2.5f, -6f);

    [Header("Настройки плавности")]
    public float positionSmoothTime = 0.1f; // Плавность следования за позицией
    public float rotationSmoothTime = 0.05f; // Плавность вращения вслед за машиной

    private Vector3 positionVelocity;
    private float rotationVelocity;

    void LateUpdate()
    {
        if (!target) return;

        // 1. Вычисляем идеальную позицию камеры с учетом текущего поворота машины
        // Метод TransformPoint переводит локальное смещение (offset) в глобальные координаты
        Vector3 targetPosition = target.TransformPoint(offset);

        // Плавно перемещаем камеру к этой позиции
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, positionSmoothTime);

        // 2. Вычисляем угол поворота машины по оси Y (куда она смотрит)
        float targetRotationAngle = target.eulerAngles.y;
        float currentRotationAngle = transform.eulerAngles.y;

        // Плавно сглаживаем переход угла поворота камеры к углу машины
        currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, targetRotationAngle, ref rotationVelocity, rotationSmoothTime);

        // 3. Направляем взгляд камеры
        // Камера сохраняет наклон по X (чтобы смотреть чуть вниз) и плавно поворачивается по Y вслед за машиной
        transform.rotation = Quaternion.Euler(20f, currentRotationAngle, 0f);
    }
}