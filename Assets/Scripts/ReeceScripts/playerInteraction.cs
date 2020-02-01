using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public enum Tool { Pickaxe, Hammer, Bucket };
    public class playerInteraction : MonoBehaviour
    {
        public Tool currentTool;

        public List<GameObject> toolList = new List<GameObject>();
        buildingDetector detector;

        // Start is called before the first frame update
        void Start()
        {
            detector = GetComponent<buildingDetector>();
            currentTool = Tool.Pickaxe;
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
                currentTool = Tool.Bucket;
                Interaction();              
            }

            if(Input.GetMouseButtonDown(0))
            {
                if(detector.res != null)
                {
                    detector.res.Gather();
                }
                
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
        }
    }
}
