using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Behaviour : MonoBehaviour {

    public GameObject target;
    public ParticleSystem particle;
    public List<ParticleCollisionEvent> collisionEvents;

    public GameObject enemy;

    public float speed = 2.0f;


    // Use this for initialization
    void Start ()
    {
        particle = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(target.GetComponent<Transform>());

        if(Vector3.Distance(transform.position, target.GetComponent<Transform>().position) <= 25.0f)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
	}

    private void OnParticleCollision(GameObject other)
    {
        if(other.tag == "beam")
        {
            Destroy(gameObject);
        }
    }
}
