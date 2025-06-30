using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Player Settings")]
    public PlayerInputManager playerInputManager;
    public int playerCount = 2;
    public List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private int joinedPlayerCount = 0;
    [SerializeField] private Color player1Color;
    [SerializeField] private Color player2Color;

    [Header("Ball Settings")]
    public int ballsToSpawn = 1;
    public List<BallController> balls = new List<BallController>();
    public Vector3 ballSize = Vector3.one;
    public GameObject ballPrefab;
    public CircleCollider2D playAreaCollider;
    
    [Header("PowerUp Settings")]
    public bool playWithPowerUps = true;
    public float minTimeBetweenPowerUps = 3f;
    public float maxTimeBetweenPowerUps = 10f;
    [SerializeField]private float currentTimeBetweenPowerUps;
    [SerializeField]private List<Coroutine> powerUpTimers = new List<Coroutine>();
    
    public static GameManager Instance { get; private set; }
    
    private bool isQuitting = false;


    // Events
    public static event Action<PlayerID> OnBallLeavePlayArea;
    public static event Action<float> OnPowerUpSpawned;
    public static event Action OnGameStarting;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (BallController ball in balls)
            {
                ball.ResetBall();
            }

            foreach (PlayerController player in players)
            {
                player.ResetScore();
            }
            // Reset power up spawning
            StopAllPowerUpTimers();
            powerUpTimers.Add(StartCoroutine(PowerUpSpawnRoutine()));
            // Remove all powerUps.
            PowerUpManager.Instance.RemoveAllPowerUps();
            // Reset player positions.
            foreach (PlayerController playerController in players)
            {
                playerController.SetCurrentAngle(playerController.startingAngle);
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PowerUpManager.Instance.SpawnRandomInsideCircle();
        }
    }

    private void OnEnable()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
        StopAllPowerUpTimers();
    }

    public void StartGame()
    {
        OnGameStarting?.Invoke();
        StartCoroutine(JoinPlayersRoutine());
        powerUpTimers.Add(StartCoroutine(PowerUpSpawnRoutine()));
    }

    public void ExitGame()
    {
        isQuitting = true;
        Application.Quit();
    }

    private IEnumerator PowerUpSpawnRoutine()
    {
        if (!playWithPowerUps) { yield break; }
        Debug.Log("PowerUp spawn routine");
        currentTimeBetweenPowerUps = Random.Range(minTimeBetweenPowerUps, maxTimeBetweenPowerUps);
        OnPowerUpSpawned?.Invoke(currentTimeBetweenPowerUps);
        yield return new WaitForSeconds(currentTimeBetweenPowerUps);
        PowerUpManager.Instance.SpawnRandomInsideCircle();
        powerUpTimers.Add(StartCoroutine(PowerUpSpawnRoutine()));

        
    }

    private IEnumerator JoinPlayersRoutine()
    {
        string[] schemes = new[] { "KeyboardScheme1", "KeyboardScheme2" };

        for (int i = 0; i < playerCount; i++)
        {
            string schemeName = schemes[i];
            Debug.Log($"Joining player {i} with scheme {schemeName}");

            playerInputManager.JoinPlayer(
                playerIndex: -1,
                pairWithDevices: new[] { Keyboard.current },
                controlScheme: schemeName
            );

            yield return null;
        }
    }
    
    


    private void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log($"Player joined: {player.playerIndex}");

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Missing PlayerController on player prefab!");
            return;
        }

        // Switch action map
        player.SwitchCurrentActionMap("Player " + (player.playerIndex + 1));
        Debug.Log($"Player's action map: {player.currentActionMap}");

        players.Add(playerController);
        playerController.playerID = (PlayerID)joinedPlayerCount;

        switch (joinedPlayerCount)
        {
            case 0:
                playerController.ownArc.GetComponent<MeshRenderer>().material.color = player1Color;
                playerController.transform.rotation = Quaternion.Euler(0, 0, 0);
                playerController.startingAngle = 180f;
                break;
            case 1:
                playerController.ownArc.GetComponent<MeshRenderer>().material.color = player2Color;
                playerController.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            // Add more colors and rotations if more than 2 players
        }

        joinedPlayerCount++;

        if (joinedPlayerCount == playerCount)
        {
            for (int i = 0; i < ballsToSpawn; i++)
            {
                SpawnBall();
            }
        }
    }

    private void SpawnBall()
    {
        GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        ball.transform.localScale = Vector3.zero;
        ball.transform.DOScale(ballSize, 0.3f).SetEase(Ease.InQuad);
        balls.Add(ball.GetComponent<BallController>());
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isQuitting) return;
        if (other.CompareTag("Ball"))
        {
            BallController ballController = other.GetComponent<BallController>();
            BallLeftPlayArea(ballController,ballController.lastHitPlayer);
            OnBallLeavePlayArea?.Invoke(ballController.lastHitPlayer);
        }
    }

    private void BallLeftPlayArea(BallController ball, PlayerID playerID)
    {
        PlayerController lastHitPlayer = players.Find(player => player.playerID == playerID);
        // Add score
        lastHitPlayer.AddScore(1);
        // Remove previous ball
        StartCoroutine(DestroyBall(ball));
        // Restart powerUp spawning.
        StopAllPowerUpTimers();
        powerUpTimers.Add(StartCoroutine(PowerUpSpawnRoutine()));
        // Spawn new ball
        SpawnBall();
        
        
    }

    private IEnumerator DestroyBall(BallController ball)
    {
        ball.transform.DOScale(Vector3.zero, 0.3f);
        balls.Remove(ball);
        yield return new WaitForSeconds(0.3f);
        Destroy(ball.gameObject);
    }

    private void StopAllPowerUpTimers()
    {
        powerUpTimers.ForEach(StopCoroutine);
        powerUpTimers.Clear();
    }
    
}