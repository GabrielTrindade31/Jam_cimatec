using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Panels")]
    [Tooltip("Painel principal com o botão Start")]
    [SerializeField] private GameObject menuPanel;
    [Tooltip("Painel de opções")]
    [SerializeField] private GameObject optionsPanel;
    [Tooltip("Painel de créditos")]
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject hudPanel;

    void Awake()
    {
        // Na cena inicial, só o menu principal está visível e o jogo fica pausado.
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        hudPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Ligado ao botão START.
    /// Esconde todos os menus e inicia o jogo.
    /// </summary>
    public void OnStartButtonPressed()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        hudPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Ligado ao botão OPTIONS no menu principal.
    /// </summary>
    public void OnOptionsButtonPressed()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Ligado ao botão CREDITS no menu principal.
    /// </summary>
    public void OnCreditsButtonPressed()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Ligado ao botão BACK dentro de Options ou Credits.
    /// Volta para o menu principal.
    /// </summary>
    public void OnBackToMainButtonPressed()
    {
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    /// <summary>
    /// (Opcional) Ligado ao botão QUIT no menu principal.
    /// </summary>
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    public void ShowStart()
    {
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        Time.timeScale = 0f;
    }
}
