using UnityEngine;
using System.Collections;
using System;

public class CameraAnimation : MonoBehaviour
{
    public AnimationStep[] animations;
    private Vector3 startPosition;
    private Quaternion startRotation;

    /// <summary>
    /// Plays all camera animations
    /// </summary>
    /// <param name="callback">callback function called when all animations have finished</param>
    public void PlayAnimations(Action callback)
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        StartCoroutine(Animate(0, callback));
    }

    // this will animate the current animation and start the next automatically
    private IEnumerator Animate(int index, Action callback)
    {
        // check if all animations are finished
        if (index >= animations.Length)
        {
            callback.Invoke();
            yield break; // stop
        }

        // init animation
        AnimationStep anim = animations[index];
        float time = 0;
        float t;

        // if no target is set, we just wait for the duration and continue to next animation
        if (anim.target == null)
        {
            yield return new WaitForSeconds(anim.duration);
            StartCoroutine(Animate(index + 1, callback));
            yield break; // stop
        }

        // do the animation
        while (time < anim.duration)
        {
            t = anim.curve.Evaluate(time / anim.duration);
            // lerp position
            if (startPosition != anim.target.position)
                transform.position = Vector3.LerpUnclamped(startPosition, anim.target.position, t);
            // lerp rotation
            if (startRotation != anim.target.rotation)
                transform.rotation = Quaternion.LerpUnclamped(startRotation, anim.target.rotation, t);

            time += Time.deltaTime;
            yield return null; // pause for a frame
        }

        // start next animation
        startPosition = anim.target.position;
        startRotation = anim.target.rotation;
        StartCoroutine(Animate(index + 1, callback));
    }

    [Serializable]
    public class AnimationStep
    {
        [Tooltip("Curve used to control animation. 0 = beginning, 1 = end")]
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Tooltip("The length of the animation in seconds")]
        public float duration = 3f;
        [Tooltip("Target position and rotation the animation will go towards")]
        public Transform target;
    }
}
