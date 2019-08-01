using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cooldown",menuName = "Cooldown")]
public class Cooldown : ScriptableObject
{
    public float CooldownTime;
    public float Progress;
    public bool IsRunning;

    private float runTime = 0;

    public void Reset()
    {
        Progress = 0;
        runTime = 0;
        IsRunning = false;
    }

    public IEnumerator Run()
    {
        Reset();
        IsRunning = true;
        while (Progress <= 1f)
        {
            Tick();
            yield return new WaitForFixedUpdate();
        }
        IsRunning = false;
    }

    public void Tick()
    {
        runTime += Time.deltaTime;
        Progress = runTime / CooldownTime;
    }
}


public class Cooldown_counter
{
    public float CooldownTime;
    public float Progress;
    public bool IsRunning;
    private System.Action StopCallback;
    private float runTime = 0;

    public Cooldown_counter(System.Action stopCallback = null)
    {
        StopCallback = stopCallback;
    }

    public void Reset()
    {
        Progress = 0;
        runTime = 0;
        IsRunning = false;
    }

    public void Start(float cdTime, System.Action callback = null)
    {
        Reset();
        CooldownTime = cdTime;
        IsRunning = true;
        StopCallback = callback;
    }

    public void Stop()
    {
        IsRunning = false;
        if (StopCallback != null)
        {
            StopCallback.Invoke();
        }
    }

    public void Tick(float deltaTime)
    {
        if (!IsRunning) return;
        runTime += deltaTime;
        Progress = runTime / CooldownTime;
        if (Progress >= 1f)
        {
            Stop();
        }
    }
}
