using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class playerInteraction : MonoBehaviour
    {
        enum Tool {Pickaxe, Hammer, Bucket};
        Tool currentTool;

        public GameObject pickaxe;
        public GameObject bucket;
        public GameObject hammer;

        // Start is called before the first frame update
        void Start()
        {
            Tool currentTool;
            currentTool = Tool.Pickaxe;
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
                currentTool = Tool.Bucket;
                Interaction();
            }
            if (Input.GetKeyDown("3"))
            {
                currentTool = Tool.Hammer;
                Interaction();
            }
        }

        void Interaction()
        {
            switch (currentTool)
            {
                case Tool.Pickaxe:
                    Instantiate(pickaxe);
                    Debug.Log("Pickaxe");
                    break;
                case Tool.Bucket:
                    Instantiate(bucket);
                    Debug.Log("Bucket");
                    break;
                case Tool.Hammer:
                    Instantiate(hammer);
                    Debug.Log("Hammer");
                    break;
                default:
                    Debug.Log("NOTHING");
                    break;
            }
        }
    }
}
