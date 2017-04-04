using UnityEngine;
using System.Collections;

public class SpiralZiggleMove : MonoBehaviour {

    public Vector3 direction = new Vector3 (1, 1, 1);
    public float speed = 0.5f;
    public float radius = 0.05f;
    public float stiffness = 0.1f;

    private Vector3 spdVec;
    private Vector3 localPosBackup;
    private Vector3 beforeVec;
    private Transform meTransform;

	// Use this for initialization
	void Start () {
        meTransform = transform;
        spdVec = GetRandomVec();
        localPosBackup = meTransform.localPosition;
        beforeVec = new Vector3(0, 0, 0);
        
        // 예외를 피하기 위해 clamp
        Mathf.Clamp(stiffness, 0.0f, 1.0f);
        direction = Vector3.ClampMagnitude(direction, Vector3.Magnitude(new Vector3(1, 1, 1)));
    }
	
	// Update is called once per frame
	void Update () {
	    if (radius <= 0.0f)
        {
            return;
        }

        Vector3 nowVec = (beforeVec * (1.0f - stiffness)) + (spdVec * Time.smoothDeltaTime);
        meTransform.localPosition = meTransform.localPosition + nowVec;
        beforeVec = nowVec;

        // 지정된 반경보다 멀리 가면 새로 랜덤 세팅
        if (Vector3.Magnitude(meTransform.localPosition - localPosBackup) > radius)
        {
            spdVec = GetRandomVec();
        }
	}

    public Vector3 GetRandomVec()
    {
        Vector3 randomVec = new Vector3(
            Random.Range(direction.x * -1.0f, direction.x),
            Random.Range(direction.y * -1.0f, direction.y),
            Random.Range(direction.z * -1.0f, direction.z)
            ) * radius;
        Vector3 dirVec = randomVec - (meTransform.localPosition - localPosBackup);
        return (Vector3.Normalize(dirVec) * speed);
    }
}
