using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JingProd.ArtProject{

	public enum ArtNatureType{
		Water,
		Tree,
		Fire
	}
	public class NatureCreator : MonoBehaviour {

		static ArtNatureType SelectedNatureType;

		[SerializeField] PlayerMovementController m_Player;

		[SerializeField] Transform m_FirePrefab;
		[SerializeField] Transform m_TreePrefab;
		[SerializeField] Transform m_WaterPrefab;
		

		public void OnSelectFire (bool selected){
			if (selected)
				SelectedNatureType = ArtNatureType.Fire;
		}
		
		public void OnSelectTree (bool selected){
			if (selected)
				SelectedNatureType = ArtNatureType.Tree;
		}

		public void OnSelectWater (bool selected){
			if (selected)
				SelectedNatureType = ArtNatureType.Water;
		}

		public void LayNatureElement (){
			if (SelectedNatureType == ArtNatureType.Fire)
				GenerateFire ();
			else if (SelectedNatureType == ArtNatureType.Tree)
				GenerateTree();
			else if (SelectedNatureType == ArtNatureType.Water)
				GenerateWater();
		}

		void GenerateFire (){
			Transform temp = Instantiate(m_FirePrefab);
			temp.SetParent (transform, false);
			Vector3 initPos = m_Player.transform.position;
			temp.position = new Vector3 (initPos.x, initPos.y + 1, initPos.z);
			temp.GetComponent<Rigidbody>().AddForce (Vector3.up * 5f, ForceMode.Impulse);
		}

		void GenerateTree (){
			Transform temp = Instantiate(m_TreePrefab);
			temp.SetParent (transform, false);
			Vector3 initPos = m_Player.transform.position;
			temp.position = new Vector3 (initPos.x, initPos.y + 1, initPos.z);
			temp.GetComponent<Rigidbody>().AddForce (Vector3.up * 5f, ForceMode.Impulse);
		}

		void GenerateWater (){
			Transform temp = Instantiate(m_WaterPrefab);
			temp.SetParent (transform, false);
			Vector3 landpos = m_Player.LandingPoint;
			temp.position = new Vector3 (landpos.x, temp.position.y, landpos.z);
		}

	}
}