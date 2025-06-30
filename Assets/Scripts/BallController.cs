using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D circleCollider;
    public Rigidbody2D rb;
    public GameObject impactParticles;
    
    
    public float launchSpeed = 10f;
    public float speedIncreaseFactor = 1.05f; // 5% speed increase per hit
    public Vector2 maxSpeed;
    public bool randomLaunch = false;
    public float launchAngle = 0f;
    
    public PlayerID lastHitPlayer;
    
    private bool didExit = false;
    
    void Start()
    {
        LaunchInRandomDirection();
    }
    
    void LaunchInRandomDirection()
    {
        float angle = launchAngle;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        rb.linearVelocity = direction.normalized * launchSpeed;
    }
    
    public void ResetBall()
    {
        transform.position = Vector3.zero;
        float angle = launchAngle;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        rb.linearVelocity = direction.normalized * launchSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        StartCoroutine(CreateAndDeleteParticles(1f,other));
        
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        stayFrameCounter = 0;
        lastHitPlayer = other.transform.parent.GetComponent<PlayerController>().playerID;
        // Set color
        spriteRenderer.material.DOColor(other.transform.GetComponent<MeshRenderer>().material.color,0.5f);
        
        
    }

    private IEnumerator CreateAndDeleteParticles(float duration, Collision2D other)
    {
        GameObject particlesGO = Instantiate(impactParticles, transform.position, Quaternion.identity, transform);
        ParticleSystem particles = particlesGO.GetComponent<ParticleSystem>();
        
        ParticleSystem.MainModule main = particles.main;

        main.startColor = new ParticleSystem.MinMaxGradient(other.transform.GetComponent<Renderer>().material.color,
            spriteRenderer.material.color);
        
        
        ContactPoint2D contact = other.contacts[0];
        particlesGO.transform.position = contact.point + contact.normal * 0.1f;
        
        particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        particles.Simulate(0f, true);
        particles.Play();
        
        yield return new WaitForSeconds(duration+0.5f);
        
        Destroy(particlesGO);
    }

    private int stayFrameCounter = 0;
    
    private void OnCollisionStay2D(Collision2D other)
    {
        stayFrameCounter++;
        if ( stayFrameCounter == 3)
        {
            transform.position = Vector3.MoveTowards(transform.position,Vector3.zero, 0.2f);
            rb.linearVelocity = (new Vector2(transform.position.x, transform.position.y) - Vector2.zero);
        }
        
        // TODO: Needs to handle other balls
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        stayFrameCounter = 0;
        if (rb.linearVelocity.magnitude < 0.5f)
        {
            rb.linearVelocity = Vector2.one;
        }
        
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed.x)
        {
            rb.linearVelocity = rb.linearVelocity.x < 0 
                ? new Vector2(-maxSpeed.x, rb.linearVelocity.y)
                : new Vector2(maxSpeed.x, rb.linearVelocity.y);
        }
        if (Mathf.Abs(rb.linearVelocity.y) > maxSpeed.y)
        {
            rb.linearVelocity = rb.linearVelocity.y < 0
                ? new Vector2(rb.linearVelocity.x, -maxSpeed.y)
                : new Vector2(rb.linearVelocity.x, maxSpeed.y);
        }
    }
}
