using UnityEngine;

public class Car : AccretionObject
{
    [Header("Car Settings")]
    public GameObject[] wheels;

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

    protected override void Update()
    {
        base.Update();
        RotateWheels();
    }
}
