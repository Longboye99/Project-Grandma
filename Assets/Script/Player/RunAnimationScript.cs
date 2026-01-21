using UnityEngine;
using UnityEngine.Splines;

public class RunAnimationScript : MonoBehaviour
{
    SplineAnimate splineAnimate;

    private void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();

    }

    private void TransitionIn()
    {
        //turn head down, lerp to first knot
    }

    //play spline animation, headbop, walking sound effect

    private void TransitionOut()
    {
        //Turn head up, lerp to cam position
    }
}