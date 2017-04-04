using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JingProd.ArtProject{
	public class NatureTree : NatureObject {

		public void OnCaughtFire (){
			Material mat = GetComponent<MeshRenderer>().material;
			mat.DOColor(Color.red,0.5f).OnComplete (()=>{
				transform.DOScale (0, 0.5f).SetEase(Ease.OutQuad).SetDelay(2f).OnComplete(()=>{
					Destroy(gameObject);
				});
			});
		}
	}
}