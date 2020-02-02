using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class Spawner : MonoBehaviour
    {
        public GameObject m_enemy;
        public GameObject m_sheep;

        bool m_spawnWave = false;
        int m_waveCount = 5;
        
        // Start is called before the first frame update
        void Start()
        {
            for (uint i = 0; i < 250; i++)
            {
                var xxx = Instantiate(m_enemy, new Vector3( Random.Range(-500, 500), 20.0f, Random.Range(-500, 500)), Quaternion.identity) as GameObject;
                xxx = Instantiate(m_sheep, new Vector3( Random.Range(-500, 500), 20.0f, Random.Range(-500, 500)), Quaternion.identity) as GameObject;
            }
            StartCoroutine(WaitForWave());
        }

        // Update is called once per frame
        void Update()
        {
            if (m_spawnWave)
            {
                for (int i = 0; i < m_waveCount; i++)
                {
                    var xxx = Instantiate(m_enemy, new Vector3(Random.Range(-500, 500), 20.0f, Random.Range(-500, 500)), Quaternion.identity) as GameObject;
                    xxx.GetComponent<EnemyController>().MakeAggressive(new Vector3(0.0f, 0.0f, 0.0f));
                }

                m_waveCount += 5;
                StartCoroutine(WaitForWave());
            }
        }

        IEnumerator WaitForWave()
        {
            m_spawnWave = false;
            yield return new WaitForSeconds(300f);
            m_spawnWave = true;
        }
    }
}