using UnityEngine;

public class SlingShot : MonoBehaviour
{
    [Header("Slingshot")]
    [SerializeField] private LineRenderer[] lineRenderers; // Contient les LineRenderers représentant les élastiques de la fronde
    [SerializeField] private Transform[] stripPositions; // Points d'attache des élastiques (gauche et droite)
    [SerializeField] private Transform centerPosition; // Position centrale de la fronde (d'où part l'oiseau)
    [SerializeField] private Transform idlePosition; // Position par défaut de l'élastique quand il est relâché
    [SerializeField] private Collider2D slingshotCollider; // Collider de la fronde pour éviter les interactions après un tir
    [SerializeField] private float bottomBoundary; // Limite basse pour éviter que l'oiseau ne soit trop bas avant d'être tiré
    [SerializeField] private float maxLenght; // Longueur maximale à laquelle on peut tirer l'élastique
    [SerializeField] private float force; // Force de propulsion de l'oiseau au lâcher de l'élastique

    [Header("Birds")]
    [SerializeField] private BirdManager birdManager;
    [SerializeField] private float birdPositionOffsetX; // Décalage horizontal appliqué à la position de l'oiseau dans l'élastique
    [SerializeField] private float birdPositionOffsetY; // Décalage vertical appliqué à la position de l'oiseau dans l'élastique

    private Vector3 currentPosition; // Position actuelle de la souris convertie en coordonnées du monde
    private bool isMouseDown; // Indique si la souris est en train d'être maintenue enfoncée

    private void Start()
    {
        // Initialise les LineRenderers pour afficher les élastiques de la fronde
        lineRenderers[0].positionCount = 2; // Chaque élastique est une ligne avec 2 points
        lineRenderers[1].positionCount = 2;

        // Place les extrémités des élastiques aux points d'attache initiaux
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        // Initialise les oiseaux (généralement en instanciant le premier oiseau)
        birdManager.InitializeBirds();
    }

    private void Update()
    {
        if (isMouseDown) // Si la souris est maintenue enfoncée
        {
            // Récupère la position de la souris en pixels et la convertit en coordonnées monde
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10; // Définir une profondeur pour la conversion en 3D
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Calcule la position limitée de l'élastique en fonction de la longueur max
            currentPosition = centerPosition.position + Vector3.ClampMagnitude(currentPosition - centerPosition.position, maxLenght);

            // Empêche l'élastique d'aller trop bas (évite que l'oiseau ne passe sous une certaine limite)
            currentPosition.y = Mathf.Clamp(currentPosition.y, bottomBoundary, currentPosition.y);

            // Met à jour la position des élastiques
            SetStrips(currentPosition);

            // Ajoutez les offsets ici
            Vector3 offsetPosition = currentPosition + new Vector3(birdPositionOffsetX, birdPositionOffsetY, 0);
            float appliedForce = (centerPosition.position - offsetPosition).magnitude * force;
            // Affiche la trajectoire prévue de l'oiseau
            TrajectoryManager.Instance.DisplayTrajectory(birdManager.GetCurrentBird(), offsetPosition, centerPosition.position, appliedForce);

            // Active le collider de l'oiseau (s'il était désactivé auparavant)
            birdManager.EnableCollider();
        }
        else
        {
            ResetStrips();
            TrajectoryManager.Instance.HideTrajectory();
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true; // Détecte que la souris a été pressée
    }

    private void OnMouseUp()
    {
        isMouseDown = false; // Détecte que la souris a été relâchée
        Shoot(); // Tire l'oiseau
    }

    // Réinitialise la position des élastiques à l'état initial
    private void ResetStrips()
    {
        currentPosition = idlePosition.position; // Remet la position de l'élastique à l'idle
        SetStrips(currentPosition);
    }

    // Met à jour la position des élastiques et la position de l'oiseau
    private void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position); // Déplace le deuxième point de l'élastique gauche
        lineRenderers[1].SetPosition(1, position); // Déplace le deuxième point de l'élastique droit

        // Met à jour la position de l'oiseau avec un décalage défini
        birdManager.UpdateBirdPosition(position, centerPosition.position, birdPositionOffsetX, birdPositionOffsetY);
    }

    // Tire l'oiseau en appliquant la force calculée
    private void Shoot()
    {
        birdManager.Shoot(currentPosition, centerPosition.position, force); // Applique la force de tir à l'oiseau

        slingshotCollider.enabled = false; // Désactive le collider pour éviter les interactions inutiles après le tir

        Invoke("NextBird", 2); // Prépare le prochain oiseau après un délai de 2 secondes
    }

    // Prépare le prochain oiseau après le tir
    private void NextBird()
    {
        birdManager.CreateBird(); // Génère un nouvel oiseau
        slingshotCollider.enabled = true; // Réactive le collider pour le prochain tir
    }
}
