﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class buildingUILookAtCamera : MonoBehaviour
    {
        Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (!gameObject.activeSelf)
                return;

            //transform.LookAt(transform.position + mainCamera.transform.rotation * -Vector3.back, mainCamera.transform.rotation * Vector3.up);
            Rotate();
        }

        void Rotate()
        {
            Quaternion rotation;
            Vector3 objToCameraVector = mainCamera.transform.position - transform.position;
            objToCameraVector.y = 0f;

            rotation = Quaternion.LookRotation(-objToCameraVector);
            transform.rotation = rotation;
        }
    }
}