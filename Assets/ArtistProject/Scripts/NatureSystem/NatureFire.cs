using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JingProd.ArtProject{
	public class NatureFire : NatureObject {

		[SerializeField] SpriteRenderer m_SpriteRenderer;

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == 8){ //player
				if (NatureCreator.SelectedNatureType == MyNatureType.Water){
					Terminate();
				}
				else if (NatureCreator.SelectedNatureType == MyNatureType.Tree){
					other.gameObject.GetComponent<PlayerRing>().SetToBurnedNature();
					NatureCreator.SelectedNatureType = MyNatureType.Burned;
				}
			}
		}

		void Terminate(){
			GetComponent<Collider2D>().enabled = false;
			m_SpriteRenderer.DOColor(Color.white, 0.5f);
			transform.DOScale(0, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.3f).OnComplete(()=>{
				Destroy(gameObject);
			});
		}
	}
}