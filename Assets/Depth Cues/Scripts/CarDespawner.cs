using UnityEngine;

public class CarDespawner : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("car"))
        {
            //Destroy(other.gameObject);
            other.gameObject.GetComponent<Car>().enabled = false;
            other.transform.position = other.transform.parent.position;
        }
    }
}
