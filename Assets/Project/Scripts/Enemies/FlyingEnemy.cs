using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public class FlyingEnemy : BaseEnemy
{
    [Header("Flying")]
    [SerializeField]
    private float distanceFromFloor;
    [SerializeField]
    private float bounceAmplitude;
    [SerializeField]
    private float bounceSpeed;

    [Header("Chasing")]
    [SerializeField]
    private float detectingRange;
    [SerializeField]
    private float chasingSpeed;
    [SerializeField]
    private float chasingSmoothness;
    [SerializeField]
    private float chasingRange;

    [Header("Attack")]
    [SerializeField]
    private float attackRange = 5f;
    private float bounceBackSpeed = 2f;

    private float bounceAngle;
    private GameObject target;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ChasePlayer();
    }

    private void FixedUpdate()
    {
        FlyBehavior();
    }

    void FlyBehavior()
    {
        if (tno.isMine)
        {
            var newPos = transform.position;
            // set height
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                bounceAngle += Time.deltaTime * bounceSpeed;
                var desiredPosition = hit.point + Vector3.up * distanceFromFloor;
                desiredPosition.y += Mathf.Cos(bounceAngle) * bounceAmplitude;
                if (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
                {
                    desiredPosition.y = target.transform.position.y + 1f; // ground offset
                }
                newPos = Vector3.Lerp(newPos, desiredPosition, Time.deltaTime);

            }
            else
            {
                newPos += Vector3.up;
            }
            tno.Send("SetTargetPosition", Target.AllSaved, newPos);
        }
        transform.position = mTargetPosition;
    }

    void ChasePlayer()
    {
        if (tno.isMine)
        {
            var targetVelocity = Vector3.zero;
            var targetRotation = transform.rotation;
            if (target == null)
            {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectingRange * .5f, Vector3.down);
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        target = hit.collider.gameObject;
                        break;
                    }
                }
            }
            else
            {
                var dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist > chasingRange)
                {
                    target = null;
                }
            }

            if (target != null)
            {
                var dir = (target.transform.position - transform.position).normalized;
                dir.y = 0;

                targetVelocity = dir.normalized; // * chasingSmoothness;
                targetRotation = Quaternion.LookRotation(targetVelocity, Vector3.up);
            }
            tno.Send(nameof(SetTargetRotation), Target.AllSaved, Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * chasingSmoothness));
            tno.Send(nameof(SetTargetVelocity), Target.AllSaved, targetVelocity);
        }
        transform.rotation = mTargetRotation;
        myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, mTargetVelocity, Time.deltaTime * chasingSmoothness);
    }

    private Vector3 mTargetPosition;
    private Quaternion mTargetRotation;
    private Vector3 mTargetVelocity;

    [RFC]
    public void SetTargetVelocity(Vector3 targetVel)
    {
        mTargetVelocity = targetVel;
    }

    [RFC]
    public void SetTargetPosition(Vector3 targetPos)
    {
        mTargetPosition = targetPos;
    }

    [RFC]
    public void SetTargetRotation(Quaternion targetRot)
    {
        mTargetRotation = targetRot;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var damagable = collision.gameObject.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(Damage);
//                collision.gameObject.GetComponent<TNObject>().Send("TakeDamage", Target.AllSaved, Damage);
            }

            myRigidbody.velocity = collision.GetContact(0).normal.normalized * bounceBackSpeed;
        }
    }
}
