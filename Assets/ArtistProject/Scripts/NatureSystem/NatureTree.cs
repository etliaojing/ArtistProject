using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JingProd.ArtProject.Metaball.Utility;

namespace JingProd.ArtProject{
	public class NatureTree : NatureObject {

		bool isWatered = false;
		[SerializeField] SpriteRenderer m_SpriteRenderer;

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == 8){ //player
				if (NatureCreator.SelectedNatureType == MyNatureType.Water){
					if (isWatered)	return;
					
					NatureTree[] allTrees = transform.parent.GetComponentsInChildren<NatureTree>(true);
					foreach (NatureTree t in allTrees) t.GrowUp();

					other.gameObject.GetComponent<PlayerRing>().SetToTreeNature();
					NatureCreator.SelectedNatureType = MyNatureType.Tree;
				}
				else if (NatureCreator.SelectedNatureType == MyNatureType.Fire){
					OnCaughtFire();
				}
			}
		}

		public void GrowUp(){
			if (isWatered) return;
			isWatered = true;
			GetComponent<AutoMover>().BoundRadius += 80;
			AutoScaler script = GetComponent<AutoScaler>();
			script.MinScale += 10f;
			script.MaxScale += 10f;
			script.CalculateDifference();
		}
		public void OnCaughtFire (){
			GetComponent<Collider2D>().enabled = false;
			m_SpriteRenderer.DOColor(new Color32(118,73,59,255), 0.5f);
		}
	}
}