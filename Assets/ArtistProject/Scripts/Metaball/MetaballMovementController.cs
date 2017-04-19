using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JingProd.ArtProject.Metaball{
	public class MetaballMovementController : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {

		public static bool IsMoving = false;
		public float SmoothedDampDuration = 2f;
		public float SpawnMinDistance = 10f;

		[SerializeField] Spawner m_Spawner;


		Vector3 targetPosition, v;
		bool isPressed;
		float accMovingDistance;

		void Update(){
			if (isPressed){
				Vector3 pos1 = transform.position;
				transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref v, SmoothedDampDuration);
				float diff = Vector3.Distance(pos1, transform.position);
				accMovingDistance += diff;
				if (accMovingDistance >= SpawnMinDistance){
					m_Spawner.Emit(new Vector2(transform.position.x, transform.position.y));
					accMovingDistance = 0;
				}
				// Vector3 dir = (targetPosition - transform.parent.position).normalized;
				// transform.parent.position += new Vector3(dir.x, dir.y, 0)*MovementSpeed;
			}
		}
		public void OnBeginDrag(PointerEventData eventData){
			if (IsMoving) return;
			IsMoving = true;
			isPressed = true;
			targetPosition = transform.position;
			accMovingDistance = 0;
		}

		public void OnDrag(PointerEventData eventData){
			if (!isPressed) return;
			if (Input.touchCount > 1) return;
			targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,0));
			targetPosition.z = 0;
		} 

		public void OnEndDrag(PointerEventData eventData){
			Debug.Log("EndDrag!");
			isPressed = false;
			IsMoving = false;
		}
	}
}