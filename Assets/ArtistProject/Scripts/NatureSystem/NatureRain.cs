using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JingProd.ArtProject.Metaball.Utility;

namespace JingProd.ArtProject{
	public class NatureRain : NatureObject {

		[SerializeField] SpriteRenderer m_SpriteRenderer;
		void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.layer == 10){ //tree
				Terminate();
			}
		}

		void Terminate(){
			GetComponent<Collider2D>().enabled = false;
			GetComponent<AutoMover>().enabled = false;
			GetComponent<AutoScaler>().enabled = false;
			m_SpriteRenderer.DOColor(Color.white, 0.5f);
			transform.DOScale(0, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.3f).OnComplete(()=>{
				Destroy(gameObject);
			});
		}
	}
}