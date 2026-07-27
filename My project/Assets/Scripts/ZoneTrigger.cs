using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    // Эта переменная будет хранить ссылку на главного менеджера
    [HideInInspector]
    public MissionManager manager;

    private void OnTriggerEnter(Collider other)
    {
        // Если в зону въехал объект с тегом Player
        if (other.CompareTag("Player"))
        {
            // Проверяем, что менеджер существует, и вызываем его команду
            if (manager != null)
            {
                manager.ZoneReached();
            }
            else
            {
                Debug.LogError("Зона не видит менеджера!");
            }
        }
    }
}