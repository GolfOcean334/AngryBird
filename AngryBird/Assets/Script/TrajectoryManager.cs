using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    public static TrajectoryManager Instance { get; private set; }

    [Header("Trajectory")]
    [SerializeField] private GameObject trajectoryPointPrefab;
    [SerializeField] private int trajectoryPointCount = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    private List<GameObject> trajectoryPoints = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitializeTrajectoryPoints();
    }

    private void InitializeTrajectoryPoints()
    {
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            GameObject point = Instantiate(trajectoryPointPrefab);
            point.SetActive(false);
            trajectoryPoints.Add(point);
        }
    }

    public void DisplayTrajectory(Rigidbody2D bird, Vector3 currentPosition, Vector3 centerPosition, float force)
    {
        if (bird == null) return;

        Vector3 initialVelocity = (currentPosition - centerPosition) * force * -1;
        Vector3 currentPos = bird.transform.position;
        Vector3 currentVelocity = initialVelocity;

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            trajectoryPoints[i].transform.position = currentPos;
            trajectoryPoints[i].SetActive(true);
            currentPos += currentVelocity * trajectoryTimeStep;
            currentVelocity += (Vector3)Physics2D.gravity * trajectoryTimeStep;
        }
    }

    public void HideTrajectory()
    {
        foreach (var point in trajectoryPoints)
        {
            point.SetActive(false);
        }
    }
}
