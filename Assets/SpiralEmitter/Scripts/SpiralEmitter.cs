using UnityEngine;
using System.Collections;

public class SpiralEmitter : MonoBehaviour {
    public enum Tweens
    {
        LINEAR,
        EASE_IN,
        EASE_OUT
        //CURVE
    }

    public bool play = true;
    public GameObject particle;
    public float spawnRate = 10.0f;
    public float spawnEndTime = 3.0f;
    public bool spawnEndUnlimited = false;
    public bool emitterPositionConstraint = true;
    public bool emitterOrientationStartSync = true;
    public bool emitterOrientationConstraint = true;
    public bool inheritScale = false;

    // life and death
    public Vector2 lifeMinMax = new Vector2(0.5f, 1.0f);
    // initDelay에 대해서
    // Trail 은 최초 생성된 상태에서 즉시 방향으 바꿔도 튀는 문제가 있어서 diabled 상태로 생성되는 시간을 지정해줘야함.
    // Trail 길이와 최소 마디 생성 간격값에 의해 가변적으로 조절해줘야한다. 최소 마디 생성 간격은 스크립트로 접근할 수 없어서 프리팹에서 직접 숫자를 조절해줘야함
    public float initDelay = 0.03f; 
    public float destroyDelay = 1.0f;

    // size
    public Vector2 sizeHeadMinMax = new Vector2(0.1f, 0.3f);
    public float sizeTailRatio = 1.0f;    // Tail only

    // Length (trail only)
    public Vector2 trailTimeMinMax = new Vector2(0.3f, 1.0f);

    // Move
    public Vector3 initSpeedMin = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 initSpeedMax = new Vector3(0.0f, 5.0f, 0.0f);
    public Vector3 endSpeedMin = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 endSpeedMax = new Vector3(0.0f, 0.0f, 0.0f);
    public Tweens speedTweenType;

    // Orientation
    public Vector3 initOrientationMin = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 initOrientationMax = new Vector3(0.0f, 360.0f, 0.0f);

    // Rotate
    public bool randomReverseRotate = false;
    public Vector3 initRotateMin = new Vector3(0.0f, 500.0f, 0.0f);
    public Vector3 initRotateMax = new Vector3(0.0f, 800.0f, 0.0f);
    public Vector3 endRotateMin = new Vector3(0.0f, 500.0f, 0.0f);
    public Vector3 endRotateMax = new Vector3(0.0f, 800.0f, 0.0f);
    public Tweens rotateTweenType;


    // Distance
    public Vector2 distanceBirthMinMax = new Vector2(1.0f, 2.0f);
    public Vector2 distanceDeathMinMax = new Vector2(0.0f, 0.2f);
    public Tweens distanceTweenType;

    // Ziggle Move
    public bool useZiggleMove = false;
    public Vector3 ziggleMoveDirection = new Vector3(1, 1, 1);
    public float ziggleMoveSpeed = 0.5f;
    public float ziggleMoveRadius = 0.05f;
    public float ziggleMoveStiffness = 0.1f;

    // private
    private Transform meTransform;
    private bool playBeforeFrame;   // 이전 update 에서 play 상태 기억
    private float playStartTime;
    private int spawnedCount = 0;
    
