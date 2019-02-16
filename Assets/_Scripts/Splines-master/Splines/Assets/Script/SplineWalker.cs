using JLProject.Spline;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SplineWalker : MonoBehaviour{
    public enum WalkerMode{
        Once, 
        Loop,
        PingPong
    }

    public WalkerMode mode;
    private bool goingForward = true;

    public bool lookForward;
    public BezierSpline spline;
    public float speed = 0;
    private float _originalSpeed; 
    private float progress;

    public delegate void EndReachedEvent();
    public EndReachedEvent OnEndReached;

    public void Initialize(float startSpeed, BezierSpline path) {
        _originalSpeed = speed = startSpeed;
        spline = path;
        StartCoroutine(WalkForward());
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    public float GetProgress() {
        return progress;
    }

    public Vector3 GetPositionAt(float progress) {
        return spline.GetPoint(progress);
    }

    public void ResetSpeed() {
        speed = _originalSpeed;
    }

    private IEnumerator WalkForward() {
        while(progress < 1f) {
            if (goingForward) {
                progress += Time.deltaTime * speed;
                if (progress > 1f) {
                    if (mode == WalkerMode.Once) {
                        OnEndReached();
                        progress = 1f;
                    }
                    else if (mode == WalkerMode.Loop) {
                        progress -= 1f;
                    }
                    else {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else {
                progress -= Time.deltaTime * speed;
                if (progress < 0) {
                    progress = -progress;
                    goingForward = true;
                }
            }

            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;
            if (lookForward) {
                transform.LookAt(position + spline.GetDirection(progress));
            }

            yield return 0f;
        }
        yield return 0f;
    }

}
