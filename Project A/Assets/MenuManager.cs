using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject creditsMenuPanel;
    public GameObject garageMenuPanel;
    public GameObject pauseMenuPanel;

    private bool isPlaying = false;
    private bool isPaused = false;
    private GameObject previousPanel;

    void Awake()
    {
        if (startMenuPanel == null) startMenuPanel = FindChildRecursively(transform, "StartMenuPanel")?.gameObject;
        if (optionsMenuPanel == null) optionsMenuPanel = FindChildRecursively(transform, "OptionsMenuPanel")?.gameObject;
        if (creditsMenuPanel == null) creditsMenuPanel = FindChildRecursively(transform, "CreditsMenuPanel")?.gameObject;
        if (garageMenuPanel == null) garageMenuPanel = FindChildRecursively(transform, "GarageMenuPanel")?.gameObject;
        if (pauseMenuPanel == null) pauseMenuPanel = FindChildRecursively(transform, "PauseMenuPanel")?.gameObject;

        WireButton(startMenuPanel, "Play Button", StartGame);
        WireButton(startMenuPanel, "Options Button", OpenOptions);
        WireButton(startMenuPanel, "Garage Button", OpenGarage);
        WireButton(startMenuPanel, "Credits Button", OpenCredits);
        WireButton(startMenuPanel, "Quit Button", QuitGame);

        WireButton(pauseMenuPanel, "Resume Button", ResumeGame);
        WireButton(pauseMenuPanel, "Options Button", OpenOptions);
        WireButton(pauseMenuPanel, "Restart Button", RestartLevel);
        WireButton(pauseMenuPanel, "Main Menu Button", OpenStartMenu);

        AutoWireBackButtons(optionsMenuPanel);
        AutoWireBackButtons(creditsMenuPanel);
        AutoWireBackButtons(garageMenuPanel);

        if (optionsMenuPanel != null)
        {
            Slider slider = optionsMenuPanel.GetComponentInChildren<Slider>(true);
            if (slider != null)
            {
                slider.value = AudioListener.volume;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(val => AudioListener.volume = val);
            }
        }
    }

    void Start()
    {
        OpenStartMenu();
    }

    void Update()
    {
        if (isPlaying)
        {
            bool escPressed = false;
            #if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
                escPressed = UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
            #else
            escPressed = Input.GetKeyDown(KeyCode.Escape);
            #endif

            if (escPressed)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    private void AutoWireBackButtons(GameObject panel)
    {
        if (panel == null) return;
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            string objName = btn.gameObject.name.ToLower();
            TMP_Text tmpText = btn.GetComponentInChildren<TMP_Text>(true);
            Text legacyText = btn.GetComponentInChildren<Text>(true);
            string btnText = tmpText != null ? tmpText.text.ToLower() : (legacyText != null ? legacyText.text.ToLower() : "");

            if (objName.Contains("back") || objName == "button" || btnText.Contains("back"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(BackToPrevious);
            }
        }
    }

    private void WireButton(GameObject parent, string buttonName, UnityEngine.Events.UnityAction action)
    {
        if (parent == null) return;
        Transform target = FindChildRecursively(parent.transform, buttonName);
        if (target != null)
        {
            Button btn = target.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }
    }

    private Transform FindChildRecursively(Transform current, string name)
    {
        if (current.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return current;

        for (int i = 0; i < current.childCount; i++)
        {
            Transform found = FindChildRecursively(current.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }

    public void OpenStartMenu()
    {
        isPlaying = false;
        isPaused = false;
        Time.timeScale = 0f;
        HideAllPanels();
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        previousPanel = startMenuPanel;
    }

    public void StartGame()
    {
        HideAllPanels();
        isPlaying = true;
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        HideAllPanels();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        previousPanel = pauseMenuPanel;
    }

    public void ResumeGame()
    {
        isPaused = false;
        HideAllPanels();
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        HideAllPanels();
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        HideAllPanels();
        if (creditsMenuPanel != null) creditsMenuPanel.SetActive(true);
    }

    public void OpenGarage()
    {
        HideAllPanels();
        if (garageMenuPanel != null) garageMenuPanel.SetActive(true);
    }

    public void BackToPrevious()
    {
        HideAllPanels();
        if (previousPanel != null)
        {
            previousPanel.SetActive(true);
        }
        else
        {
            OpenStartMenu();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void HideAllPanels()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (creditsMenuPanel != null) creditsMenuPanel.SetActive(false);
        if (garageMenuPanel != null) garageMenuPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }
}
