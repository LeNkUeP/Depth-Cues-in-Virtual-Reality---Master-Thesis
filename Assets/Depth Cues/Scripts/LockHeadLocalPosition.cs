using UnityEngine;

public class LockHeadLocalPosition : MonoBehaviour
{
    Vector3 lockedPos;

    void Start()
    {
        lockedPos = transform.localPosition;
    }

    void LateUpdate()
    {
        transform.localPosition = lockedPos;
    }
}
