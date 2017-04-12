using UnityEngine;
using System.Collections;

public class AutoMover : MonoBehaviour
{
    #region vars
    private bool moving = false;
    public float MovementSpeed = 3f;

    public float BoundRadius = 15f;

    Vector3 startPosition;
    #endregion

    public Vector3 StartPosition{
        set{
            startPosition = value;
        }
    }

    #region init
    IEnumerator Start()
    {
        //first random movement direction based on area around the mover -> looks better
        Vector3 movementDirection = Random.insideUnitCircle.normalized;
        moving = true;

        while (true)
        {
            //move transform
            while (moving)
            {
                transform.localPosition += movementDirection * MovementSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.localPosition, startPosition) >= BoundRadius){
                    // transform.localPosition = (transform.localPosition - startPosition).normalized * BoundRadius;
                    movementDirection = Random.insideUnitCircle.normalized;
                }
                yield return null;
            }

            yield return null;
        }
    }
    #endregion
    void OnBecameInvisible()
    {
        moving = false;
    }

}
