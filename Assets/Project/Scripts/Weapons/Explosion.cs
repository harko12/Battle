using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public class Explosion : TNBehaviour
{
    public float Range;
    public float Damage;
    public Transform explosionEffect;
    public float explodeTime = 5;
    public float explosionSpeed = 5;

    public void Explode(float range, float damage)
    {
        if (tno.isMine)
        {

            Range = range;
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, range, transform.forward);
            foreach (var hit in hits)
            {
                var destruct = hit.collider.gameObject.GetComponent<IDamagable>();
                if (destruct == null) { destruct = hit.collider.GetComponentInParent<IDamagable>(); }

                // apply damage to destructable object
                if (destruct != null)
                {
                    Debug.LogFormat("{0} taking damage {1}", hit.collider.gameObject.name, damage);
                    destruct.TakeDamage(damage);
                }
            }
        }
        StartCoroutine(ShowEffect());
    }

    public IEnumerator ShowEffect()
    {
        bool grow = true;
        float runTime = 0f;
        while (grow)
        {
            runTime += Time.deltaTime;
            var newScale = (Vector3.one * runTime * explosionSpeed);
            explosionEffect.localScale = newScale;
            if (runTime > explodeTime || newScale.x > Range)
            {
                grow = false;
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, Range * 2);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
