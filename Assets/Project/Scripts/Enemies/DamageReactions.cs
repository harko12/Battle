using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageReactions{

    public static IEnumerator ShiftPosition(Transform t, float reactionTime)
    {
        var startRotation = t.rotation;
        var oldEuler = startRotation;
        var newEuler = new Vector3(Random.Range(-1, 1f), Random.Range(-5, 5f), Random.Range(-1, 1f));
        var newRot = oldEuler * Quaternion.Euler(newEuler);
        var elapsedTime = 0f;
        while (elapsedTime <= reactionTime)
        {
            float curveProgress = elapsedTime / reactionTime;
            t.localRotation = Quaternion.Lerp(startRotation, newRot, curveProgress);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        t.rotation = newRot;
        yield return null;
    }

    public static IEnumerator QuickScale(Transform t, float hitSmoothness)
    {
        t.localScale = Vector3.one * .5f;
        while (t.localScale != Vector3.one)
        {
            t.localScale = Vector3.Lerp(t.localScale, Vector3.one, Time.deltaTime * hitSmoothness);
            yield return null;
        }
    }


}
