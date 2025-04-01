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
    [SerializeField] public float mass = 0.8f;
    public float g = 9.81f; // gravit�
    public float k = 10f;   // constante du ressort
    public float f2 { get; private set; } // coefficient de frottement (calcul� via mass)

    private Vector3 velocity; // vitesse actuelle
    private bool isLaunched = false;

    // Capacit� sp�ciale
    public BirdType birdType = BirdType.Normal;
    private bool hasDashed = false;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float radiusExplosion = 2f;
    [SerializeField] private float explosionForce = 5f;

    private BirdManager birdManager; // r�f�rence au manager pour notifier la fin de vol

    private void Start()
    {
        f2 = 0.2f / mass;
        birdManager = FindObjectOfType<BirdManager>();
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
        Vector3 gravity = new Vector3(0, -g, 0);
        velocity += gravity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        if (velocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
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
                    Jump();
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
        Vector2 dashDirection = velocity.normalized;
        velocity += (Vector3)dashDirection * dashForce;
        hasDashed = true;
    }

    private void Jump()
    {
        // Remettre une impulsion verticale
        velocity = new Vector2(velocity.x, jumpForce);
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
                Vector2 direction = obj.transform.position - transform.position;
                float distance = direction.magnitude;
                float forceFactor = 1 - (distance / radiusExplosion);
                obj.transform.position += (Vector3)(direction.normalized * explosionForce * forceFactor * Time.deltaTime);
            }
        }
    }

    public void SetInitialVelocity(Vector3 initialVelocity)
    {
        velocity = initialVelocity;
        isLaunched = true;
    }

    public void SetLaunched(bool launched)
    {
        isLaunched = launched;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLaunched)
        {
            isLaunched = false;
            birdManager?.BirdLanded();
        }
    }
}
