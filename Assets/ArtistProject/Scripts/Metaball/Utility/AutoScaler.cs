using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JingProd.ArtProject.Metaball.Utility{
	public class AutoScaler : MonoBehaviour {

		public float MaxScale = 1.5f, MinScale = 0.7f;

		float differenceS;
		float randomStartValue;

		
		void Start () {
			randomStartValue = Random.value * Mathf.PI;
			CalculateDifference();
		}
		
		void Update () {
			transform.localScale = Vector3.one * (MinScale + Mathf.Sin(randomStartValue + Time.time) * differenceS);
		}

		public void CalculateDifference(){
			differenceS = MaxScale - MinScale;
		}
	}
}
