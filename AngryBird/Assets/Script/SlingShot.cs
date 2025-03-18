using UnityEngine;

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
    [SerializeField] private float birdPositionOffset;

    private Vector3 currentPosition;
    private bool isMouseDown;

    private void Start()
    {
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
            TrajectoryManager.Instance.DisplayTrajectory(birdManager.GetCurrentBird(), currentPosition, centerPosition.position, force);

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

        birdManager.UpdateBirdPosition(position, centerPosition.position, birdPositionOffset);
    }

    private void Shoot()
    {
        birdManager.Shoot(currentPosition, centerPosition.position, force);
        slingshotCollider.enabled = false;
        Invoke("NextBird", 2);
    }

    private void NextBird()
    {
        birdManager.CreateBird();
        slingshotCollider.enabled = true;
    }
}
