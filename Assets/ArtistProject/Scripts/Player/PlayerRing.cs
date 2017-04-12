using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JingProd.ArtProject{
	public class PlayerRing : MonoBehaviour {

		[SerializeField] SpriteRenderer m_SpriteRenderer;

		float myOriginalScale = 30f;
		float myCurrentScale = 30f;
		Tween _t_Scale;
		Tween _t_Color;

		public void GrowUp(){
			if(_t_Scale!=null) _t_Scale.Kill();
			myCurrentScale += 0.02f * myOriginalScale;
			_t_Scale = transform.DOScale(myCurrentScale, 0.2f).SetEase(Ease.OutBack);
		}
		public void SetToNoneNature(){
			if (_t_Color!=null)	_t_Color.Kill();
			_t_Color = m_SpriteRenderer.DOColor (Color.white, 0.3f);
		}

		public void SetToBurnedNature(){
			if (_t_Color!=null)	_t_Color.Kill();
			_t_Color = m_SpriteRenderer.DOColor (new Color32(118,73,59,255), 0.3f);
		}

		public void SetToFireNature(){
			if (_t_Color!=null)	_t_Color.Kill();
			_t_Color = m_SpriteRenderer.DOColor (Color.red, 0.3f);
		}

		public void SetToWaterNature(){
			if (_t_Color!=null)	_t_Color.Kill();
			_t_Color = m_SpriteRenderer.DOColor (Color.blue, 0.3f);
		}

		public void SetToTreeNature(){
			if (_t_Color!=null)	_t_Color.Kill();
			_t_Color = m_SpriteRenderer.DOColor (Color.green, 0.3f);
		}
	}
}
