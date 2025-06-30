using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerID playerID;
    [SerializeField] private int score;
    
    public GameObject arcPaddlePrefab;
    public CircularArc ownArc;
    public Vector2 direction;

    public float angularSpeed = 90f;
    public float radius = 3f;
    
    public float startingAngle = 0f;
    public List<PowerUp> powerUps = new List<PowerUp>();
    public bool removePowerUpsRecursively = true;
    
    private float currentAngle;
    
    private bool pushingUp;
    private bool pushingDown;
    private bool pushingLeft;
    private bool pushingRight;
    
    // Events
    public static event Action<PlayerID,int> OnPlayerScoreChanged; // Returns the score after it was changed.
    
    public void MultiplySideLength(float value)
    {
        ownArc.angleDegrees *= value;
    }
    
    public void SetSideLength(float value)
    {
        ownArc.angleDegrees = value;
    }

    private void Start()
    {
        SetCurrentAngle(startingAngle);
    }

    public void SetCurrentAngle(float value)
    {
        currentAngle = value;
    }

    void Update()
    {
        
        Vector2 input = Vector2.zero;
        if (pushingUp) input += Vector2.up;
        if (pushingDown) input += Vector2.down;
        if (pushingLeft) input += Vector2.left;
        if (pushingRight) input += Vector2.right;

        direction = input;
        
        if (input != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            targetAngle = (targetAngle + 360f) % 360f;
            
            float angleDelta = Mathf.DeltaAngle(currentAngle, targetAngle);
            currentAngle += Mathf.Clamp(angleDelta, -angularSpeed * Time.deltaTime, angularSpeed * Time.deltaTime);

        }
        
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        
    }
    
    public void OnPushUp(InputAction.CallbackContext context)
    {
        pushingUp = context.ReadValueAsButton();
    }

    public void OnPushDown(InputAction.CallbackContext context)
    {
        pushingDown = context.ReadValueAsButton();
    }

    public void OnPushRight(InputAction.CallbackContext context)
    {
        pushingRight = context.ReadValueAsButton();
    }

    public void OnPushLeft(InputAction.CallbackContext context)
    {
        pushingLeft = context.ReadValueAsButton();
    }
    
    
    
    private class ActivePowerUp
    {
        public PowerUp powerUp;
        public float endTime;

        public ActivePowerUp(PowerUp powerUp, float endTime)
        {
            this.powerUp = powerUp;
            this.endTime = endTime;
        }
    }
    
    public void AddPowerUp(PowerUp powerUp)
    {
        if (!powerUp.CanApply(this))
        {
            Debug.Log("Can't apply powerup." + powerUp.name);
            return;
        }
        
        powerUps.Add(powerUp);
        powerUp.Apply(this);
        StartCoroutine(RemoveAfterDuration(powerUp));
    }

    private IEnumerator RemoveAfterDuration(PowerUp powerUp)
    {
        yield return new WaitForSeconds(powerUp.duration);
        if (!powerUp.CanRemove(this))
        {
            Debug.Log("Can't remove powerup." + powerUp.name);
            if (removePowerUpsRecursively) { StartCoroutine(RemoveAfterDuration(powerUp)); }

        }
        else
        {
            powerUp.Remove(this);
            powerUps.Remove(powerUp);
        }
        
    }

    public void AddScore(int amount)
    {
        score += amount;
        OnPlayerScoreChanged?.Invoke(playerID,score);
    }

    public void RemoveScore(int amount)
    {
        score -= amount;
        OnPlayerScoreChanged?.Invoke(playerID,score);
    }

    public void ResetScore()
    {
        score = 0;
        OnPlayerScoreChanged?.Invoke(playerID,score);
    }
}
