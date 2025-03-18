using System.Collections;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [Header("Birds")]
    [SerializeField] private GameObject normalBirdPrefab;
    [SerializeField] private GameObject fastBirdPrefab;
    [SerializeField] private int maxBirds;

    private int remainingBirds;
    private Rigidbody2D bird;
    private Collider2D birdCollider;
    private bool isFastBird;
    private bool hasDashed;
    private bool isLaunched;

    [Header("Fast Bird Settings")]
    [SerializeField] private float dashForce = 10f;

    void Update()
    {
        if (isFastBird && bird != null && !hasDashed && Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }
    }

    public void InitializeBirds()
    {
        remainingBirds = maxBirds;
        CreateBird();
    }

    public void CreateBird()
    {
        if (remainingBirds > 0)
        {
            GameObject birdPrefab = (Random.value > 0.5f) ? fastBirdPrefab : normalBirdPrefab;
            bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
            birdCollider = bird.GetComponent<Collider2D>();
            birdCollider.enabled = false;

            bird.isKinematic = true;
            remainingBirds--;

            isFastBird = (birdPrefab == fastBirdPrefab);
            hasDashed = false;
            isLaunched = false;
        }
    }

    public Rigidbody2D GetCurrentBird()
    {
        return bird;
    }

    public void EnableCollider()
    {
        if (birdCollider)
        {
            birdCollider.enabled = true;
        }
    }

    public void UpdateBirdPosition(Vector3 position, Vector3 center, float offset)
    {
        if (bird != null && !isLaunched)
        {
            Vector3 direction = position - center;
            bird.transform.position = position + direction.normalized * offset;
            bird.transform.right = -direction.normalized;
        }
    }

    public void Shoot(Vector3 currentPosition, Vector3 centerPosition, float force)
    {
        if (bird != null)
        {
            bird.isKinematic = false;
            Vector3 birdForce = (currentPosition - centerPosition) * force * -1;
            bird.velocity = birdForce;
            isLaunched = true;
        }
    }

    private void Dash()
    {
        if (bird != null)
        {
            Vector2 dashDirection = bird.velocity.normalized;
            bird.velocity += dashDirection * dashForce;
            hasDashed = true;
        }
    }
}
