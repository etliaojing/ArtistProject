using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JingProd.ArtProject{
	public class DPadController : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler{

		public float MaxDistance;

		[SerializeField] PlayerMovementController m_Player;
		[SerializeField] RectTransform m_DragPlane;

		public void OnBeginDrag(PointerEventData eventData){
			Debug.Log ("Drag Start");
		}

		public void OnDrag(PointerEventData eventData){
			Vector2 mousePosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle (m_DragPlane, eventData.position, eventData.pressEventCamera, out mousePosition)){
				float distance = mousePosition.magnitude;
				Vector2 dir = mousePosition.normalized;
				if (distance >= MaxDistance)	mousePosition = dir * MaxDistance;
				// Debug.Log ("Dragging... "+mousePosition +", dir :"+dir+ ", distance : "+distance);	
				transform.GetComponent<RectTransform>().anchoredPosition = mousePosition;
				m_Player.Movement = mousePosition;
			}
		}

		public void OnEndDrag(PointerEventData eventData){
			Debug.Log ("Drag End");
			transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			m_Player.Movement = Vector2.zero;
		}
	}
}