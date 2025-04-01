using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    // Instance unique du TrajectoryManager (singleton)
    public static TrajectoryManager Instance { get; private set; }

    [Header("Trajectory")]
    [SerializeField] private GameObject trajectoryPointPrefab; // Préfabriqué représentant un point de la trajectoire
    [SerializeField] private int trajectoryPointCount = 30; // Nombre de points affichés pour représenter la trajectoire
    [SerializeField] private float trajectoryTimeStep = 0.1f; // Intervalle de temps entre chaque point de la trajectoire

    private List<GameObject> trajectoryPoints = new List<GameObject>(); // Liste contenant les points de la trajectoire

    private void Awake()
    {
        // Implémentation du pattern Singleton pour s'assurer qu'une seule instance de TrajectoryManager existe
        if (Instance == null)
        {
            Instance = this;
        }
        InitializeTrajectoryPoints();
    }

    // Initialise et instancie les points de la trajectoire
    private void InitializeTrajectoryPoints()
    {
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            GameObject point = Instantiate(trajectoryPointPrefab); // Crée un point à partir du préfabriqué
            point.SetActive(false); // Désactive le point au départ
            trajectoryPoints.Add(point); // Ajoute le point à la liste
        }
    }

    public void DisplayTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3 currentPos = startPosition; // Position de départ de la trajectoire
        Vector3 currentVelocity = initialVelocity; // Stocke la vitesse actuelle de l'oiseau

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            trajectoryPoints[i].transform.position = currentPos; // Met à jour la position du point
            trajectoryPoints[i].SetActive(true); // Active le point pour l'afficher
            currentPos += currentVelocity * trajectoryTimeStep; // Mise à jour de la position en fonction de la vitesse
            currentVelocity += (Vector3)Physics2D.gravity * trajectoryTimeStep; // Applique la gravité sur la vitesse
        }
    }

    // Cache la trajectoire en désactivant tous les points
    public void HideTrajectory()
    {
        foreach (var point in trajectoryPoints)
        {
            point.SetActive(false);
        }
    }
}
