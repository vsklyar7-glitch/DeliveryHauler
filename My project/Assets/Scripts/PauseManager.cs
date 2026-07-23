using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Окна UI")]
    public GameObject pauseMenuPanel;    // Главный родитель PauseMenu
    public GameObject mainMenuWindow;    // Объект MainMenuWindow
    public GameObject settingsWindow;    // Объект SettingsWindow
    public GameObject gameplayHUD;       // Приборная панель Dashboard

    [Header("Ссылки на компоненты машины")]
    public PickupController carController;

    [Header("Элементы настроек (Toggles)")]
    public Toggle absToggle;
    public Toggle autoTransmissionToggle;
    public Toggle hudToggle;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        // Синхронизация настроек
        if (carController != null)
        {
            if (absToggle != null) absToggle.isOn = carController.useABS;
            if (autoTransmissionToggle != null) autoTransmissionToggle.isOn = carController.isAutomatic;
        }
        if (hudToggle != null && gameplayHUD != null) hudToggle.isOn = gameplayHUD.activeSelf;

        // Слушатели для Toggles
        if (absToggle != null) absToggle.onValueChanged.AddListener(SetABS);
        if (autoTransmissionToggle != null) autoTransmissionToggle.onValueChanged.AddListener(SetTransmission);
        if (hudToggle != null) hudToggle.onValueChanged.AddListener(SetHUDVisibility);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        // Принудительно открываем главное окно паузы и закрываем настройки
        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // ⚡ НОВЫЕ ФУНКЦИИ ПЕРЕКЛЮЧЕНИЯ ОКОН
    public void OpenSettings()
    {
        if (mainMenuWindow != null) mainMenuWindow.SetActive(false); // Прячем главное меню
        if (settingsWindow != null) settingsWindow.SetActive(true);   // Показываем настройки
    }

    public void CloseSettings()
    {
        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);  // Возвращаем главное меню
        if (settingsWindow != null) settingsWindow.SetActive(false); // Прячем настройки
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- ФУНКЦИИ НАСТРОЕК ---
    private void SetABS(bool value) { if (carController != null) carController.useABS = value; }
    private void SetTransmission(bool value) { if (carController != null) carController.isAutomatic = value; }
    private void SetHUDVisibility(bool value) { if (gameplayHUD != null) gameplayHUD.SetActive(value); }
}
