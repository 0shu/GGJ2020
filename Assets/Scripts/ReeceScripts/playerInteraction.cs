﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public enum Tool { Pickaxe, Hammer, Bucket, Weapon, None };

    public class playerInteraction : MonoBehaviour
    {
        public Tool currentTool;

        public Animator m_anim;

        public List<GameObject> toolList = new List<GameObject>();
        buildingDetector detector;

        List<resource> m_activeResources = new List<resource>();

        // Start is called before the first frame update
        void Start()
        {
            detector = GetComponent<buildingDetector>();
            currentTool = Tool.Hammer;
            Interaction();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("1"))
            {
                currentTool = Tool.Pickaxe;
                Interaction();
            }
            if (Input.GetKeyDown("2"))
            {
                currentTool = Tool.Hammer;
                Interaction();
            }
            if (Input.GetKeyDown("3"))
            {
                currentTool = Tool.Weapon;
                print("Weapon");
                Interaction();
            }

            if (Input.GetMouseButtonDown(0))
            {
                
                if(detector.res != null)
                {
                    print("!!!");
                    detector.res.Interact();
                }
                if(currentTool == Tool.Weapon)
                {
                    m_anim.Play("Stab", 0, 0);
                    this.gameObject.GetComponent<attackEnemy>().AttackEnemy();
                }
                else if (currentTool == Tool.Hammer) m_anim.Play("Swing2", 0, 0);
                else if (currentTool == Tool.Pickaxe) m_anim.Play("Saw", 0, 0);
                
            }
        }

        void Interaction()
        {
            toolList[(int)currentTool].SetActive(true);

            for(int i = 0; i < toolList.Count; i++)
            {
                if(i != (int)currentTool)
                {
                    toolList[i].SetActive(false);
                }
            }

            foreach (var res in m_activeResources) { res.SwitchedTool(currentTool); }
        }

        public void AddResourceToWatch(resource toWatch) { m_activeResources.Add(toWatch); }
        public void RemoveResourceFromWatch(resource fromWatch) { m_activeResources.Remove(fromWatch); }
    }
}
