using UnityEngine;

public class clusterSpin : MonoBehaviour
{
    // Public variables to control the spin speed for each axis
    public float spindSpeedX = 30f;
    public float spindSpeedY = 30f;
    public float spindSpeedZ = 30f;

    // Update is called once per frame
    void Update()
    {
        // Calculate rotation for each axis
        float rotationX = spindSpeedX * Time.deltaTime;
        float rotationY = spindSpeedY * Time.deltaTime;
        float rotationZ = spindSpeedZ * Time.deltaTime;

        // Apply the rotation to the object
        transform.Rotate(rotationX, rotationY, rotationZ);
    }
}