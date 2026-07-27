using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Окна UI")]
    public GameObject pauseMenuPanel;
    public GameObject mainMenuWindow;
    public GameObject settingsWindow;
    public GameObject gameplayHUD;

    [Header("Настройки прозрачности фона")]
    [Range(0f, 1f)] public float pauseAlpha = 0.5f; // Прозрачность фона во время паузы (настраивается в инспекторе)
    private Image bgImage; // Компонент Image фоновой панели

    [Header("Кнопки Главного Меню / Паузы")]
    public GameObject startGameButton;
    public GameObject resumeButton;
    public GameObject restartButton;

    [Header("Ссылки на компоненты машины")]
    public PickupController carController;

    [Header("Элементы настроек (Toggles)")]
    public Toggle absToggle;
    public Toggle autoTransmissionToggle;
    public Toggle hudToggle;

    private bool isPaused = false;
    private bool isInMainMenu = true;

    private const string ABS_KEY = "Setting_ABS";
    private const string TRANSMISSION_KEY = "Setting_Transmission";
    private const string HUD_KEY = "Setting_HUD";

    void Start()
    {
        Time.timeScale = 0f;
        isInMainMenu = true;
        isPaused = true;

        // Получаем компонент Image с панели фона
        if (pauseMenuPanel != null)
        {
            bgImage = pauseMenuPanel.GetComponent<Image>();
            pauseMenuPanel.SetActive(true);
        }

        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);
        if (gameplayHUD != null) gameplayHUD.SetActive(false);

        // При старте ставим полную непрозрачность фона и обновляем кнопки
        SetBackgroundAlpha(1f);
        UpdateMenuButtons();

        // Загрузка настроек
        LoadAndApplySettings();

        // Слушатели для Toggles
        if (absToggle != null) absToggle.onValueChanged.AddListener(SetABS);
        if (autoTransmissionToggle != null) autoTransmissionToggle.onValueChanged.AddListener(SetTransmission);
        if (hudToggle != null) hudToggle.onValueChanged.AddListener(SetHUDVisibility);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isInMainMenu)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void StartGame()
    {
        isInMainMenu = false;
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameplayHUD != null && (hudToggle == null || hudToggle.isOn)) gameplayHUD.SetActive(true);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);

        // Включаем настроенную полупрозрачность для паузы
        SetBackgroundAlpha(pauseAlpha);
        UpdateMenuButtons();

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

    public void GoToMainMenu()
    {
        isInMainMenu = true;
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (mainMenuWindow != null) mainMenuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);
        if (gameplayHUD != null) gameplayHUD.SetActive(false);

        // При возврате в главное меню снова делаем фон непрозрачным
        SetBackgroundAlpha(1f);
        UpdateMenuButtons();
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }

    // Метод изменения прозрачности (Alpha-канала) цвета фона
    private void SetBackgroundAlpha(float alphaValue)
    {
        if (bgImage != null)
        {
            Color currentColor = bgImage.color;
            currentColor.a = alphaValue; // Меняем только параметр прозрачности (от 0 до 1)
            bgImage.color = currentColor;
        }
    }

    private void UpdateMenuButtons()
    {
        if (isInMainMenu)
        {
            if (startGameButton != null) startGameButton.SetActive(true);
            if (resumeButton != null) resumeButton.SetActive(false);
            if (restartButton != null) restartButton.SetActive(false);
        }
        else
        {
            if (startGameButton != null) startGameButton.SetActive(false);
            if (resumeButton != null) resumeButton.SetActive(true);
            if (restartButton != null) restartButton.SetActive(true);
        }
    }

    // --- ФУНКЦИИ НАСТРОЕК С СОХРАНЕНИЕМ --- 
    private void LoadAndApplySettings()
    {
        bool absValue = PlayerPrefs.GetInt(ABS_KEY, 1) == 1;
        if (carController != null) carController.useABS = absValue;
        if (absToggle != null) absToggle.isOn = absValue;

        bool transValue = PlayerPrefs.GetInt(TRANSMISSION_KEY, 1) == 1;
        if (carController != null) carController.isAutomatic = transValue;
        if (autoTransmissionToggle != null) autoTransmissionToggle.isOn = transValue;

        bool hudValue = PlayerPrefs.GetInt(HUD_KEY, 1) == 1;
        if (gameplayHUD != null) gameplayHUD.SetActive(hudValue);
        if (hudToggle != null) hudToggle.isOn = hudValue;
    }

    private void SetABS(bool value)
    {
        if (carController != null) carController.useABS = value;
        PlayerPrefs.SetInt(ABS_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetTransmission(bool value)
    {
        if (carController != null) carController.isAutomatic = value;
        PlayerPrefs.SetInt(TRANSMISSION_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetHUDVisibility(bool value)
    {
        if (gameplayHUD != null) gameplayHUD.SetActive(value);
        PlayerPrefs.SetInt(HUD_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
