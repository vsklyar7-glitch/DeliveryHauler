using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Окна UI")]
    public GameObject pauseMenuPanel;    // Главный родитель всего Canvas Pause/Menu
    public GameObject mainMenuWindow;    // Объект MainMenuWindow (Кнопки: Начать, Настройки, Выход)
    public GameObject settingsWindow;    // Объект SettingsWindow
    public GameObject gameplayHUD;       // Приборная панель Dashboard

    [Header("Ссылки на компоненты машины")]
    public PickupController carController;

    [Header("Элементы настроек (Toggles)")]
    public Toggle absToggle;
    public Toggle autoTransmissionToggle;
    public Toggle hudToggle;

    private bool isPaused = false;
    private bool isInMainMenu = true; // Флаг: находится ли игрок в самом Главном Меню при старте

    void Start()
    {
        // ПРИ СТАРТЕ ИГРЫ: Включаем меню и ставим мир на паузу
        Time.timeScale = 0f;
        isInMainMenu = true;
        isPaused = true;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);
        if (gameplayHUD != null) gameplayHUD.SetActive(false); // Прячем спидометр в меню

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
        // Кнопка Escape не должна работать, если игрок находится в Главном Меню при старте
        if (Input.GetKeyDown(KeyCode.Escape) && !isInMainMenu)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // Кнопка "Начать игру" (вызывается из Главного Меню)
    public void StartGame()
    {
        isInMainMenu = false;
        isPaused = false;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameplayHUD != null && (hudToggle == null || hudToggle.isOn)) gameplayHUD.SetActive(true); // Включаем HUD машины

        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

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

    public void OpenSettings()
    {
        if (mainMenuWindow != null) mainMenuWindow.SetActive(false);
        if (settingsWindow != null) settingsWindow.SetActive(true);
    }

    public void CloseSettings()
    {
        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ⚡ КНОПКА "В ГЛАВНОЕ МЕНЮ" (вызывается во время паузы посреди игры)
    public void GoToMainMenu()
    {
        isInMainMenu = true;
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);
        if (gameplayHUD != null) gameplayHUD.SetActive(false); // Отключаем HUD
    }

    // ⚡ КНОПКА "ВЫЙТИ" (на рабочий стол)
    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit(); // Работает в скомпилированной игре
    }

    // --- ФУНКЦИИ НАСТРОЕК ---
    private void SetABS(bool value) { if (carController != null) carController.useABS = value; }
    private void SetTransmission(bool value) { if (carController != null) carController.isAutomatic = value; }
    private void SetHUDVisibility(bool value) { if (gameplayHUD != null) gameplayHUD.SetActive(value); }
}
