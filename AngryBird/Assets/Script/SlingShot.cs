using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    [SerializeField] private LineRenderer[] lineRenderers;
    [SerializeField] private Transform[] stripPositions;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;

    [SerializeField] private Collider2D slingshotCollider;

    [SerializeField] private Vector3 currentPosition;

    [SerializeField] private float maxLenght;

    [SerializeField] private float bottomBoundary;

    [SerializeField] private GameObject birdPefab;
    Rigidbody2D bird;
    Collider2D birdCollider;
    [SerializeField] private float birdPositionOffset;

    [SerializeField] private float force;

    bool isMouseDown;
    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        CreateBird();
    }

    private void CreateBird()
    {
        bird = Instantiate(birdPefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;

        bird.isKinematic = true;

        ResetStrips();
        slingshotCollider.enabled = true;
    }
    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = centerPosition.position + Vector3.ClampMagnitude(currentPosition - centerPosition.position, maxLenght);

            currentPosition.y = Mathf.Clamp(currentPosition.y, bottomBoundary, currentPosition.y);

            SetStrips(currentPosition);

            if (birdCollider)
            {
                birdCollider.enabled = true;
            }
        }
        else
        {
            ResetStrips();
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

    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);
        if (bird)
        {
            Vector3 direction = position - centerPosition.position;
            bird.transform.position = position + direction.normalized * birdPositionOffset;
            bird.transform.right = -direction.normalized;
        }
    }

    void Shoot()
    {
        bird.isKinematic = false;
        Vector3 birdForce = (currentPosition - centerPosition.position) * force * -1;
        bird.velocity = birdForce;

        bird = null;
        birdCollider = null; 
        slingshotCollider.enabled = false;
        Invoke("CreateBird", 2);
    }
}
