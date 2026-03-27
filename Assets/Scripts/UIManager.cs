using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    // UI references
    [SerializeField] private TMPro.TMP_Text scoreText;
    [SerializeField] private TMPro.TMP_Text livesText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMPro.TMP_Text powerupMessageText; // Pour afficher les messages de powerup
    [SerializeField] private TMPro.TMP_Text timeText; // Pour afficher le temps �coul�



    private TMPro.TMP_Text countdownText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.getInstance();

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (powerupMessageText) powerupMessageText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        // Affichage du temps de jeu (optionnel)
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(gameManager.getGameTime() / 60);
            int seconds = Mathf.FloorToInt(gameManager.getGameTime() % 60);
            timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        // Mise à jour des textes de score et de vies
        if (scoreText != null)
        {
            scoreText.text = "Score: " + gameManager.getScore();
        }

        if (livesText != null)
        {
            livesText.text = "Lives: " + gameManager.getLives();
        }
    }

    // Coroutine pour afficher un message temporaire
    public IEnumerator ShowPowerupMessage(string message)
    {
        if (powerupMessageText != null)
        {
            powerupMessageText.text = message;
            powerupMessageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            powerupMessageText.gameObject.SetActive(false);
        }
        yield return null;
    }
}
