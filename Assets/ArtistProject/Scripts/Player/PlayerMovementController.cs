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

		public float MovementSpeed = 5f;

		Vector2 mMovement;
		
		void Update () {
			#if UNITY_EDITOR
			float moveX = Input.GetAxis("Horizontal");
			float moveY = Input.GetAxis("Vertical");
			transform.localPosition += new Vector3 (moveX, moveY, 0) * 150 * MovementSpeed * Time.deltaTime;
			#endif
			transform.localPosition += new Vector3 (mMovement.x,mMovement.y, 0) * MovementSpeed * Time.deltaTime;
		}
	}
}