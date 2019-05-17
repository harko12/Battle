using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public float ttl = 1f;
    private float _timeAlive = Mathf.Infinity;
    private MeshRenderer myRenderer;
    private bool showing;

    private void Awake()
    {
        myRenderer = GetComponentInChildren<MeshRenderer>();
        myRenderer.enabled = false;
    }

    private void Start()
    {
        _timeAlive = 0;
        myRenderer.enabled = true;
        showing = true;
    }

    private void Die()
    {
        myRenderer.enabled = false;
        _timeAlive = Mathf.Infinity;
        showing = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (showing)
        {
            _timeAlive += Time.deltaTime;
            if (_timeAlive >= ttl)
            {
                Die();
            }
        }
    }
}
