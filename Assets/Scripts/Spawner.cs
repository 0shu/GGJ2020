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
        int m_waveCount = 1; // How many flocks to spawn.
        
        // Start is called before the first frame update
        void Start()
        {
            // Random strays:
            for (uint i = 0; i < 50; i++)
            {
                Instantiate(m_enemy, new Vector3( Random.Range(-500, 500), 50.0f, Random.Range(-500, 500)), Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
                Instantiate(m_sheep, new Vector3( Random.Range(-500, 500), 50.0f, Random.Range(-500, 500)), Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
            }

            // Mixed flocks
            for (uint i = 0; i < 65; i++)
            {
                // TODO: Update to level size.
                Vector3 position = new Vector3(Random.Range(-500, 500), 0.0f, Random.Range(-500, 500));
                // TODO: Change with getting height of relevant tile.
                position.y = 30.0f;
                SpawnFlock(position);
            }

            // Extrajudicial paramilitary death squads.
            for (uint i = 0; i < 10; i++)
            {
                // TODO: Update to level size.
                Vector3 position = new Vector3(Random.Range(-500, 500), 0.0f, Random.Range(-500, 500));
                // TODO: Change with getting height of relevant tile.
                position.y = 30.0f;
                SpawnFlock(position, true);
            }

            StartCoroutine(WaitForWave());
        }

        // Update is called once per frame
        void Update()
        {
            if (m_spawnWave)
            {
                float radius = ResourceManager.GetFurthestActiveBuildingRadius() + 32.0f;
                
                for (int i = 0; i < m_waveCount; i++)
                {
                    float angle = Random.Range(0.0f, 2.0f * Mathf.PI);
                    Vector3 position = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
                    position *= radius;
                    // TODO: Change with getting height of relevant tile.
                    position.y = 30.0f;

                    SpawnFlock(position, true, new Vector3(0.0f, 0.0f, 0.0f));
                }

                m_waveCount += 1;
                StartCoroutine(WaitForWave());
            }
        }

        IEnumerator WaitForWave()
        {
            m_spawnWave = false;
            yield return new WaitForSeconds(300f);
            m_spawnWave = true;
        }

        void SpawnFlock(Vector3 roughPosition) { SpawnFlock(roughPosition, false, false, new Vector3()); }
        void SpawnFlock(Vector3 roughPosition, bool allEnemies) { SpawnFlock(roughPosition, allEnemies, false, new Vector3()); }
        void SpawnFlock(Vector3 roughPosition, bool allEnemies, Vector3 aggressionDestination) { SpawnFlock(roughPosition, allEnemies, true, aggressionDestination); }
        void SpawnFlock(Vector3 roughPosition, bool allEnemies, bool autoAggression, Vector3 aggressionDestination)
        {
            int redCount, whiteCount;
            if (!allEnemies) {
                whiteCount = Random.Range(5, 9);
                redCount = Random.Range(2, 6);
            }
            else {
                whiteCount = 0;
                redCount = Random.Range(5, 8);
            }

            for (uint i = 0; i < whiteCount; i++) {
                Instantiate(m_sheep, roughPosition + new Vector3(Random.Range(-5, 6), 24.0f, Random.Range(-5, 6)), Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
            }

            if (autoAggression)
            {
                for (uint i = 0; i < redCount; i++) {
                    var e = Instantiate(m_enemy, roughPosition + new Vector3(Random.Range(-5, 6), 24.0f, Random.Range(-5, 6)), Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f)) as GameObject;
                    e.GetComponent<EnemyController>().MakeAggressive(aggressionDestination);
                }
            }
            else {
                for (uint i = 0; i < redCount; i++) {
                    Instantiate(m_enemy, roughPosition + new Vector3(Random.Range(-5, 6), 24.0f, Random.Range(-5, 6)), Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
                }
            }
        }
    }
}