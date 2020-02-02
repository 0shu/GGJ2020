using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020 {
    public class Plinth : MonoBehaviour
    {
        [SerializeField]
        GameObject m_buildingPrefab;

        [System.Serializable]
        public class SpawnWeight
        {
            public BuildingTypes m_type;
            public float m_weight;
        }
        [SerializeField]
        List<SpawnWeight> m_options;

        float m_totalWeight = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            foreach (var i in m_options) { m_totalWeight += i.m_weight; }

            float selection = Random.Range(0.0f, m_totalWeight);
            bool found = false;
            float currentProgress = 0.0f;

            for (int j = 0; j < m_options.Count && !found; j++)
            {
                currentProgress += m_options[j].m_weight;
                if (selection < currentProgress)
                {
                    found = true;
                    GameObject building = Instantiate(m_buildingPrefab, transform.position + new Vector3(0.0f, 5.0f, 0.0f), transform.rotationr) as GameObject;

                    Building comp = building.GetComponent<Building>();
                    comp.ChangeBuildingType(m_options[j].m_type);
                }
            }

            GameObject.Destroy(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
