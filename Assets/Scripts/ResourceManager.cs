using System.Collections;
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
        Wood = 0,
        Bricks,
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
        public InventoryDisplay m_inventoryDisplay;

        [System.Serializable]
        public struct ResourceCostProfile
        {
            public ResourceTypes m_type;

            [Tooltip("Positive if adding to stockpile, negative if removing from, ALWAYS.")]
            public int m_delta;

            [Tooltip("Used dependant on context, for whether a resource cost is available, etc.")]
            public bool m_viabilityBool;
        }

        [System.Serializable]
        public class BuildingYieldProfile
        {
            public BuildingTypes m_type;
            public ResourceCostProfile[] m_deltas;
        }
        [SerializeField] BuildingYieldProfile[] m_initialBuildingYields;

        public class RuntimeBuildingYieldProfile
        {
            public BuildingTypes m_type;
            public Dictionary<ResourceTypes, float> m_currentDeltas = new Dictionary<ResourceTypes, float>();
            public Dictionary<ResourceTypes, float> m_maxDeltas = new Dictionary<ResourceTypes, float>();
            public bool m_maxed = true; // Runtime bool for whether a building has all inputs it needs.
        }

        [System.Serializable]
        public class BuildingCostProfile
        {
            public BuildingTypes m_buildingType;
            public ResourceCostProfile[] m_buildingCost;
        }
        [SerializeField] BuildingCostProfile[] m_buildingCosts;

        [SerializeField] int[] m_buildingCounts = new int[BuildingTypeCount];
        List<Building>[] m_activeBuildings = new List<Building>[BuildingTypeCount];

        private class InternalBYP
        {
            public BuildingTypes m_buildingType;
            public int[] m_deltas = new int[ResourceTypeCount];
        }

        InternalBYP[] m_buildingYields = new InternalBYP[BuildingTypeCount];
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
            else { print("Warning! Multiple ResourceManager instances created!"); return; }

            m_trigger = true;

            for (int j = 0; j < BuildingTypeCount; j++)
            {
                m_activeBuildings[j] = new List<Building>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            m_inventoryDisplay = InventoryDisplay.s_instance;

            // Reset building yields, transfer initial to current.
            for (int j = 0; j < BuildingTypeCount; j++)
            {
                m_buildingYields[j] = new InternalBYP();
                m_buildingYields[j].m_buildingType = (BuildingTypes)j;
                {
                    for (int i = 0; i < ResourceTypeCount; i++) { m_buildingYields[j].m_deltas[i] = 0; }
                }
            }

            foreach (var variable in m_initialBuildingYields)
            {
                foreach (var resProf in variable.m_deltas)
                {
                    m_buildingYields[(int)variable.m_type].m_deltas[(int)resProf.m_type] = resProf.m_delta;
                }
            }
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
                        int delta = m_buildingYields[j].m_deltas[i] * m_buildingCounts[j];
                        if (delta > 0) { m_maxPosDeltaR[i] += delta; }
                        else { m_maxNegDeltaR[i] += delta; }
                    }
                }

                // Reset Ratios
                for (int j = 0; j < BuildingTypeCount; j++) { m_buildingActivationRatios[j] = 1.0f; }
                for (int i = 0; i < ResourceTypeCount; i++) { m_resourceIsLimited[i] = false; }

            
                // Repeat until no limitations found.
                bool foundLimiter = true; // Found a resource that will dip below zero this tick.
                for (int index = 0; index < 15 && foundLimiter; index++) {
                    foundLimiter = false;
                    for (int i = 0; i < ResourceTypeCount; i++)
                    {
                        float pos = 0;
                        float neg = 0;
                        for (int j = 0; j < BuildingTypeCount; j++)
                        {
                            float delta = m_buildingYields[j].m_deltas[i] * m_buildingCounts[j] * m_buildingActivationRatios[j];
                            if (delta > 0.0f) { pos += delta; }
                            else { neg -= delta; }
                        }

                        //int deltaFloor = (int)Mathf.Floor(pos - neg);
                        float deltaTotal = pos - neg;
                        //Debug.Log("Resource Check (" + i + "): Neg-" + neg + " Pos-" + pos + " Del-"+ deltaTotal);
                        if (m_resources[i] + deltaTotal >= 0)
                        //if (m_resources[i] + deltaFloor >= 0)
                        {
                            // Got enough, not a limiter.
                            m_DeltaR[i] = (int)Mathf.Ceil(pos - neg);
                        }
                        else
                        {
                            foundLimiter = true;
                            m_DeltaR[i] = -m_resources[i];
                            m_resourceIsLimited[i] = true;

                            float fulfilmentRatio = (m_resources[i] + pos) / neg;
                            for (int j = 0; j < BuildingTypeCount; j++)
                            {
                                if (m_buildingYields[j].m_deltas[i] < 0 && fulfilmentRatio < m_buildingActivationRatios[j])
                                {
                                    m_buildingActivationRatios[j] = fulfilmentRatio;
                                }
                            }

                            Debug.Log("Loop " + index + ", Limited Resource (" + i + "): Neg-" + neg + " Ava-" + (m_resources[i] + pos) + " Rat-" + fulfilmentRatio);
                        }
                    }
                }

                for (int i = 0; i < ResourceTypeCount; i++)
                {
                    m_resources[i] += m_DeltaR[i];
                }
                for (int j = 0; j < BuildingTypeCount; j++)
                {
                    for (int k = 0; k < m_activeBuildings[j].Count; k++) {
                        m_activeBuildings[j][k].SetActivationRatio(m_buildingActivationRatios[j]);
                    }
                }

                m_inventoryDisplay.UpdateValues(m_resources, m_maxPosDeltaR, m_maxNegDeltaR, m_DeltaR, m_resourceIsLimited);

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

        static public int GetResourceCount(ResourceTypes type)
        {
            return s_instance.m_resources[(int)type];
        }

        static public void ChangeResource(ResourceTypes type, int delta)
        {
            s_instance.m_resources[(int)type] = Mathf.Max(0, s_instance.m_resources[(int)type] + delta);
        }

        static public BuildingCostProfile GetCostProfile(BuildingTypes type)
        {

            BuildingCostProfile output = new BuildingCostProfile();
            output.m_buildingType = type;

            bool found = false;
            for (int k = 0; k < s_instance.m_buildingCosts.Length && !found; k++)
            {
                if (s_instance.m_buildingCosts[k].m_buildingType == type)
                {
                    found = true;
                    output.m_buildingCost = new ResourceCostProfile[s_instance.m_buildingCosts[k].m_buildingCost.Length];
                    for (int l = 0; l < s_instance.m_buildingCosts[k].m_buildingCost.Length; l++)
                    {
                        output.m_buildingCost[l] = s_instance.m_buildingCosts[k].m_buildingCost[l];
                        output.m_buildingCost[l].m_viabilityBool = (-output.m_buildingCost[l].m_delta <= GetResourceCount(output.m_buildingCost[l].m_type));
                    }
                }
            }

            return output;
        }

        static public RuntimeBuildingYieldProfile GetCurrentYields(BuildingTypes type)
        {
            RuntimeBuildingYieldProfile output = new RuntimeBuildingYieldProfile();
            output.m_type = type;
            if (s_instance.m_buildingActivationRatios[(int)type] < 1.0f) { output.m_maxed = false; }
            
            for (int i = 0; i < ResourceTypeCount; i++)
            {
                if (s_instance.m_buildingYields[(int)type].m_deltas[i] != 0.0f)
                {
                    output.m_currentDeltas[(ResourceTypes)i] = s_instance.m_buildingYields[(int)type].m_deltas[i] * s_instance.m_buildingActivationRatios[(int)type];
                    output.m_maxDeltas[(ResourceTypes)i] = s_instance.m_buildingYields[(int)type].m_deltas[i];
                }
            }

            return output;
        }
    }
}