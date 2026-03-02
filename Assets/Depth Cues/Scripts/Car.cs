using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("General")]
    public GameObject[] wheels;

    [Header("Driving Settings")]
    public Vector3 driveDirection = Vector3.right;
    public float speed = 10f;

    private bool isDriving = false;

    void Start()
    {
        Drive();
    }

    void Update()
    {
        if (isDriving)
        {
            MoveCar();
            RotateWheels();
        }
    }

    public void Drive()
    {
        isDriving = true;
    }

    void MoveCar()
    {
        transform.Translate(speed * Time.deltaTime * driveDirection.normalized, Space.World);
    }

    void RotateWheels()
    {
        float rotationSpeed = speed * 360f * Time.deltaTime;

        foreach (GameObject wheel in wheels)
        {
            if (wheel != null)
            {
                wheel.transform.Rotate(rotationSpeed, 0f, 0f);
            }
        }
    }
}