    // Use this for initialization
    void Start () {
        meTransform = transform;
        playBeforeFrame = play;
        playStartTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        // Play 와 Stop 처리 시작 ============================================
        // play가 아니였다가 play 상태로 된 경우
        if (play && play != playBeforeFrame)
        {
            playStartTime = Time.time;
            spawnedCount = 0;
        }

        // play 상황이 아니면 그냥 리턴
        if (!play)
        {
            playBeforeFrame = play;
            return;
        }

        // play가 정해진 시간을 넘어서면 자동 stop 처리
        if ( (( Time.time - playStartTime) > spawnEndTime) && !spawnEndUnlimited )
        {
            play = false;
        }

        playBeforeFrame = play;
        // Play 와 Stop 처리 끝 ============================================

        // Spawn 시작
        // play 시작순간부터 현재 시간까지 전부 몇 개 스폰되었어야하는지 총 량 계산
        int totalSpawnCount = Mathf.FloorToInt( (Time.time - playStartTime) / (1.0f / spawnRate) ) + 1;
        // 총 스폰되었어야 할 카운트에서 그동안 스폰된 카운트를 빼면 현재 스폰해야할 카운트가 계산됨
        int nowSpawnCount = totalSpawnCount - spawnedCount;
        if (nowSpawnCount != 0)
        {
            for ( int i = 0; i < nowSpawnCount; i++)
            {
                // 초기화
                GameObject newParticle = (GameObject.Instantiate(particle, meTransform.position, meTransform.localRotation)) as GameObject;
                if (inheritScale)
                {
                    newParticle.transform.localScale = meTransform.localScale;
                }
                SpiralParticle spiralParticle = newParticle.GetComponent<SpiralParticle>();
                spiralParticle.emitter = meTransform;
                spiralParticle.emitterPositionConstraint = emitterPositionConstraint;
                spiralParticle.emitterOrientationConstraint = emitterOrientationConstraint;
                SpiralRotate spiralRotate = newParticle.transform.GetComponentInChildren<SpiralRotate>();
                if (!spiralParticle)    // 필요 컴포넌트가 없는 경우 그냥 리턴
                {
                    return;
                }

                // Life 설정
                spiralParticle.life = Random.Range(lifeMinMax.x, lifeMinMax.y);
                spiralParticle.destroyDelay = destroyDelay;
                spiralParticle.initDelay = initDelay;

                if (emitterOrientationStartSync)
                {
                    newParticle.transform.rotation = meTransform.rotation;
                }

                // 초기 방향 설정
                Transform spiralRotateObj = spiralRotate.transform;
                spiralRotateObj.localEulerAngles = new Vector3(Random.Range(initOrientationMin.x, initOrientationMax.x), Random.Range(initOrientationMin.y, initOrientationMax.y), Random.Range(initOrientationMin.z, initOrientationMax.z));

                // 이동
                spiralParticle.initSpeed = new Vector3(Random.Range(initSpeedMin.x, initSpeedMax.x), Random.Range(initSpeedMin.y, initSpeedMax.y), Random.Range(initSpeedMin.z, initSpeedMax.z));
                spiralParticle.endSpeed = new Vector3(Random.Range(endSpeedMin.x, endSpeedMax.x), Random.Range(endSpeedMin.y, endSpeedMax.y), Random.Range(endSpeedMin.z, endSpeedMax.z));
                spiralParticle.speedTweenType = (SpiralParticle.Tweens)((int)speedTweenType);

                // 회전
                int randomDir = 1;
                if (randomReverseRotate)
                {
                    if (Random.value > 0.5f)
                    {
                        randomDir = -1;
                    }
                }
                spiralParticle.initRotate = new Vector3(Random.Range(initRotateMin.x, initRotateMax.x) * randomDir, Random.Range(initRotateMin.y, initRotateMax.y) * randomDir, Random.Range(initRotateMin.z, initRotateMax.z) * randomDir);
                spiralParticle.endRotate = new Vector3(Random.Range(endRotateMin.x, endRotateMax.x) * randomDir, Random.Range(endRotateMin.y, endRotateMax.y) * randomDir, Random.Range(endRotateMin.z, endRotateMax.z) * randomDir);
                spiralParticle.rotateTweenType = (SpiralParticle.Tweens)((int)rotateTweenType);

                // Distance (반경)
                spiralParticle.distanceBirth = Random.Range(distanceBirthMinMax.x, distanceBirthMinMax.y);
                spiralParticle.distanceDeath = Random.Range(distanceDeathMinMax.x, distanceDeathMinMax.y);
                spiralParticle.distanceTweenType = (SpiralParticle.Tweens)((int)distanceTweenType);

                // Trail 처리
                TrailRenderer trailRenderer = newParticle.transform.GetComponentInChildren<TrailRenderer>();
                if (trailRenderer)
                {
                    // Length
                    trailRenderer.time = Random.Range(trailTimeMinMax.x, trailTimeMinMax.y);

                    // Width
                    trailRenderer.startWidth = Random.Range(sizeHeadMinMax.x, sizeHeadMinMax.y);
                    trailRenderer.endWidth = trailRenderer.startWidth * sizeTailRatio;
                }

                // Ziggle Move
                if (useZiggleMove)
                {
                    SpiralZiggleMove spiralZiggleMove = spiralRotateObj.gameObject.GetComponent<SpiralZiggleMove>();
                    
                    // ZiggleMove 컴포넌트가 없으면 생성
                    if (!spiralZiggleMove)
                    {
                        spiralZiggleMove = spiralRotateObj.gameObject.AddComponent<SpiralZiggleMove>();
                    }

                    spiralZiggleMove.direction = ziggleMoveDirection;
                    spiralZiggleMove.speed = ziggleMoveSpeed;
                    spiralZiggleMove.radius = ziggleMoveRadius;
                    spiralZiggleMove.stiffness = ziggleMoveStiffness;
                }
            }
            spawnedCount = totalSpawnCount;
        }
    }
}
