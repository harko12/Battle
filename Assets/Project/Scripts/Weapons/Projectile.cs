using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public class Projectile : TNBehaviour
{
    public float speed;
    public float ttl;
    private Rigidbody myRigidbody;
    public Explosion explosionPrefab;

    private float lifetime = 0f;

    protected override void Awake()
    {
        base.Awake();
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 spawn, Vector3 shootDirection)
    {
        transform.forward = shootDirection;
        transform.position = spawn + transform.forward;
        myRigidbody.velocity = shootDirection * speed;
    }

    [RCC]
    public static GameObject FireProjectile(GameObject prefab, Vector3 spawn, Vector3 shootDirection)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        var p = go.GetComponent<Projectile>();
        p.Shoot(spawn, shootDirection);
        return go;
    }

    [RFC]
    private void Explode(Vector3 pos, float range, float damage)
    {
        var e = GameObject.Instantiate<Explosion>(explosionPrefab, pos, Quaternion.identity);
        e.Explode(range, damage);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!tno.isMine) { return; }
        lifetime += Time.deltaTime;
        if (lifetime >= ttl)
        {
            tno.Send("Explode", Target.All, transform.position, 5, 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!tno.isMine) { return; }
        if (other.GetComponent<Pickup>() == null)
        {
            tno.Send("Explode", Target.All, transform.position, 5, 2);
        }
    }
}
