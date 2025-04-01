using UnityEngine;
using System.Collections;

public class BirdManager : MonoBehaviour
{
    [Header("Birds")]
    [SerializeField] private GameObject normalBirdPrefab;
    [SerializeField] private GameObject fastBirdPrefab;
    [SerializeField] private GameObject doubleJumpBirdPrefab;
    [SerializeField] private GameObject explosiveBirdPrefab;
    [SerializeField] private int maxBirds;

    private int remainingBirds;
    private Collider2D birdCollider;
    private GameObject bird;

    // Référence vers le UIManager pour la fin de partie
    [SerializeField] private UIManager UIManager;

    public void InitializeBirds()
    {
        remainingBirds = maxBirds;
        CreateBird();
    }

    public void CreateBird()
    {
        if (remainingBirds <= 0) return;

        float randomValue = Random.value;
        GameObject birdPrefab;
        BirdType type;

        if (randomValue > 0.66f)
        {
            birdPrefab = fastBirdPrefab;
            type = BirdType.Fast;
        }
        else if (randomValue > 0.33f)
        {
            birdPrefab = doubleJumpBirdPrefab;
            type = BirdType.DoubleJump;
        }
        else
        {
            birdPrefab = normalBirdPrefab;
            type = BirdType.Normal;
        }

        bird = Instantiate(birdPrefab);
        birdCollider = bird.GetComponent<Collider2D>();
        if (birdCollider != null)
            birdCollider.enabled = false; // désactiver jusqu'au lancement

        // Assurer une position initiale (par exemple, celle du lanceur)
        bird.transform.position = transform.position;

        remainingBirds--;

        // Transmettre le type à l'oiseau
        Bird birdScript = bird.GetComponent<Bird>();
        if (birdScript != null)
        {
            birdScript.birdType = type;
            birdScript.SetLaunched(false);
        }
    }

    public void BirdLanded()
    {
        bird = null;

        if (remainingBirds <= 0)
        {
            StartCoroutine(ShowEndGameAfterDelay());
        }
    }

    private IEnumerator ShowEndGameAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        UIManager.ShowUIEndGame();
    }

    public void EnableCollider()
    {
        if (birdCollider)
        {
            birdCollider.enabled = true;
        }
    }

    // Méthode appelée lors du lancement (depuis SlingShot)
    public void Shoot(Vector3 launchPosition, Vector3 centerPosition, float force)
    {
        if (bird == null) return;

        // Calculer la force initiale en se basant sur launchPosition
        Vector3 birdForce = (centerPosition - launchPosition) * force;

        // On active le lancement en passant la vitesse à l'oiseau
        Bird birdScript = bird.GetComponent<Bird>();
        if (birdScript != null)
        {
            birdScript.SetInitialVelocity(birdForce);
        }
    }


    public void UpdateBirdPosition(Vector3 position, Vector3 centerPosition, float offsetX, float offsetY)
    {
        Bird currentBird = GetCurrentBirdScript();
        if (currentBird != null && !currentBird.IsLaunched)
        {
            Vector3 birdPosition = position + new Vector3(offsetX, offsetY, 0);
            currentBird.transform.position = birdPosition;
        }
    }

    public Bird GetCurrentBirdScript()
    {
        return bird?.GetComponent<Bird>();
    }
}
