using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2020
{
    public class resource : MonoBehaviour
    {

        //public enum ToolUsed { Hammer, Saw };

        protected float currentHit;
        public float maxHit;

        Image healthBar = null;

        protected GameObject m_currentUIWrapper = null;
        bool m_closeEnough = false;

        protected playerInteraction player;
        protected materialDrops drops;

        bool m_validToolActive = false;
        protected bool m_uiActive = false;

        public Tool gatherTool;
        public GameObject m_gatherUIWrapper;

        public Tool repairTool;
        public GameObject m_repairUIWrapper;

        public GameObject m_runningUIWrapper;

        bool deleteFlag = false;

        MeshFilter mRenderer;

        protected void Start()
        {
            if (m_gatherUIWrapper != null) { m_gatherUIWrapper.SetActive(false); }
            if (m_repairUIWrapper != null) { m_repairUIWrapper.SetActive(false); }
            if (m_runningUIWrapper != null) { m_runningUIWrapper.SetActive(false); }

            mRenderer = GetComponent<MeshFilter>();

            player = FindObjectOfType<playerInteraction>();
            player.AddResourceToWatch(this);

            drops = GetComponentInParent<materialDrops>();
            currentHit = maxHit;
            //Material(1);
            UpdateUI();
        }
        protected void Update()
        {
            if (m_uiActive)
            {
                if (player.currentTool == gatherTool && drops != null)
                {
                    UIConstructor uic = m_currentUIWrapper.GetComponent<UIConstructor>();
                    uic.Flush();

                    foreach (var option in drops.m_options)
                    {
                        float prob = (option.m_weight / drops.m_totalWeight);
                        string weightText = (prob * 100.0f).ToString() + "%";
                        Color col = prob > 0.25f ? Color.black : Color.red;

                        uic.AddSlot(option.m_type, weightText, col);
                    }
                }
            }
        }

        public virtual void Interact()
        {
            if (m_validToolActive)
            {
                currentHit--;
                UpdateUI();
                if (currentHit <= 0)
                {
                    CompleteAction();
                    SwitchedTool(player.currentTool);
                }
            }
        }

        protected virtual void CompleteAction()
        {
            if (player.currentTool == gatherTool)
            {
                drops.spawnFromMaterial(transform.position);
                player.RemoveResourceFromWatch(this);
                GameObject.Destroy(this.gameObject);
            }
        }

        public virtual void SwitchedTool(Tool newCurrent) {
            currentHit = maxHit;

            SetHasRelevantTool(newCurrent == gatherTool || newCurrent == repairTool);
            if (m_currentUIWrapper != null)
            {
                m_currentUIWrapper.SetActive(false);
                m_uiActive = false;
            }

            if (newCurrent == gatherTool && m_gatherUIWrapper != null) {
                m_currentUIWrapper = m_gatherUIWrapper;
                m_uiActive = true;
                FindHealthbarImage();
                CheckActive();
                UpdateUI();
            }
            else if (newCurrent == repairTool && m_repairUIWrapper != null)
            {
                m_currentUIWrapper = m_repairUIWrapper;
                m_uiActive = true;
                FindHealthbarImage();
                CheckActive();
                UpdateUI();
            }
            else { m_currentUIWrapper = null; }
        }

        protected void UpdateUI()
        {
            if (healthBar != null) { healthBar.fillAmount = currentHit / maxHit; }
        }

        protected void CheckActive()
        {
            if (m_currentUIWrapper != null)
            {
                m_uiActive = (m_closeEnough && m_validToolActive);
                m_currentUIWrapper.SetActive(m_uiActive);
            }
        }
        public void SetCloseEnough(bool value = true)
        {
            m_closeEnough = value;
            CheckActive();
        }
        public void SetHasRelevantTool(bool value = true)
        {
            m_validToolActive = value;
            CheckActive();
        }

        protected void FindHealthbarImage()
        {
            bool found = false;
            var images = m_currentUIWrapper.GetComponentsInChildren<Image>();
            foreach (Image i in images)
            {
                if (i.CompareTag("HealthBarImage")) {
                    healthBar = i;
                    found = true;
                }
            }
            if (!found) { healthBar = null; }
        }
    }
}