using System;
using System.Collections;
using DG.Tweening;
using EasyTextEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [Header("Score Display")] public TextMeshProUGUI player1Score;
    private TextEffect player1TextEffect;
    public TextMeshProUGUI player2Score;
    private TextEffect player2TextEffect;
    [Header("Power Up Timer")]
    public TextMeshProUGUI powerUpTimerText;
    public Slider powerUpTimerSlider;
    [Header("Pause Menu")]
    public GameObject pauseMenu;

    void Start()
    {
        // Text Effects
        player1TextEffect = player1Score.GetComponent<TextEffect>();
        player2TextEffect = player2Score.GetComponent<TextEffect>();
    }

    private void OnEnable()
    {
        GameManager.OnBallLeavePlayArea += OnBallLeavePlayArea;
        GameManager.OnPowerUpSpawned += OnPowerUpSpawned;
        GameManager.OnGameStarting += OnGameStarting;
        PlayerController.OnPlayerScoreChanged += OnPlayerScoreChanged;
        
    }
    
    private void OnDisable()
    {
        GameManager.OnBallLeavePlayArea -= OnBallLeavePlayArea;
        GameManager.OnPowerUpSpawned -= OnPowerUpSpawned;
        GameManager.OnGameStarting -= OnGameStarting;
        PlayerController.OnPlayerScoreChanged -= OnPlayerScoreChanged;
    }
    
    private void OnGameStarting()
    {
        StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        Time.timeScale = 0;
        pauseMenu.transform.DOMove(Vector3.down * 10, 1f).SetEase(Ease.InQuad).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;

    }

    private void OnBallLeavePlayArea(PlayerID playerID)
    {
        
    }
    
    private void OnPowerUpSpawned(float remainingTime)
    {
        powerUpTimerText.text = "Next power up in: "+remainingTime.ToString("00")+"s";
        powerUpTimerSlider.maxValue = remainingTime;
        powerUpTimerSlider.value = remainingTime;
        StartCoroutine(AnimateTimerValues(remainingTime));
    }

    private void OnPlayerScoreChanged(PlayerID playerID, int score)
    {
        switch (playerID)
        {
            case PlayerID.Player1:
                player1Score.text = "P1-  " + score.ToString("D2");
                player1TextEffect.StartManualEffects();
                break;
            case PlayerID.Player2:
                player2Score.text = "P2-  " + score.ToString("D2");
                player2TextEffect.StartManualEffects();
                break;
            case PlayerID.Player3:
                break;
            case PlayerID.Player4:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(playerID), playerID, null);
        }
        
    }

    private IEnumerator AnimateTimerValues(float totalTime)
    {
        do
        {
            yield return new WaitForEndOfFrame();
            totalTime -= Time.deltaTime;
            powerUpTimerSlider.value = totalTime;
            powerUpTimerText.text = "Next power up in: "+totalTime.ToString("00")+"s";
        } while (totalTime >= 0.05f);
    }
}