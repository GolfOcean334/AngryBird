using System.Collections;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [Header("Birds")]
    // Références aux prefabs des différents types d'oiseaux
    [SerializeField] private GameObject normalBirdPrefab;
    [SerializeField] private GameObject fastBirdPrefab;
    [SerializeField] private GameObject doubleJumpBirdPrefab;
    [SerializeField] private GameObject explosiveBirdPrefab;
    [SerializeField] private int maxBirds; // Nombre maximum d'oiseaux disponibles

    private int remainingBirds; // Nombre d'oiseaux restants
    private Rigidbody2D bird; // Référence au Rigidbody de l'oiseau actuel
    private Collider2D birdCollider; // Référence au Collider de l'oiseau actuel
    private BirdType currentBirdType; // Type de l'oiseau actuel
    private bool hasDashed; // Indique si l'oiseau rapide a déjà dashé
    private bool isLaunched; // Indique si l'oiseau a été lancé

    [Header("Fast Bird Settings")]
    [SerializeField] private float dashForce = 10f; // Force du dash pour l'oiseau rapide

    [Header("Jump Bird Settings")]
    [SerializeField] private float jumpForce; // Force du saut pour l'oiseau double saut

    [Header("Explosive Bird Settings")]
    [SerializeField] private float radiusExplosion; // Rayon d'explosion pour l'oiseau explosif
    [SerializeField] private float explosionForce; // Force de l'explosion

    // Enumération des types d'oiseaux disponibles
    private enum BirdType
    {
        Normal,
        Fast,
        DoubleJump,
        Explosive
    }

    void Update()
    {
        // Si aucun oiseau n'est actif ou qu'il n'a pas été lancé, ne rien faire
        if (bird == null || !isLaunched) return;

        // Détection de l'entrée utilisateur pour déclencher une capacité spéciale
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentBirdType)
            {
                case BirdType.Fast:
                    if (!hasDashed) Dash();
                    break;
                case BirdType.DoubleJump:
                    Jump();
                    break;
                case BirdType.Explosive:
                    Explode();
                    break;
            }
        }

        UpdateBirdRotation();
    }

    // Initialise le nombre d'oiseaux et en crée un premier
    public void InitializeBirds()
    {
        remainingBirds = maxBirds;
        CreateBird();
    }

    // Crée un nouvel oiseau aléatoire parmi les types disponibles
    public void CreateBird()
    {
        if (remainingBirds <= 0) return; // Vérifie si des oiseaux sont encore disponibles

        float randomValue = Random.value;
        GameObject birdPrefab;

        // Sélectionne un type d'oiseau en fonction d'une valeur aléatoire
        if (randomValue > 0.75f)
        {
            birdPrefab = fastBirdPrefab;
            currentBirdType = BirdType.Fast;
        }
        else if (randomValue > 0.5f)
        {
            birdPrefab = doubleJumpBirdPrefab;
            currentBirdType = BirdType.DoubleJump;
        }
        else if (randomValue > 0.25f)
        {
            birdPrefab = explosiveBirdPrefab;
            currentBirdType = BirdType.Explosive;
        }
        else
        {
            birdPrefab = normalBirdPrefab;
            currentBirdType = BirdType.Normal;
        }

        // Instancie le prefab de l'oiseau sélectionné
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;
        bird.isKinematic = true; // Empêche l'oiseau de bouger avant le lancement
        remainingBirds--;

        hasDashed = false;
        isLaunched = false;

        // Marque l'oiseau comme lancé dans son script dédié
        Bird birdScript = bird.GetComponent<Bird>();
        birdScript?.SetLaunched(true);
    }

    // Réinitialise le type d'oiseau lorsqu'il atterrit
    public void BirdLanded()
    {
        currentBirdType = BirdType.Normal;
    }

    // Retourne l'oiseau actuellement en jeu
    public Rigidbody2D GetCurrentBird()
    {
        return bird;
    }

    // Active le collider de l'oiseau
    public void EnableCollider()
    {
        if (birdCollider)
        {
            birdCollider.enabled = true;
        }
    }

    // Met à jour la position et la rotation de l'oiseau avant son lancement
    public void UpdateBirdPosition(Vector3 position, Vector3 center, float offsetX, float offsetY)
    {
        if (bird == null || isLaunched) return;

        // Calcule le vecteur directionnel entre la position actuelle de la souris (position) et le centre de la fronde (center)
        Vector3 direction = position - center;

        // Normalise la direction pour obtenir un vecteur unitaire (de longueur 1)
        // Ensuite, applique un décalage horizontal (offsetX) et vertical (offsetY) pour ajuster la position de l'oiseau
        bird.transform.position = position + direction.normalized * offsetX + new Vector3(0, offsetY, 0);

        // Oriente l'oiseau dans la direction opposée au vecteur directionnel
        bird.transform.right = -direction.normalized;
    }

    // Lance l'oiseau avec une force calculée
    public void Shoot(Vector3 currentPosition, Vector3 centerPosition, float force)
    {
        if (bird == null) return;
        bird.isKinematic = false;

        // Calcule la force de tir : (différence entre la position actuelle et le centre de la fronde) * force
        // Multiplie par -1 pour que l'oiseau parte dans la direction opposée au tir
        Vector3 birdForce = (currentPosition - centerPosition) * force * -1;

        // Applique la force calculée à la vélocité de l'oiseau pour le propulser
        bird.velocity = birdForce;

        // Marque l'oiseau comme lancé
        isLaunched = true;
    }

    // Applique un dash à l'oiseau rapide
    private void Dash()
    {
        if (bird == null) return;

        // Récupère la direction actuelle du mouvement de l'oiseau sous forme d'un vecteur normalisé
        Vector2 dashDirection = bird.velocity.normalized;

        // Applique une force supplémentaire dans cette direction pour accélérer l'oiseau
        bird.velocity += dashDirection * dashForce;

        hasDashed = true;
    }

    // Applique un saut à l'oiseau double saut
    private void Jump()
    {
        if (bird == null) return;

        // Augmente la composante verticale (y) de la vélocité de l'oiseau pour lui donner un effet de saut
        bird.velocity = new Vector2(bird.velocity.x, jumpForce);
    }

    // Déclenche une explosion autour de l'oiseau explosif
    private void Explode()
    {
        if (bird == null) return;

        // Trouve tous les objets dans un rayon donné autour de l'oiseau
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(bird.transform.position, radiusExplosion);

        foreach (Collider2D obj in objectsInRange)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

            // Applique l'explosion uniquement aux objets possédant un Rigidbody2D et qui ne sont pas l'oiseau lui-même
            if (rb != null && rb != bird)
            {
                // Calcule la direction entre l'oiseau et l'objet touché par l'explosion
                Vector2 direction = obj.transform.position - bird.transform.position;

                // Calcule la distance entre l'objet et l'oiseau
                float distance = direction.magnitude;

                // Établit un facteur de force basé sur la distance (plus proche = plus de force)
                float forceFactor = 1 - (distance / radiusExplosion);

                // Applique une force dans la direction opposée à l'explosion avec une intensité proportionnelle
                rb.AddForce(direction.normalized * explosionForce * forceFactor, ForceMode2D.Impulse);
            }
        }
    }


    // Met à jour la rotation de l'oiseau en fonction de sa vitesse
    private void UpdateBirdRotation()
    {
        if (bird == null) return;

        float angle = Mathf.Atan2(bird.velocity.y, bird.velocity.x) * Mathf.Rad2Deg;
        bird.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
