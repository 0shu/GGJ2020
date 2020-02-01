﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public enum BuildingTypes
    {
        Power = 0,
        Water,
        Food,
        People,
        Turret
    }

    public enum ResourceTypes
    {
        Bricks = 0,
        Wood,
        Steel,
        Power,
        Water,
        Food,
        BlindNationalism,
        Firepower
    }

    public class ResourceManager : MonoBehaviour
    {
        public const int BuildingTypeCount = 5;
        public const int ResourceTypeCount = 8;

        public static ResourceManager s_instance;

        [System.Serializable]
        public class BuildTypeToInt
        {
            public BuildingTypes bT;
            public int[] value;
        }
        public BuildTypeToInt[] bti;

        [SerializeField] int[] m_buildingCounts = new int[BuildingTypeCount];
        List<Building>[] m_activeBuildings = new List<Building>[BuildingTypeCount];

        public Dictionary<BuildingTypes, int[]> m_buildingYieldTypes = new Dictionary<BuildingTypes, int[]>();
        float[] m_buildingActivationRatios = new float[BuildingTypeCount]; // What portion of the building's costs/yields are being used/created.

        int[] m_resources       = new int[ResourceTypeCount];
        int[] m_maxPosDeltaR    = new int[ResourceTypeCount];
        int[] m_maxNegDeltaR    = new int[ResourceTypeCount];
        int[] m_DeltaR          = new int[ResourceTypeCount];
        bool[] m_resourceIsLimited = new bool[ResourceTypeCount];

        bool m_trigger;
        public float m_tick;

        void Awake()
        {
            if (s_instance == null) { s_instance = this; }
            else { return; }
            
            m_trigger = false;

            for (int j = 0; j < BuildingTypeCount; j++)
            {
                m_activeBuildings[j] = new List<Building>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach(var variable in bti)
            {
                m_buildingYieldTypes[variable.bT] = variable.value;
            }

            StartCoroutine("ResourceTick");
        }

        // Update is called once per frame
        void Update()
        {
            if (m_trigger == true)
            {
                // Find Maximums
                for (int i = 0; i < ResourceTypeCount; i++)
                {
                    m_maxNegDeltaR[i] = 0;
                    m_maxPosDeltaR[i] = 0;
                    for (int j = 0; j < BuildingTypeCount; j++)
                    {
                        int delta = m_buildingYieldTypes[(BuildingTypes)j][i] * m_buildingCounts[j];
                        if (delta > 0) { m_maxPosDeltaR[i] += delta; }
                        else { m_maxNegDeltaR[i] += delta; }
                    }
                }

                // Reset Ratios
                for (int j = 0; j < BuildingTypeCount; j++) { m_buildingActivationRatios[j] = 1.0f; }
                for (int i = 0; i < ResourceTypeCount; i++) { m_resourceIsLimited[i] = false; }

            
                // Repeat until no limitations found.
                bool foundLimiter = true; // Found a resource that will dip below zero this tick.
                while (foundLimiter) {
                    foundLimiter = false;
                    for (int i = 0; i < ResourceTypeCount; i++)
                    {
                        float pos = 0;
                        float neg = 0;
                        for (int j = 0; j < BuildingTypeCount; j++)
                        {
                            float delta = m_buildingYieldTypes[(BuildingTypes)j][i] * m_buildingCounts[j] * m_buildingActivationRatios[j];
                            if (delta > 0.0f) { pos += delta; }
                            else { neg -= delta; }
                        }
                        
                        int deltaFloor = (int)Mathf.Floor(pos - neg);
                        if (m_resources[i] + deltaFloor >= 0)
                        {
                            // Got enough, not a limiter.
                            m_DeltaR[i] = deltaFloor;
                        }
                        else
                        {
                            foundLimiter = true;
                            m_DeltaR[i] = -m_resources[i];
                            m_resourceIsLimited[i] = true;

                            float fulfilmentRatio = neg / (m_resources[i] + pos);
                            for (int j = 0; j < BuildingTypeCount; j++)
                            {
                                if (m_buildingYieldTypes[(BuildingTypes)j][i] < 0 && fulfilmentRatio < m_buildingActivationRatios[j])
                                {
                                    m_buildingActivationRatios[j] = fulfilmentRatio;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < ResourceTypeCount; i++)
                {
                    m_resources[i] += m_DeltaR[i];
                }
                for (int j = 0; j < BuildingTypeCount; j++)
                {
                    foreach (var building in m_activeBuildings[j]) { building.SetActivationRatio(m_buildingActivationRatios[j]); }
                }

                StartCoroutine("ResourceTick");
            }
        }

        IEnumerator ResourceTick()
        {
            m_trigger = false;
            yield return new WaitForSeconds(m_tick);
            print("Resource Tick Triggered.");
            m_trigger = true;
        }

        // ------------------

        static public void AddBuilding(BuildingTypes type, Building building)
        {
            s_instance.m_buildingCounts[(int)type]++;
            s_instance.m_activeBuildings[(int)type].Add(building);
        }

        static public void RemoveBuilding(BuildingTypes type, Building building)
        {
            s_instance.m_buildingCounts[(int)type]--;
            s_instance.m_activeBuildings[(int)type].Remove(building);
        }
        static public Building GetClosestActiveBuildingTo(Vector3 position)
        {
            bool foundAny = false;
            Building closestYet = null;
            float closestYetDistSq = 100000000000000.0f;

            for (int j = 0; j < BuildingTypeCount; j++)
            {
                foreach (var building in s_instance.m_activeBuildings[j]) {
                    float distSq = (building.transform.position - position).sqrMagnitude;
                    if (distSq < closestYetDistSq)
                    {
                        foundAny = true;
                        closestYetDistSq = distSq;
                        closestYet = building;
                    }
                }
            }

            if (!foundAny) { return null; }
            else { return closestYet; }
        }
    }
}