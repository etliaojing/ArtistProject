using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JingProd.ArtProject.Metaball{
    public class Spawner : MonoBehaviour
    {
        #region vars
        public GameObject InitialSpawningPlace;
        public GameObject MetaBallPrefab;
        public int Amount = 100;

        public float Radius = 100f;

        public int EmitMinMetaball = 2;
        public int EmitMaxMetaball = 5;
        public float EmitMinScale = 0.5f;
        public float EmitMaxScale = 1f;
        public float EmitRandomness = 0.5f;
        public float EmitRadius = 10f;

        #endregion
        #region init
        private void Start()
        {
            Initialise();
        }

        protected virtual void Initialise(){
            if (InitialSpawningPlace == null)   
                return;

            for (int i = 0; i < Amount; i++)
            {
                //create metaball in random position
                var ball = Instantiate(MetaBallPrefab) as GameObject;
                ball.transform.SetParent(transform,false);
                ball.transform.position = new Vector2 (InitialSpawningPlace.transform.position.x, InitialSpawningPlace.transform.position.y) + Radius * Random.insideUnitCircle;

                //randomize movementspeed
                var mover = ball.GetComponent<AutoMover>();
                mover.MovementSpeed = Random.Range(5f, 20f);
                mover.StartPosition = ball.transform.localPosition;
            }
        }

        public virtual void Emit(Vector2 atPosition){
            int emitNum = Random.Range(EmitMinMetaball, EmitMaxMetaball);
            
            for(int i=0; i<emitNum; i++){
                var ball = Instantiate(MetaBallPrefab) as GameObject;
                ball.transform.SetParent(transform,false);
                ball.transform.localScale = Random.Range(EmitMinScale, EmitMaxScale) * Vector3.one;
                ball.transform.position = atPosition + Random.insideUnitCircle * EmitRandomness * EmitRadius;

                //randomize movementspeed
                var mover = ball.GetComponent<AutoMover>();
                mover.MovementSpeed = Random.Range(5f, 20f);
                mover.StartPosition = ball.transform.localPosition;
            }
        }
        #endregion
    }
}