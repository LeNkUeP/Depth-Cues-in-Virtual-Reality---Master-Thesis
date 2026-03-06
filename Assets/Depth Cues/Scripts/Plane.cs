using UnityEngine;

public class Plane : AccretionObject
{
    [Header("Plane Settings")]
    public GameObject propeller;

    void RotatePropeller()
    {
        float rotationSpeed = speed * 360f * Time.deltaTime;

        propeller.transform.Rotate(rotationSpeed, 0f, 0f);
    }

    protected override void Update()
    {
        base.Update();
        RotatePropeller();
    }
}
