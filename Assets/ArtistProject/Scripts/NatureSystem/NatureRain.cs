using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JingProd.ArtProject{
	public class NatureRain : NatureObject {

		[SerializeField] SpriteRenderer m_SpriteRenderer;
		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == 8){ //player
				if (NatureCreator.SelectedNatureType == MyNatureType.Fire){
					other.gameObject.GetComponent<PlayerRing>().SetToNoneNature();
					NatureCreator.SelectedNatureType = MyNatureType.None;
				}
				else if (NatureCreator.SelectedNatureType == MyNatureType.Tree){
					Terminate();
					other.gameObject.GetComponent<PlayerRing>().GrowUp();
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