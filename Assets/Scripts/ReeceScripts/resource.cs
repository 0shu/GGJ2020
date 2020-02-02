using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2020
{
    public class resource : MonoBehaviour
    {

        //public enum GatheredResource { Wood, Brick, Steel };

        float currentHit;
        public float maxHit;

        public Image healthBar;

        protected playerInteraction player;
        protected materialDrops drops;

        public Tool gatherTool;

        [SerializeField]
        public ResourceTypes currentResource;

        bool deleteFlag = false;

        public Mesh destroyedMesh;
        MeshFilter mRenderer;

        void Start()
        {
            mRenderer = GetComponent<MeshFilter>();
            player = FindObjectOfType<playerInteraction>();
            drops = GetComponentInParent<materialDrops>();
            currentHit = maxHit;
            //Material(1);
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

        public virtual void Material(int GatheredResource)
        {
            if (GatheredResource == 0)
            {
                currentResource = ResourceTypes.Wood;
                print("GOT WOOD");
                print(currentResource);
            }
            if (GatheredResource == 1)
            {
                currentResource = ResourceTypes.Bricks;
                print("GOT BRICK");
                print(currentResource);
            }
            if (GatheredResource == 2)
            {
                currentResource = ResourceTypes.Steel;
            }
        }

        void UpdateUI()
        {
            healthBar.fillAmount = currentHit / maxHit;
        }
    }
}