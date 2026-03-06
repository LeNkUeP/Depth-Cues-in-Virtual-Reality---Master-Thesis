using System.Collections;
using UnityEngine;

public class AccretionSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Vector3 direction = Vector3.right;
    public float speed = 10f;
    public float startMovementDelay = .5f;
    public float restartMovementInterval = 5f;
    public float initialDelay = 3f;
    public GameObject[] objects;

    private void Start()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<AccretionObject>().direction = direction;
            objects[i].GetComponent<AccretionObject>().speed = speed;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        HideAllObjects();
    }

    private IEnumerator SpawnLoop()
    {
        // wait with spawning to control object spawn flow with multiple spawners
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            HideAllObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].GetComponent<AccretionObject>().enabled = true;
                yield return new WaitForSeconds(startMovementDelay);
            }

            yield return new WaitForSeconds(restartMovementInterval);
        }
    }

    private void HideAllObjects()
    {
        foreach (GameObject obj in objects)
        {
            obj.GetComponent<AccretionObject>().enabled = false;
            obj.transform.position = transform.position;
        }
    }
}
