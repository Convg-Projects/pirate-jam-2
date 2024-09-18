using UnityEngine;

public class SpinObject : MonoBehaviour
{
    // Public variables to control the spin speed for each axis
    public float spinSpeedX = 30f;
    public float spinSpeedY = 30f;
    public float spinSpeedZ = 30f;

    // Update is called once per frame
    void Update()
    {
        // Calculate rotation for each axis
        float rotationX = 0;
        float rotationY = spinSpeedY * Time.deltaTime;
        float rotationZ = 0;

        // Apply the rotation to the object
        transform.Rotate(rotationX, rotationY, rotationZ);
    }
}