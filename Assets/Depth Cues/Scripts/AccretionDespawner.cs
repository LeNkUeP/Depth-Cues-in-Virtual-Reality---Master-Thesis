using UnityEngine;

public class AccretionDespawner : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("accretionObject"))
        {
            other.gameObject.GetComponent<AccretionObject>().enabled = false;
            other.transform.position = other.transform.parent.position;
        }
    }
}
