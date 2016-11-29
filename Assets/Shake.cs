using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour {

    public AnimationCurve dampCurve;
    public float duration = 1.0f;
    public float magnitude = 1.0f;
    public const float timePerImpulse = 0.05f;

    void Start()
    {
        StartCoroutine(StartShake());
    }

    IEnumerator StartShake()
    {
        yield return new WaitForSeconds(0.5f); 
        float totalTime = 0.0f;
        Vector3 startPos = transform.localPosition;

        while (totalTime < duration)
        {
            totalTime += Time.deltaTime;

            float factor = magnitude * dampCurve.Evaluate(totalTime / duration);
            float x = (Random.value * 2.0f - 1.0f) * factor;
            float y = (Random.value * 2.0f - 1.0f) * factor;
            Vector3 oldPos = transform.localPosition;
            Vector3 newPos = startPos + new Vector3(x, y, 0);
            float time = 0;
            

            while (time < timePerImpulse)
            {
                totalTime += Time.deltaTime;
                time += Time.deltaTime;
                float t = time / timePerImpulse;
                transform.localPosition = Vector3.Lerp(oldPos, newPos, t);
                yield return null;
            }
        }

        transform.position = startPos;
    }
}
