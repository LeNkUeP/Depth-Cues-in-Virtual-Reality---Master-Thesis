using UnityEngine;

public class AccretionObject : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 direction = Vector3.right;
    public float speed = 10f;

    private bool isMoving = false;

    void Start()
    {
        isMoving = true;
    }

    protected virtual void Update()
    {
        if (isMoving)
        {
            MoveObject();
        }
    }

    void MoveObject()
    {
        transform.Translate(speed * Time.deltaTime * direction.normalized, Space.World);
    }
}
