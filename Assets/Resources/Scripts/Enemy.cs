using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float forceToPlayer;
    public float forceToRandom;
    public float scaleIncreaseValue;
    public Color[] hitColors;

    Rigidbody2D rb;
    Collider2D myCollider;
    Animation animationComponent;
    Vector2 targetVector;
    SpriteRenderer spriteRenderer;
    GameObject fireParticles;

    bool isInsideWorld;
    bool isDanger;
    int numOfHitsToPlayer;

    public void Initialize(bool isDanger)
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animationComponent = GetComponent<Animation>();
        if(animationComponent) animationComponent.Stop();

        myCollider.enabled = false;
        this.isDanger = isDanger;

        if(isDanger)
        {
            fireParticles = transform.GetChild(0).gameObject;
            if (fireParticles) fireParticles.SetActive(false);
        }
    }

    public void AddForceTowardsTarget()
    {
        targetVector = new Vector2(Random.Range(-MainFlow.Instance.worldWidth, MainFlow.Instance.worldWidth),
                        Random.Range(-MainFlow.Instance.worldHeight, MainFlow.Instance.worldHeight));
        rb.AddForce((targetVector - (Vector2)transform.position).normalized * Random.Range(3f,5f), ForceMode2D.Impulse);
    }

    public void RefreshUpdate()
    {
        if(!isInsideWorld)
        {
            if (transform.position.y < (MainFlow.Instance.GetWallPlacingRef.GetWallsHeight - 1f))
            {
                myCollider.enabled = true;
                isInsideWorld = true;
            }
        }
    }

    void GoTowardsPlayer()
    {
        Vector2 playerPosition = new Vector2(MainFlow.Instance.playerBall.transform.position.x, MainFlow.Instance.playerBall.transform.position.y);
        Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;
        Vector2 vectorNearPlayer = new Vector2(Random.Range(direction.x - 0.3f, direction.x + 0.3f), 
                                    Random.Range(direction.y - 0.3f, direction.y + 0.3f));
        rb.AddForce(vectorNearPlayer * forceToPlayer, ForceMode2D.Impulse);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetSpriteColor(int index)
    {
        spriteRenderer.color = hitColors[index];
    }

    void IncreaseScaleAndDecreaseForce()
    {
        transform.localScale += new Vector3(scaleIncreaseValue, scaleIncreaseValue, 1);
    }

    void UpdateManagers(Vector2 pos)
    {
        if(!isDanger) MainFlow.Instance.PlayExplosionAt(pos);

        MainFlow.Instance.soundManager.PlaySafeHitSound();
        SpawnManager.Instance.RemoveEnemy(this);
    }

    void UpdateHitsToPlayer()
    {
        numOfHitsToPlayer++;
        SetSpriteColor(numOfHitsToPlayer >= 3 ? 2 : numOfHitsToPlayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D otherCollider = collision.collider;

        if(otherCollider.CompareTag("Player"))
        {
            if (!isDanger)
            {
                UpdateManagers(otherCollider.transform.position);
                gameObject.SetActive(false);
            }
            else
            {
                IncreaseScaleAndDecreaseForce();
                UpdateHitsToPlayer();
                CameraShaker.Instance.ShakeCameraOnDangerTouch();

                if(numOfHitsToPlayer == 2)
                {
                    animationComponent.Play("enemyBlink");
                    fireParticles.SetActive(true);
                }
                else if (numOfHitsToPlayer >= 3)
                {
                    UpdateManagers(collision.transform.position);

                    CameraShaker.Instance.ShakeCameraOnPlayerDeath();
                    MainFlow.Instance.PlayerDied();
                    MainFlow.Instance.PlayPlayerExplosionAt(otherCollider.transform.position);

                    otherCollider.GetComponent<Ball>().BlastShockwaveForce();
                    otherCollider.gameObject.SetActive(false);
                }
            }
        }
        else if (otherCollider.CompareTag("Danger") || otherCollider.CompareTag("Safe"))
        {
            MainFlow.Instance.soundManager.PlayEnemiesHitSound();
        }
        else
        {
            if(otherCollider.CompareTag("Wall") && isDanger)
            {
                GoTowardsPlayer();
            }
            else
            {
                Vector2 normal = collision.contacts[0].normal;
                rb.AddForce(normal * forceToRandom, ForceMode2D.Impulse);
            }
        }
    }
}
