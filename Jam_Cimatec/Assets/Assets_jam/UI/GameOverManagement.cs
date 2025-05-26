using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject hudPanel;

    // guarda metade do cash entre recarregamentos
    public static int SavedCash = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Chame quando o Core morrer.
    /// </summary>
    public void ShowGameOver(int currentCash)
    {
        // pausa
        Time.timeScale = 0f;

        // guarda metade do cash
        SavedCash = currentCash / 2;

        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
    }

    /// <summary>
    /// Botão Retry: recarrega a cena atual.
    /// </summary>
    public void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Botão Main Menu: recarrega a cena de menu (por nome ou buildIndex).
    /// </summary>
    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // ou o nome/index da sua cena de menu
    }
}
