using UnityEngine;

public class Bird : MonoBehaviour
{
    private BirdManager birdManager;
    private Rigidbody2D rb; // Composant Rigidbody2D permettant la gestion de la physique de l'oiseau
    private bool isLaunched; // Indique si l'oiseau a été lancé par le lance-pierre

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Récupère le composant Rigidbody2D attaché à l'oiseau
        birdManager = FindObjectOfType<BirdManager>(); // Trouve automatiquement l'instance du BirdManager dans la scène
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si l'oiseau a été lancé avant de signaler l'atterrissage
        if (isLaunched)
        {
            birdManager.BirdLanded(); // Informe le BirdManager que l'oiseau a atterri après un tir
        }
    }

    public void SetLaunched(bool launched)
    {
        isLaunched = launched; // Définit l'état de lancement de l'oiseau
    }
}
