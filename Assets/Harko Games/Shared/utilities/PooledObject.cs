using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarkoGames.Pooling
{
    public class PooledObject : MonoBehaviour
    {
        private float spawnedTime;
        public ObjectPoolPO myPool;
        public float TTL = 1f;

        private void OnEnable()
        {
            spawnedTime = Time.time;
        }

        private void Update()
        {
            if (Time.time >= spawnedTime + TTL)
            {
                myPool.ReturnObject(this);
            }
        }
    }
}