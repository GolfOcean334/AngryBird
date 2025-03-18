using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private int maxBirds;

    private int remainingBirds;
    private Rigidbody2D bird;
    private Collider2D birdCollider;

    public void InitializeBirds()
    {
        remainingBirds = maxBirds;
        CreateBird();
    }

    public void CreateBird()
    {
        if (remainingBirds > 0)
        {
            bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
            birdCollider = bird.GetComponent<Collider2D>();
            birdCollider.enabled = false;

            bird.isKinematic = true;
            remainingBirds--;
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
        if (bird)
        {
            Vector3 direction = position - center;
            bird.transform.position = position + direction.normalized * offset;
            bird.transform.right = -direction.normalized;
        }
    }

    public void Shoot(Vector3 currentPosition, Vector3 centerPosition, float force)
    {
        if (bird)
        {
            bird.isKinematic = false;
            Vector3 birdForce = (currentPosition - centerPosition) * force * -1;
            bird.velocity = birdForce;
            bird = null;
            birdCollider = null;
        }
    }
}
