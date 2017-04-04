using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JingProd.ArtProject{
	public class NatureFire : NatureObject {

		void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.layer == 9){
				if (other.gameObject.GetComponent<NatureObject>().NatureType == MyNatureType.Tree){
					other.gameObject.GetComponent<NatureTree>().OnCaughtFire();
					transform.DOScale (0, 0.5f).SetEase (Ease.OutQuad).OnComplete (()=>{
						Destroy (gameObject);
					});
				}
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == 9){
				if (other.gameObject.GetComponent<NatureObject>().NatureType == MyNatureType.Water){
					Terminate();
				}
			}
		}

		void Terminate(){
			transform.DOScale(0, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f).OnComplete(()=>{
				Destroy(gameObject);
			});
		}
	}
}