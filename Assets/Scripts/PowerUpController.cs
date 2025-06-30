using System;
using System.Collections;
using PowerUps;
using TMPro;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public PowerUp powerUp;
    public TextMeshProUGUI valueText;
    
    private SpriteRenderer spriteRenderer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Ball")
        {
            PlayerController player = 
                GameManager.Instance.players.Find(
                    x => x.playerID ==
                         other.transform.GetComponent<BallController>().lastHitPlayer);
            player.AddPowerUp(powerUp);
            PowerUpManager.Instance.RemovePowerUp(this);
        }    
    }

    public void UpdateIcon()
    {
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = powerUp.icon;
        
        if (powerUp is ModifyArc modifyArc)
        {
            valueText.text = modifyArc.modifyValue.ToString();
        }
    }
    
}
