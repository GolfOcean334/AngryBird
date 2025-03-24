using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

            if (TrajectoryManager.Instance != null)
            {
                TrajectoryManager.Instance.DisplayTrajectory(birdManager.GetCurrentBird(), offsetPosition, centerPosition.position, appliedForce);
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
        if (birdManager.GetCurrentBird() == null)
        {
            Debug.LogError("Aucun oiseau n'est disponible pour être lancé.");
            return;
        }

        Vector3 direction = currentPosition - centerPosition.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        float length = direction.magnitude;

        List<Vector2> trajectory = LancerOiseauFrottementRecurrence(angle, length);

        // Appliquer la première position de la trajectoire à l'oiseau
        birdManager.Shoot(currentPosition, centerPosition.position, force);

        // Appliquer les positions suivantes de la trajectoire à l'oiseau
        StartCoroutine(ApplyTrajectory(trajectory));

        slingshotCollider.enabled = false;

        Invoke("NextBird", 2);
    }

    private IEnumerator ApplyTrajectory(List<Vector2> trajectory)
    {
        Rigidbody2D bird = birdManager.GetCurrentBird();
        foreach (Vector2 position in trajectory)
        {
            bird.position = position;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void NextBird()
    {
        birdManager.CreateBird();
        slingshotCollider.enabled = true;
    }

    // Fonction pour calculer la vitesse initiale
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

    // Fonction pour calculer la trajectoire avec frottement par récurrence
    private List<Vector2> LancerOiseauFrottementRecurrence(float alpha, float l1)
    {
        float v0 = VitesseInitiale(alpha, l1);
        float dt = 0.01f; // Pas de temps (plus petit = plus précis)
        float x = 0, y = 0; // Position initiale
        List<Vector2> positions = new List<Vector2> { new Vector2(0, 0) }; // Liste des positions
        float vx = v0 * Mathf.Cos(alpha); // Vitesse initiale - coordonnée x
        float vy = v0 * Mathf.Sin(alpha); // Vitesse initiale - coordonnée y

        Bird currentBird = birdManager.GetCurrentBirdScript();
        if (currentBird == null)
        {
            Debug.LogError("Aucun oiseau n'est disponible pour calculer la trajectoire.");
            return positions;
        }

        float f2 = currentBird.f2;
        float g = currentBird.g;

        while (y >= 0) // Tant que l'oiseau n'a pas touché le sol
        {
            x += vx * dt; // Mise à jour de la position horizontale
            y += vy * dt; // Mise à jour de la position verticale
            positions.Add(new Vector2(x, y)); // Stockage des positions
            vx += -f2 * vx * dt; // Mise à jour de la vitesse horizontale
            vy += -(g + f2 * vy) * dt; // Mise à jour de la vitesse verticale
        }

        return positions;
    }
}
