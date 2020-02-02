using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class Building : resource
    {
        [SerializeField] BuildingTypes m_type;
        bool m_ruined = true;
        float m_activationRatio = 1.0f;

        [SerializeField] bool m_autoActivate = false;

        [System.Serializable]
        public struct MeshPair
        {
            public BuildingTypes m_type;
            public GameObject m_ruinedMesh;
            public GameObject m_repairedMesh;
        }
        [SerializeField]
        MeshPair[] m_meshes;

        int m_modelIndex = -1;
        GameObject m_currentModel;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            Setup();
            if (m_autoActivate) { Repair(); }
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            if (m_uiActive)
            {
                if (m_ruined && player.currentTool == repairTool)
                {
                    UIConstructor uic = m_currentUIWrapper.GetComponent<UIConstructor>();
                    uic.Flush();

                    var cp = ResourceManager.GetCostProfile(m_type);
                    foreach (var res in cp.m_buildingCost)
                    {
                        Color col = res.m_viabilityBool ? Color.black : Color.red;
                        uic.AddSlot(res.m_type, res.m_delta.ToString(), col);
                    }
                }
                else if (!m_ruined)
                {
                    UIConstructor uic = m_currentUIWrapper.GetComponent<UIConstructor>();
                    uic.Flush();


                    var cy = ResourceManager.GetCurrentYields(m_type);
                    Color col = cy.m_maxed ? Color.black : Color.red;
                    for (int i = 0; i < ResourceManager.ResourceTypeCount; i++)
                    {
                        ResourceTypes resType = (ResourceTypes)i;
                        if (cy.m_maxDeltas.ContainsKey(resType))
                        {
                            if (cy.m_maxDeltas[resType] != 0.0f)
                            {
                                print("add slot " + resType);
                                uic.AddSlot(resType, cy.m_currentDeltas[resType].ToString(), col);
                            }
                        }
                    }
                }
            }
        }

        public Building()
        {
            
        }
        public Building(BuildingTypes type)
        {
            m_type = type;
        }

        // ------------------

        void Setup()
        {
            int i = 0;
            foreach (var option in m_meshes)
            {
                if (option.m_type == m_type)
                {
                    m_modelIndex = i;

                }
                else { i++; }
            }

            if (m_modelIndex == -1) { Debug.Log("Could not find appropriate prefab mesh for building of type " + m_type); }
            else
            {
                if (m_ruined) { m_currentModel = Instantiate(m_meshes[m_modelIndex].m_ruinedMesh, transform.position, Quaternion.identity) as GameObject; }
                else { m_currentModel = Instantiate(m_meshes[m_modelIndex].m_repairedMesh, transform.position, Quaternion.identity) as GameObject; }
                m_currentModel.transform.parent = this.transform;
            }

            
        }

        public void ChangeBuildingType(BuildingTypes type)
        {
            m_type = type;
            Setup();
        }

        public bool IsRuined() { return m_ruined; }
        public void Ruin()
        {
            ResourceManager.RemoveBuilding(m_type, this);
            m_ruined = true;

            GameObject.Destroy(m_currentModel);
            m_currentModel = Instantiate(m_meshes[m_modelIndex].m_ruinedMesh, transform.position, Quaternion.identity) as GameObject;
            m_currentModel.transform.parent = this.transform;
        }
        public void Repair()
        {
            ResourceManager.AddBuilding(m_type, this);
            m_ruined = false;
            print("Building repaired.");

            GameObject.Destroy(m_currentModel);
            m_currentModel = Instantiate(m_meshes[m_modelIndex].m_repairedMesh, transform.position, Quaternion.identity) as GameObject;
            m_currentModel.transform.parent = this.transform;
        }

        public bool IsFullyActive() { return (m_activationRatio == 1.0f); }
        public float GetActivationRatio() { return m_activationRatio; }
        public void SetActivationRatio(float newActivationRatio) { m_activationRatio = newActivationRatio; }

        protected override void CompleteAction()
        {
            if (player.currentTool == gatherTool)
            {
                drops.spawnFromMaterial(transform.position);
                player.RemoveResourceFromWatch(this);
                GameObject.Destroy(this.gameObject);
            }
            else if (player.currentTool == repairTool)
            {
                var costProfile = ResourceManager.GetCostProfile(m_type);
                bool foundLimiter = false;
                foreach (var costItem in costProfile.m_buildingCost)
                {
                    if (costItem.m_viabilityBool == false) { foundLimiter = true; }
                }

                if (!foundLimiter)
                {
                    foreach (var costItem in costProfile.m_buildingCost)
                    {
                        Debug.Log("Cost Item: ID-" + costItem.m_type + " Delta-" + costItem.m_delta);
                        ResourceManager.ChangeResource(costItem.m_type, costItem.m_delta);
                    }
                    Repair();
                }
                else { currentHit = maxHit; UpdateUI(); }
            }
        }

        public override void SwitchedTool(Tool newCurrent)
        {
            if (!m_ruined || (newCurrent != repairTool && newCurrent != gatherTool)) {
                SetHasRelevantTool(true);
                if (m_currentUIWrapper != null)
                {
                    m_currentUIWrapper.SetActive(false);
                    m_uiActive = false;
                }

                if (m_runningUIWrapper != null)
                {
                    m_currentUIWrapper = m_runningUIWrapper;
                    m_uiActive = true;
                    FindHealthbarImage();
                    CheckActive();
                    UpdateUI();
                }
            }
            else { base.SwitchedTool(newCurrent); }
        }
    }
}