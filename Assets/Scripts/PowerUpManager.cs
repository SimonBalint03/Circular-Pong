using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : MonoBehaviour
{
    public GameObject genericPowerUpPrefab;
    public List<PowerUpController> powerUps = new List<PowerUpController>();
    [SerializeField] private CircleCollider2D circleCollider;
    
    public static PowerUpManager Instance { get; private set; }

    
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

    public void SpawnRandomInsideCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle * circleCollider.radius;
        Vector2 worldPosition = (Vector2)circleCollider.transform.position + randomPoint;
        
        SpawnPowerUp(worldPosition);
    }

    private void SpawnPowerUp(Vector2 position, [Optional] PowerUp powerUp)
    {
        GameObject powerUpObject = Instantiate(genericPowerUpPrefab, position, Quaternion.identity);
        PowerUpController powerUpController = powerUpObject.GetComponent<PowerUpController>();
        
        powerUpObject.transform.localScale = Vector3.zero;
        powerUpObject.transform.DOScale(Vector3.one*0.5f, 0.5f);

        powerUpController.powerUp = powerUp
            ? powerUp
            : PowerUpDatabase.Instance.powerUps[Random.Range(0, PowerUpDatabase.Instance.powerUps.Count)];
        powerUpController.UpdateIcon();
        
        powerUps.Add(powerUpController);
        
    }

    public void RemoveAllPowerUps()
    {
        int count = powerUps.Count;
        
        for (int i = 0; i < count; i++)
        {
            RemovePowerUp(powerUps[0]);
        }
        //powerUps = new List<PowerUpController>();
    }
    
    public void RemovePowerUp(PowerUpController powerUp)
    {
        StartCoroutine(RemovePowerUpAnimate(powerUp, 0.3f));
    }

    private IEnumerator RemovePowerUpAnimate(PowerUpController powerUp, float duration)
    {
        powerUp.transform.DOScale(Vector3.zero, duration);
        yield return new WaitForSeconds(duration);
        powerUps.Remove(powerUp);
        Destroy(powerUp.gameObject);
    }
    

}
