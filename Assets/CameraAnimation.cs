using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CameraAnimation : MonoBehaviour
{
    [Tooltip("The start position and rotation for when the first animation begins")]
    public Transform start;
    public AnimationStep[] animations;


    // Use this for initialization
    void Start()
    {
        PlayAnimations(() =>
        {
            print("Animations are over");
        });
    }

    /// <summary>
    /// Plays all camera animations
    /// </summary>
    /// <param name="callback">callback function called when all animations have finished</param>
    public void PlayAnimations(Action callback)
    {
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
            yield break;
        }

        // do the animation
        while (time < anim.duration)
        {
            t = anim.curve.Evaluate(time / anim.duration);
            // lerp position
            if (start.position != anim.target.position)
                transform.position = Vector3.LerpUnclamped(start.position, anim.target.position, t);
            // lerp rotation
            if (start.rotation != anim.target.rotation)
                transform.rotation = Quaternion.LerpUnclamped(start.rotation, anim.target.rotation, t);

            time += Time.deltaTime;
            yield return null; // pause for a frame
        }

        // start next animation
        start = anim.target;
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
