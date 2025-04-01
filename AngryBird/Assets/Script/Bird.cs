using UnityEngine;

public enum BirdType
{
    Normal,
    Fast,
    DoubleJump,
    Explosive
}

public class Bird : MonoBehaviour
{
    // Param�tres de physique
    [SerializeField] public float mass = 0.8f; // Masse de l'oiseau
    public float g = 9.81f; // Gravit�
    public float k = 10f;   // Constante du ressort
    public float f2 { get; private set; } // Coefficient de frottement (calcul� via la masse)

    private Vector3 velocity; // Vitesse courante de l'oiseau
    private bool isLaunched = false; // Indique si l'oiseau a �t� lanc�

    // Capacit� sp�ciale
    public BirdType birdType = BirdType.Normal; // Type de l'oiseau
    private bool hasDashed = false; // Indique si l'oiseau a utilis� son dash
    private bool hasJumped = false; // Indique si l'oiseau a utilis� son jump
    [SerializeField] private float dashForce = 10f; // Force du dash
    [SerializeField] private float jumpForce = 5f; // Force du saut
    [SerializeField] private float radiusExplosion = 2f; // Rayon de l'explosion
    [SerializeField] private float explosionForce = 5f; // Force de l'explosion

    private BirdManager birdManager; // R�f�rence au manager pour notifier la fin de vol

    private void Start()
    {
        f2 = 0.2f / mass; // Calcul du coefficient de frottement
        birdManager = FindObjectOfType<BirdManager>(); // Trouver le BirdManager dans la sc�ne
    }

    private void Update()
    {
        if (isLaunched)
        {
            // Appliquer la gravit�
            ApplyGravity();
            // Mettre � jour la position
            transform.position += velocity * Time.deltaTime;
            // Mettre � jour la rotation en fonction de la vitesse
            UpdateRotation();
            // G�rer les capacit�s sp�ciales via l'input utilisateur
            HandleSpecialAbility();
        }
    }

    public bool IsLaunched
    {
        get { return isLaunched; }
    }

    private void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, -g, 0); // Vecteur de gravit�
        velocity += gravity * Time.deltaTime; // Appliquer la gravit� � la vitesse
    }

    private void UpdateRotation()
    {
        if (velocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg; // Calculer l'angle de rotation
            transform.rotation = Quaternion.Euler(0, 0, angle); // Appliquer la rotation
        }
    }

    private void HandleSpecialAbility()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (birdType)
            {
                case BirdType.Fast:
                    if (!hasDashed) { Dash(); }
                    break;
                case BirdType.DoubleJump:
                    if (!hasJumped) { Jump(); }
                    break;
                case BirdType.Explosive:
                    Explode();
                    break;
                default:
                    break;
            }
        }
    }

    private void Dash()
    {
        Vector2 dashDirection = velocity.normalized; // Direction du dash
        velocity += (Vector3)dashDirection * dashForce; // Appliquer la force du dash
        hasDashed = true; // Marquer le dash comme utilis�
    }

    private void Jump()
    {
        // Appliquer une impulsion verticale
        velocity = new Vector2(velocity.x, jumpForce);
        hasJumped = true;
    }

    private void Explode()
    {
        // Recherche d�objets dans la zone d�explosion
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, radiusExplosion);
        foreach (Collider2D obj in objectsInRange)
        {
            // On �vite de modifier la position de soi-m�me
            if (obj.gameObject != gameObject)
            {
                Vector2 direction = obj.transform.position - transform.position; // Direction de l'explosion
                float distance = direction.magnitude; // Distance de l'objet
                float forceFactor = 1 - (distance / radiusExplosion); // Facteur de force bas� sur la distance
                obj.transform.position += (Vector3)(direction.normalized * explosionForce * forceFactor * Time.deltaTime); // Appliquer la force de l'explosion
            }
        }
    }

    public void SetInitialVelocity(Vector3 initialVelocity)
    {
        velocity = initialVelocity; // D�finir la vitesse initiale
        isLaunched = true; // Marquer l'oiseau comme lanc�
    }

    public void SetLaunched(bool launched)
    {
        isLaunched = launched; // D�finir l'�tat de lancement
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLaunched)
        {
            isLaunched = false; // Marquer l'oiseau comme non lanc�
            birdManager?.BirdLanded(); // Notifier le BirdManager que l'oiseau a atterri

            // Calculer les d�g�ts bas�s sur la vitesse de l'oiseau
            float collisionForce = velocity.magnitude;
            int damage = CalculateDamage(collisionForce);

            // Appliquer les d�g�ts � l'objet destructible
            DestructibleObject destructible = collision.gameObject.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.TakeDamage(damage);
            }

            // Ajouter un Rigidbody2D pour appliquer la gravit�
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.mass = mass;
            rb.gravityScale = 1; // Ajuster la gravit� si n�cessaire
        }
    }

    private int CalculateDamage(float collisionForce)
    {
        int baseDamage = Mathf.RoundToInt(collisionForce * 10); // Exemple de calcul de base des d�g�ts
        switch (birdType)
        {
            default:
                return baseDamage;
        }
    }
}
