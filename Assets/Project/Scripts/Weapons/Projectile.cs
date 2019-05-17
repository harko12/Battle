using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float ttl;
    private Rigidbody myRigidbody;
    public Explosion explosionPrefab;

    private float lifetime = 0f;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 spawn, Vector3 shootDirection)
    {
        transform.forward = shootDirection;
        transform.position = spawn + transform.forward;
        myRigidbody.velocity = shootDirection * speed;
    }

    private void Explode()
    {
        var e = GameObject.Instantiate<Explosion>(explosionPrefab, transform.position, Quaternion.identity);
        e.Explode(5, 2);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= ttl)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }
}
