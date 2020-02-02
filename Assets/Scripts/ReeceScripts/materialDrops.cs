using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGJ2020
{
    public class materialDrops : MonoBehaviour
    {

        [System.Serializable]
        public struct SpawnOption
        {
            public ResourceTypes m_type;
            public GameObject m_prefab;
            public float m_weight;
        }

        //[Header("Resources")]
        //[Space(5f)]
        //public GameObject steel;
        //public GameObject wood;
        //public GameObject brick;

        //public float steelCounter;
        //public float woodCounter;
        //public float brickCounter;

        public float m_totalWeight = 0.0f;

        [SerializeField]
        public List<SpawnOption> m_options = new List<SpawnOption>();
        [SerializeField]
        int m_minSpawncount;
        [SerializeField]
        int m_maxSpawncount;

        //[Header("Travel targets")]
        //[Space(5f)]
        //[SerializeField]
        //Transform[] spawnTargets;

        void Start()
        {
            foreach (var i in m_options) { m_totalWeight += i.m_weight; }

            //print("Total Weight: " + m_totalWeight);
        }

        public void spawnFromMaterial(Vector3 pos)
        {
            int count = Random.Range(m_minSpawncount, m_maxSpawncount);

            for (int i = 0; i < count; i++)
            {
                float selection = Random.Range(0.0f, m_totalWeight);
                bool found = false;
                float currentProgress = 0.0f;

                for (int j = 0; j < m_options.Count && !found; j++)
                {
                    currentProgress += m_options[j].m_weight;
                    //print("Current Progress: " + currentProgress);
                    if (selection < currentProgress)
                    {
                        found = true;

                        //print("Spawning item of index: " + j);

                        GameObject materialCollect = Instantiate(m_options[j].m_prefab, pos, Quaternion.identity) as GameObject;
                        
                        materialPickup item = materialCollect.GetComponent<materialPickup>();

                        Vector3 dest = new Vector3(Random.Range(-2.0f, 2.0f), 0.25f, Random.Range(-2.0f, 2.0f));
                        dest += transform.position;

                        print("Moving spawned item to: " + dest.x + ", " + dest.y + ", " + dest.z);

                        item.StartMoveToTargetCoroutine(dest, 1, 1);
                    }
                }
            }
        }
    }
}
