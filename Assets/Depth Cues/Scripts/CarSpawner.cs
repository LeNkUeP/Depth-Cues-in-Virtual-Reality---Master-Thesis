using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject carPrefab;
    public Vector3 direction = Vector3.right;
    public int spawnCount = 4;
    public float spawnDelay = .5f;
    public float spawnInterval = 5f;
    public float initialDelay = 3f;
    public GameObject[] cars;

    private List<GameObject> spawnedCars = new List<GameObject>();

    private void Start()
    {
        carPrefab.GetComponent<Car>().driveDirection = direction;
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        //DestroyAllCars();
        HideAllCars();
    }

    private IEnumerator SpawnLoop()
    {
        // wait with spawning to control car spawn flow with multiple spawners
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                //SpawnCar();
                cars[i].GetComponent<Car>().enabled = true;
                yield return new WaitForSeconds(spawnDelay);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCar()
    {
        if (carPrefab != null)
        {
            spawnedCars.Add(Instantiate(carPrefab, transform.position, carPrefab.transform.rotation));
        }
    }

    private void DestroyAllCars()
    {
        foreach (GameObject car in cars)
        {
            Destroy(car);
        }
        spawnedCars.Clear();
    }

    private void HideAllCars()
    {
        foreach (GameObject car in cars)
        {
            car.GetComponent<Car>().enabled = false;
            car.transform.position = transform.position;
        }
    }

}
