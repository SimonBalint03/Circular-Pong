using System.Collections.Generic;
using UnityEngine;

public class PowerUpDatabase : MonoBehaviour
{
    public static PowerUpDatabase Instance { get; private set; }
    
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
    
    public List<PowerUp> powerUps = new List<PowerUp>();
}
