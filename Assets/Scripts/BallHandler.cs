using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] float spawnDelay;
    [SerializeField] float detachDelay;
    [SerializeField] float deletDelay = 3f;
    GameObject ballInstance;



    SpringJoint2D currentBallSpringJoint;
    Rigidbody2D currentBallRB;

    private Camera mainCamera;

    private bool isThrowed = false;
    private bool isDragging;

    private void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();

    }
    void Update()
    {
        if (currentBallRB == null) { return; }
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchBall();
            }
            isDragging = false;

            return;
        }
        isDragging = true;
        currentBallRB.isKinematic = true;
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRB.position = worldPosition;


    }

    private void SpawnNewBall()
    {
        ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRB = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;

    }
    IEnumerator DestroyBall(GameObject ball)
    {

        yield return new WaitForSeconds(deletDelay);

        if (isThrowed)
        {
            Destroy(ball);
            isThrowed = false;
        }

    }



    void LaunchBall()
    {
        currentBallRB.isKinematic = false;
        currentBallRB = null;
        Invoke("DetachBall", detachDelay);
        StartCoroutine(DestroyBall(ballInstance));

    }
    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;
        isThrowed = true;
        Invoke(nameof(SpawnNewBall), spawnDelay);
    }
}
