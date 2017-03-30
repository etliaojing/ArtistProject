using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JingProd.ArtProject{
	public class PlayerMovementController : MonoBehaviour {

		public Vector2 Movement{
			set{
				mMovement = value;
			}
		}

		public Vector3 LandingPoint{
			get{
				RaycastHit info;
				if (Physics.Raycast (transform.position, -transform.up, out info, 1000)){
					if (info.collider.CompareTag("Terrain")){
						return info.point;
					}
				}
				return Vector3.zero;
			}
		}

		public float MovementSpeed = 5f;
		public float FloatingHeight = 100;
		public float UpdateDelay = 0.2f;

		float targetHeight, currentVelocity;

		Vector2 mMovement;

		void Start () {
			StartCoroutine(UpdateFloatingHeight());
		}
		
		void Update () {
			transform.position += new Vector3 (mMovement.x, 0, mMovement.y) * MovementSpeed * Time.deltaTime;
			float h = transform.position.y;
			h = Mathf.SmoothDamp (h, targetHeight, ref currentVelocity, 0.2f);
			transform.position = new Vector3 (transform.position.x, h, transform.position.z);
		}

		IEnumerator UpdateFloatingHeight (){
			while (true) {
				RaycastHit info;
				if (Physics.Raycast (transform.position, -transform.up, out info, 1000)){
					if (info.collider.CompareTag("Terrain")){
						targetHeight = info.point.y + FloatingHeight;
					}
				}
				yield return new WaitForSeconds(UpdateDelay);
			}
		}
	}
}