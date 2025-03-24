using UnityEngine;

public class Bird : MonoBehaviour
{
    private BirdManager birdManager;
    private Rigidbody2D rb;
    private bool isLaunched;

    private float _mass = 0.8f; // masse de l'oiseau (kg)
    public float mass
    {
        get { return _mass; }
        set
        {
            _mass = value;
            f2 = 0.2f / _mass; // Met à jour f2 chaque fois que mass est modifiée
        }
    }
    public float g = 9.81f; // constante gravitationnelle (m/s²)
    public float k = 10f; // constante de raideur du ressort (N/m)
    public float f2; // coeff de frottement divisé par la masse


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        birdManager = FindObjectOfType<BirdManager>();
        f2 = 0.2f / mass;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLaunched)
        {
            birdManager.BirdLanded();
        }
    }

    public void SetLaunched(bool launched)
    {
        isLaunched = launched;
    }
}
