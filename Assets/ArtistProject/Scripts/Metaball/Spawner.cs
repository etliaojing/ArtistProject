using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JingProd.ArtProject.Metaball{
    public class Spawner : MonoBehaviour
    {
        #region vars
        public GameObject MetaBallPrefab;
        public int Amount = 100;

        public float Radius = 100f;
        #endregion
        #region init
        private void Start()
        {
            Initialise();
        }

        protected virtual void Initialise(){
            for (int i = 0; i < Amount; i++)
            {
                //create metaball in random position
                var ball = Instantiate(MetaBallPrefab) as GameObject;
                ball.transform.SetParent(transform,false);
                ball.transform.localPosition = Radius * Random.insideUnitCircle;

                //randomize movementspeed
                var mover = ball.GetComponent<AutoMover>();
                mover.MovementSpeed = Random.Range(5f, 20f);
                mover.StartPosition = ball.transform.localPosition;
            }
        }
        #endregion
    }
}