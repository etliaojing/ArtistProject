using UnityEngine;
using System.Collections;

public class SpiralParticle : MonoBehaviour
{
    public enum Tweens
    {
        LINEAR,
        EASE_IN,
        EASE_OUT
        //CURVE
    }

    public Transform emitter;

    public bool emitterPositionConstraint = true;
    public bool emitterOrientationConstraint = true;

    public float life = 1.0f;
    public float initDelay = 0.03f;
    public float destroyDelay = 1.0f;

    // Move
    public Vector3 initSpeed = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 endSpeed = new Vector3(0.0f, 0.0f, 0.0f);
    public Tweens speedTweenType;
    public Vector3 movSpd = new Vector3(0, 0, 0);

    // Rotate
    public Vector3 initRotate = new Vector3(0, 100.0f, 0);
    public Vector3 endRotate = new Vector3(0, 500.0f, 0);
    public Tweens rotateTweenType;

    // 반경
    public float distanceBirth = 1.0f;
    public float distanceDeath = 0.5f;
    public Tweens distanceTweenType;

    public float deltaDebug;

    private SpiralRotate spiralRotate;
    private float birthTime;
    private TrailRenderer trailRenderer;
    private float trailOriginStartWidth;
    private float trailOriginEndWidth;
    private Transform meTransform;
    private Transform distanceEnd;
    private int frameCounter = 0;
    private Vector3 beforeEmitterPos;

    // Use this for initialization
    void Start () {
        meTransform = transform;
        birthTime = Time.time;

        spiralRotate = GetComponent<SpiralRotate>();
        if (!spiralRotate)
        {
            spiralRotate = meTransform.GetComponentInChildren<SpiralRotate>();
        }
        distanceEnd = spiralRotate.transform.GetChild(0);
        distanceEnd.localPosition = new Vector3(0, 0, distanceBirth);

        trailRenderer = GetComponent<TrailRenderer>();
        if (!trailRenderer)
        {
            trailRenderer = meTransform.GetComponentInChildren<TrailRenderer>();
        }
        if (trailRenderer)
        {
            trailOriginStartWidth = trailRenderer.startWidth;
            trailOriginEndWidth = trailRenderer.endWidth;
        }

        // 첫 생성시 튀는 것을 막기 위해 일단 비활성으로 시작
        trailRenderer.enabled = false;

        if (emitter)
            beforeEmitterPos = emitter.position;
    }
	
	// Update is called once per frame
	void Update () {
        float age = Time.time - birthTime;
        float delta = age / (life + destroyDelay);
        deltaDebug = delta;

        // Orientation Constraint
        if (emitterOrientationConstraint)
        {
            if (emitter)
                meTransform.rotation = emitter.rotation;
        }

        // Move
        switch (speedTweenType)
        {
            case Tweens.LINEAR:
                movSpd = Vector3.Lerp(initSpeed, endSpeed, delta);
                break;
            case Tweens.EASE_IN:
                movSpd = EaseInV3(initSpeed, endSpeed, delta);
                break;
            case Tweens.EASE_OUT:
                movSpd = EaseOutV3(initSpeed, endSpeed, delta);
                break;
            default:
                break;
        }
        if (emitter)
        {
            if (emitterPositionConstraint)
            {
                Vector3 nowEmitterDeltaPos = emitter.position - beforeEmitterPos;
                meTransform.Translate(nowEmitterDeltaPos + emitter.TransformDirection(movSpd * Time.smoothDeltaTime), Space.World);
            }
            else
            {
                meTransform.Translate(emitter.TransformDirection(movSpd * Time.smoothDeltaTime), Space.World);
            }
        }

        // Rotate
        if (spiralRotate)
        {
            switch (rotateTweenType)
            {
                case Tweens.LINEAR:
                    spiralRotate.rotSpd = Vector3.Lerp(initRotate, endRotate, delta);
                    break;
                case Tweens.EASE_IN:
                    spiralRotate.rotSpd = EaseInV3(initRotate, endRotate, delta);
                    break;
                case Tweens.EASE_OUT:
                    spiralRotate.rotSpd = EaseOutV3(initRotate, endRotate, delta);
                    break;
                default:
                    break;
            }

            // 반경 변화
            switch (distanceTweenType)
            {
                case Tweens.LINEAR:
                    distanceEnd.localPosition = new Vector3(0, 0, Mathf.Lerp(distanceBirth, distanceDeath, delta));
                    break;
                case Tweens.EASE_IN:
                    distanceEnd.localPosition = new Vector3(0, 0, EaseIn(distanceBirth, distanceDeath, delta));
                    break;
                case Tweens.EASE_OUT:
                    distanceEnd.localPosition = new Vector3(0, 0, EaseOut(distanceBirth, distanceDeath, delta));
                    break;
                default:
                    break;
            }
        }


        // trail 이 있는 경우
        if (trailRenderer)
        {
            if (age > initDelay)
            {
                trailRenderer.enabled = true;
            }

            if (age > life)
            {
                float bias = (age - life) / destroyDelay;
                trailRenderer.startWidth = Mathf.Lerp(trailOriginStartWidth, 0.0f, bias);
                trailRenderer.endWidth = Mathf.Lerp(trailOriginEndWidth, 0.0f, bias);
            }
        }

        // Destroy
        if ( age > (life + destroyDelay))
        {
            GameObject.Destroy(meTransform.gameObject);
        }

        frameCounter++;

        if (emitter)
            beforeEmitterPos = emitter.position;
    } //update end========================================

    /*
    // 리니어는 굳이 함수로 안 써도 될듯
    float Linear(float start, float end, float delta)
    {
        return Mathf.Lerp(start, end, delta);
    }
    */

    float EaseIn(float start, float end, float delta)
    {
        return Mathf.Lerp(start, end, delta * delta);
    }

    Vector3 EaseInV3(Vector3 start, Vector3 end, float delta)
    {
        return Vector3.Lerp(start, end, delta * delta);
    }

    float EaseOut(float start, float end, float delta)
    {
        return Mathf.Lerp(start, end, (delta - (delta * delta)) + delta);
    }

    Vector3 EaseOutV3(Vector3 start, Vector3 end, float delta)
    {
        return Vector3.Lerp(start, end, (delta - (delta * delta)) + delta);
    }
}
