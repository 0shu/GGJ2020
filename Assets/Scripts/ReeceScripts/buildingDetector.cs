using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class buildingDetector : MonoBehaviour
    {
        public Transform playerCamera;
        public float detectionDistance;

        bool detected;
        resource ui;
        public resource res { get; set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            DetectBuilding();
        }

        void DetectBuilding()
        {
            Ray raycast = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(raycast, out hit, detectionDistance))
            {
                if (detected == false)
                {
                    detected = true;
                    if (hit.collider.gameObject.CompareTag("Building"))
                    {
                        ui = hit.collider.gameObject.GetComponent<resource>();
                        ui.SetCloseEnough(true);
                    }
                    if(hit.collider.gameObject.CompareTag("Resource"))
                    {
                        // target is rock
                        ui = hit.collider.gameObject.GetComponent<resource>();
                        ui.SetCloseEnough(true);
                        res = hit.collider.gameObject.GetComponent<resource>();
                    }
                }
            }
            else
            {
                if (ui != null)
                {
                    ui.SetCloseEnough(false);
                    ui = null;
                }

                if(res != null)
                {
                    res = null;
                }

                detected = false;
            }
        }
    }
}