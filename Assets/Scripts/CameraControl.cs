using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float mSpeed = 10.0f;
    public float mInitalSpeed = 10.0f;
    public float mSpeedMultipler = 3.0f;

    private void Start()
    {
        mInitalSpeed = mSpeed;
    }
    void Update()
    {
        // Get input for horizontal and vertical movement
        float horizontal = Input.GetAxis("Horizontal"); // WSAD left/right
        float forwardBackward = Input.GetAxis("Vertical"); // WSAD forward/backward

        // Get input for vertical movement (Q and E keys)
        float vertical = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            vertical = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            vertical = 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            mSpeed *= mSpeedMultipler;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            mSpeed = mInitalSpeed;
        }

        // Create movement vector
        Vector3 movement = new Vector3(horizontal, 0, forwardBackward) * mSpeed * Time.deltaTime;
        Vector3 verticalMovement = new Vector3(0, vertical, 0) * mSpeed * Time.deltaTime;

        // Apply movement to the camera
        transform.Translate(movement + verticalMovement, Space.World);
    }
}
