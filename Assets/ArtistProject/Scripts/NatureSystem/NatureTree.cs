using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JingProd.ArtProject.Metaball.Utility;

namespace JingProd.ArtProject{
	public class NatureTree : NatureObject {

		bool isWatered = false;
		[SerializeField] SpriteRenderer m_SpriteRenderer;

		void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.layer == 11){ //water
				if (isWatered)	return;
					
					// NatureTree[] allTrees = transform.parent.GetComponentsInChildren<NatureTree>(true);
					// foreach (NatureTree t in allTrees) t.GrowUp();
				GrowUp();

					// other.gameObject.GetComponent<PlayerRing>().SetToTreeNature();
			}else if(other.gameObject.layer == 9){ //fire
				OnCaughtFire();
			}
		}

		public void GrowUp(){
			if (isWatered) return;
			isWatered = true;
			GetComponent<AutoMover>().BoundRadius += 20;
			AutoScaler script = GetComponent<AutoScaler>();
			script.MinScale += 0.5f;
			script.MaxScale += 0.5f;
			script.CalculateDifference();
		}
		public void OnCaughtFire (){
			GetComponent<Collider2D>().enabled = false;
			GetComponent<AutoMover>().enabled = false;
			GetComponent<AutoScaler>().enabled = false;
			m_SpriteRenderer.DOColor(new Color32(118,73,59,255), 0.5f);
			transform.DOScale(0, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.3f).OnComplete(()=>{
				Destroy(gameObject);
			});
		}
	}
}