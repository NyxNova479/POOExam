using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    // UI references
    private TMPro.TMP_Text scoreText;
    private TMPro.TMP_Text livesText;
    private GameObject gameOverPanel;
    private TMPro.TMP_Text powerupMessageText; // Pour afficher les messages de powerup
    private TMPro.TMP_Text timeText; // Pour afficher le temps �coul�
    private GameObject playerDamageEffect; // Effet visuel quand un ennemi traverse


    private TMPro.TMP_Text countdownText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = gameManager.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI()
    {
        // Mise � jour des textes de score et de vies
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
