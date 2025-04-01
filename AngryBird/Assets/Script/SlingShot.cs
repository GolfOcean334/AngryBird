using UnityEngine;
using System.Collections.Generic;

public class SlingShot : MonoBehaviour
{
    [Header("Slingshot")]
    [SerializeField] private LineRenderer[] lineRenderers;
    [SerializeField] private Transform[] stripPositions;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private Collider2D slingshotCollider;
    [SerializeField] private float bottomBoundary;
    [SerializeField] private float maxLenght;
    [SerializeField] private float force;

    [Header("Birds")]
    [SerializeField] private BirdManager birdManager;
    [SerializeField] private float birdPositionOffsetX;
    [SerializeField] private float birdPositionOffsetY;

    private Vector3 currentPosition;
    private bool isMouseDown;

    private void Start()
    {
        if (birdManager == null)
        {
            Debug.LogError("BirdManager n'est pas assigné dans l'inspecteur.");
            return;
        }

        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        birdManager.InitializeBirds();
    }

    private void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = centerPosition.position + Vector3.ClampMagnitude(currentPosition - centerPosition.position, maxLenght);
            currentPosition.y = Mathf.Clamp(currentPosition.y, bottomBoundary, currentPosition.y);

            SetStrips(currentPosition);

            Vector3 offsetPosition = currentPosition + new Vector3(birdPositionOffsetX, birdPositionOffsetY, 0);
            float appliedForce = (centerPosition.position - offsetPosition).magnitude * force;
            Vector3 initialVelocity = (centerPosition.position - offsetPosition).normalized * appliedForce;

            // Calculer la trajectoire avec frottement
            Vector3 direction = currentPosition - centerPosition.position;
            float angle = Mathf.Atan2(direction.y, direction.x);
            float length = direction.magnitude;
            List<Vector2> trajectoryPoints = LancerOiseauFrottementRecurrence(angle, length);

            if (TrajectoryManager.Instance != null)
            {
                TrajectoryManager.Instance.DisplayTrajectory(offsetPosition, initialVelocity);
            }

            birdManager.EnableCollider();
        }
        else
        {
            ResetStrips();
            if (TrajectoryManager.Instance != null)
            {
                TrajectoryManager.Instance.HideTrajectory();
            }
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        Shoot();
    }

    private void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    private void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);
        birdManager.UpdateBirdPosition(position, centerPosition.position, birdPositionOffsetX, birdPositionOffsetY);
    }

    private void Shoot()
    {
        if (birdManager.GetCurrentBirdScript() == null)
        {
            Debug.LogError("Aucun oiseau n'est disponible pour être lancé.");
            return;
        }

        // Calcul de la direction du tir basé sur currentPosition
        Vector3 direction = currentPosition - centerPosition.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        float length = direction.magnitude;

        // Calculer la position de lancement en appliquant l'offset
        Vector3 launchPosition = currentPosition + new Vector3(birdPositionOffsetX, birdPositionOffsetY, 0);

        // Calculer la trajectoire prévisualisée à partir de launchPosition
        float appliedForce = (centerPosition.position - launchPosition).magnitude * force;
        Vector3 initialVelocity = (centerPosition.position - launchPosition).normalized * appliedForce;

        // Utiliser LancerOiseauFrottementRecurrence pour calculer la trajectoire avec frottement
        List<Vector2> trajectoryPoints = LancerOiseauFrottementRecurrence(angle, length);

        // Optionnel : ignorer temporairement le collider du slingshot
        Bird currentBird = birdManager.GetCurrentBirdScript();
        if (currentBird != null)
        {
            Physics2D.IgnoreCollision(currentBird.GetComponent<Collider2D>(), slingshotCollider, true);
        }

        // Lancer l'oiseau en passant la position de lancement
        birdManager.Shoot(launchPosition, centerPosition.position, force);

        slingshotCollider.enabled = false;

        Invoke("NextBird", 2);
    }



    private void NextBird()
    {
        birdManager.CreateBird();
        slingshotCollider.enabled = true;
    }

    // Calcul de la vitesse initiale avec frottement (pour l'affichage de la trajectoire)
    private float VitesseInitiale(float alpha, float l1)
    {
        Bird currentBird = birdManager.GetCurrentBirdScript();
        if (currentBird == null)
        {
            Debug.LogError("Aucun oiseau n'est disponible pour calculer la vitesse initiale.");
            return 0;
        }
        float g = currentBird.g;
        float k = currentBird.k;
        float f2 = currentBird.f2;
        float mass = currentBird.mass;
        return l1 * Mathf.Sqrt(k / mass) * Mathf.Sqrt(1 - Mathf.Pow((mass * g * Mathf.Sin(alpha) / (k * l1)), 2));
    }

    // Calcul de la trajectoire avec frottement par récurrence
    private List<Vector2> LancerOiseauFrottementRecurrence(float alpha, float l1)
    {
        // Calculer la vitesse initiale de l'oiseau
        float v0 = VitesseInitiale(alpha, l1);
        float dt = 0.01f; // Intervalle de temps pour chaque étape de la simulation
        float x = 0, y = 0; // Positions initiales
        List<Vector2> positions = new List<Vector2> { new Vector2(0, 0) }; // Liste des positions de la trajectoire
        float vx = v0 * Mathf.Cos(alpha); // Composante x de la vitesse initiale
        float vy = v0 * Mathf.Sin(alpha); // Composante y de la vitesse initiale
        Bird currentBird = birdManager.GetCurrentBirdScript();
        if (currentBird == null)
        {
            Debug.LogError("Aucun oiseau n'est disponible pour calculer la trajectoire.");
            return positions;
        }
        float f2 = currentBird.f2; // Coefficient de frottement
        float g = currentBird.g; // Gravité

        // Boucle pour calculer les positions successives de l'oiseau
        while (y >= 0)
        {
            x += vx * dt; // Mettre à jour la position x
            y += vy * dt; // Mettre à jour la position y
            positions.Add(new Vector2(x, y)); // Ajouter la nouvelle position à la liste
            vx += -f2 * vx * dt; // Mettre à jour la vitesse x en tenant compte du frottement
            vy += -(g + f2 * vy) * dt; // Mettre à jour la vitesse y en tenant compte de la gravité et du frottement
        }
        return positions; // Retourner la liste des positions de la trajectoire
    }
}
