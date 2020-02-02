using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2020
{
    public class resource : MonoBehaviour
    {
        float currentHit;
        public float maxHit;

        public Image healthBar;

        protected playerInteraction player;
        protected materialDrops drops;
        public Tool gatherTool;

        bool deleteFlag = false;

        public Mesh destroyedMesh;
        MeshFilter mRenderer;

        void Start()
        {
            mRenderer = GetComponent<MeshFilter>();
            player = FindObjectOfType<playerInteraction>();
            drops = GetComponentInParent<materialDrops>();
            currentHit = maxHit;
            UpdateUI();
        }

        void OnDisable()
        {
            drops.spawnFromMaterial(transform.position);
        }

        public virtual void Gather()
        {
            currentHit--;
            UpdateUI();
            if (currentHit <= 0)
            {
                //play sound
                //play particle system
                //add resource            
                //destroy
                gameObject.SetActive(false);
                // activate destroyed version
            }
        }

        void UpdateUI()
        {
            healthBar.fillAmount = currentHit / maxHit;
        }
    }
}