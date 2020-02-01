using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGJ2020
{
    public class materialDrops : MonoBehaviour
    {

        [Header("Resources")]
        [Space(5f)]
        public GameObject steel;
        public GameObject wood;
        public GameObject brick;

        public float steelCounter;
        public float woodCounter;
        public float brickCounter;

        List<GameObject> materialPickup = new List<GameObject>();

        [Header("Travel targets")]
        [Space(5f)]
        [SerializeField]
        Transform[] spawnTargets;

        void Start()
        {
            for(int i = 0; i < steelCounter; i++)
            {
                materialPickup.Add(steel);
            }

            for (int i = 0; i < woodCounter; i++)
            {
                materialPickup.Add(wood);
            }

            for (int i = 0; i < brickCounter; i++)
            {
                materialPickup.Add(brick);
            }
        }

        public void spawnFromMaterial(Vector3 pos)
        {
            for(int i = 0; i < spawnTargets.Length; i++)
            {
                int num = Random.Range(0, materialPickup.Count);
                GameObject materialCollect = Instantiate(materialPickup[num], pos, Quaternion.identity) as GameObject;

                materialPickup item = materialCollect.GetComponent<materialPickup>();
                StartCoroutine(item.MoveToTarget(spawnTargets[i].position, 1, 1));
            }
            
        }
    }
}
