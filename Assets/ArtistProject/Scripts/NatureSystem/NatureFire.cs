using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JingProd.ArtProject.Metaball.Utility;

namespace JingProd.ArtProject{
	public class NatureFire : NatureObject {

		[SerializeField] Transform m_TerminatePrefab;

		void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.layer == 11){ //nature water
				Terminate();
			}
		}

		void Terminate(){
			GetComponent<Collider2D>().enabled = false;
			GetComponent<AutoMover>().enabled = false;
			GetComponent<AutoScaler>().enabled = false;
			Transform whiteFire = Instantiate(m_TerminatePrefab);
			whiteFire.SetParent(transform, false);
			whiteFire.localScale = Vector3.one;
			whiteFire.GetComponent<SpriteRenderer>().DOFade(1, 0.5f);
			transform.DOScale(0, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.3f).OnComplete(()=>{
				Destroy(gameObject);
			});
		}
	}
}