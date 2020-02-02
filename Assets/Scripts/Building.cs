using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class Building : MonoBehaviour
    {
        [SerializeField] BuildingTypes m_type;
        bool m_ruined = true;
        float m_activationRatio = 1.0f;

        [SerializeField] bool m_autoActivate = false;

        // Start is called before the first frame update
        void Start()
        {
            if (m_autoActivate) { Repair(); }
        }

        // Update is called once per frame
        void Update()
        {

        }

        // ------------------

        public bool IsRuined() { return m_ruined; }
        public void Ruin()
        {
            ResourceManager.RemoveBuilding(m_type, this);
            m_ruined = true;
        }
        public void Repair()
        {
            ResourceManager.AddBuilding(m_type, this);
            m_ruined = false;
            print("Building repaired.");
        }

        public bool IsFullyActive() { return (m_activationRatio == 1.0f); }
        public float GetActivationRatio() { return m_activationRatio; }
        public void SetActivationRatio(float newActivationRatio) { m_activationRatio = newActivationRatio; }
    }
}