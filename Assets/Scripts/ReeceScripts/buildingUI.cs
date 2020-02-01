using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2020
{
    public class buildingUI : MonoBehaviour
    {
        public GameObject uiBuilding;

        void Start()
        {
            uiBuilding.SetActive(false);
        }

        public void ActivateUI()
        {
            uiBuilding.SetActive(true);
        }

        public void DeactivateUI()
        {
            uiBuilding.SetActive(false);
        }
    }
}