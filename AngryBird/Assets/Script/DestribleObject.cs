using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private Sprite[] damageSprites;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private int damageMultiplier = 0;

    [SerializeField] private GameObject floatingScoreTextPrefab;
    [SerializeField] private int nbpoints = 100;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bird") || collision.gameObject.CompareTag("Destructible"))
        {
            float collisionForce = collision.relativeVelocity.magnitude;
            int damage = Mathf.RoundToInt(collisionForce * damageMultiplier);
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        int oldHealth = currentHealth;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateSprite();

        int lostHealth = oldHealth - currentHealth;
        int pointsGagnes = lostHealth * nbpoints;

        if (pointsGagnes > 0)
        {
            ScoreManager.instance.AddScore(pointsGagnes);

            if (floatingScoreTextPrefab != null)
            {
                float rangeSpawnText = Random.Range(-2, 2);
                Vector3 spawnPosition = transform.position + new Vector3(rangeSpawnText, -2f, 0);
                GameObject floatingText = Instantiate(floatingScoreTextPrefab, spawnPosition, Quaternion.identity);
                FloatingScoreText fst = floatingText.GetComponent<FloatingScoreText>();
                if (fst != null)
                {
                    fst.Initialize(pointsGagnes);
                }
            }
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }


    private void UpdateSprite()
    {
        int spriteIndex = Mathf.FloorToInt((1 - (float)currentHealth / maxHealth) * (damageSprites.Length - 1));
        spriteRenderer.sprite = damageSprites[spriteIndex];
    }
}
