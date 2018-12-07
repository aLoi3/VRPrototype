using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform target;

    public Vector3 center;
    public Vector3 size;

    public int maxUnits = 4;
    public float spawnTime = 2.0f;

    private int units;
    private float timer;

	// Use this for initialization
	void Start ()
    {
        units = 0;
        timer = spawnTime;

        //InvokeRepeating("SpawnEnemy", spawnTime, spawnTime);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SpawnEnemy();

        timer -= Time.deltaTime;
        if(timer <= 0.0f && units < maxUnits)
        {
            SpawnEnemy();
            timer = spawnTime;
            units++;
        }
	}

    public void SpawnEnemy()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

        GameObject clone;
        clone = Instantiate(enemyPrefab, pos, Quaternion.identity) as GameObject;
        //clone.transform.LookAt(target);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
