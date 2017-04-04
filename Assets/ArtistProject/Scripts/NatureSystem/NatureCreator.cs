﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JingProd.ArtProject{
	public class NatureCreator : MonoBehaviour {

		static MyNatureType SelectedNatureType;

		[SerializeField] PlayerMovementController m_Player;

		[SerializeField] Transform m_FirePrefab;
		[SerializeField] Transform[] m_TreePrefabs;
		[SerializeField] Transform m_WaterPrefab;
		

		public void OnSelectFire (bool selected){
			if (selected)
				SelectedNatureType = MyNatureType.Fire;
		}
		
		public void OnSelectTree (bool selected){
			if (selected)
				SelectedNatureType = MyNatureType.Tree;
		}

		public void OnSelectWater (bool selected){
			if (selected)
				SelectedNatureType = MyNatureType.Water;
		}

		public void LayNatureElement (BaseEventData data){
			// if (!MyPrepared)	return;
			if (SelectedNatureType == MyNatureType.Fire)
				GenerateFire ();
			else if (SelectedNatureType == MyNatureType.Tree)
				GenerateTree();
			else if (SelectedNatureType == MyNatureType.Water)
				GenerateRain();
		}

		public void Release (BaseEventData data){
			// MyPrepared = false;
			if (SelectedNatureType == MyNatureType.Water){
				RemoveRain();
			}
		}

		void GenerateFire (){
			Transform temp = Instantiate(m_FirePrefab);
			temp.SetParent (transform, false);
			Vector3 initPos = m_Player.transform.position;
			temp.position = initPos;
			// temp.position = new Vector3 (initPos.x, initPos.y + 1, initPos.z);
			// temp.GetComponent<Rigidbody>().AddForce (Vector3.up * 5f, ForceMode.Impulse);
		}

		void GenerateTree (){
			Transform temp = Instantiate(m_TreePrefabs[Random.Range(0,m_TreePrefabs.Length)]);
			temp.SetParent (transform, false);
			Vector3 initPos = m_Player.transform.position;
			temp.position = initPos;
			// temp.position = new Vector3 (initPos.x, initPos.y + 1, initPos.z);
			// temp.GetComponent<Rigidbody>().AddForce (Vector3.up * 5f, ForceMode.Impulse);
		}

		void GenerateRain (){
			if (NatureRain.Instance != null){
				NatureRain.Instance.gameObject.SetActive(true);
				NatureRain.Instance.transform.SetParent(m_Player.transform.GetChild(1), false);
				// NatureRain.Instance.GetComponent<ParticleSystem>().enableEmission = true;
				return;
			}

			Transform temp = Instantiate(m_WaterPrefab);
			temp.SetParent (m_Player.transform.GetChild(1), false);
			temp.localPosition = Vector3.zero;
			NatureRain.Instance = temp.GetComponent<NatureRain>();
			// NatureRain.Instance.GetComponent<ParticleSystem>().enableEmission = true;
			// temp.position = new Vector3 (landpos.x, temp.position.y, landpos.z);
		}

		void RemoveRain (){
			if (NatureRain.Instance != null){
				Destroy(NatureRain.Instance.gameObject);
			}
		}

	}
}