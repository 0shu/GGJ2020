using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GGJ2020
{
    public class InventoryDisplay : MonoBehaviour
    {
        [System.Serializable]
        public struct ResourceTextPair
        {
            public ResourceTypes m_type;
            public Text m_text;
        }

        public List<ResourceTextPair> m_simpleDisplays;
        public List<ResourceTextPair> m_complexDisplays;
        public List<ResourceTextPair> m_customDisplays;

        int[] m_resources = new int[ResourceManager.ResourceTypeCount];
        int[] m_maxPosDeltaR = new int[ResourceManager.ResourceTypeCount];
        int[] m_maxNegDeltaR = new int[ResourceManager.ResourceTypeCount];
        int[] m_DeltaR = new int[ResourceManager.ResourceTypeCount];
        bool[] m_resourceIsLimited = new bool[ResourceManager.ResourceTypeCount];

        public static InventoryDisplay s_instance;

        void Awake()
        {
            if (s_instance == null) { s_instance = this; }
            else { return; }
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < m_complexDisplays.Count; i += 4)
            {
                m_complexDisplays[i + 1].m_text.color = Color.green;
                m_complexDisplays[i + 3].m_text.color = Color.red;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        //changes the stored values
        public void UpdateValues(int[] resources, int[] maxPosDeltaR, int[] maxNegDeltaR, int[] deltaR, bool[] resourceIsLimited)
        {
            m_resources = resources;
            m_maxPosDeltaR = maxPosDeltaR;
            m_maxNegDeltaR = maxNegDeltaR;
            m_DeltaR = deltaR;
            m_resourceIsLimited = resourceIsLimited;

            SetDisplayValues();
        }

        //Sets the texts to display the stored values
        public void SetDisplayValues()
        {
            for (int i = 0; i < m_simpleDisplays.Count; i++)
            {
                m_simpleDisplays[i].m_text.text = m_resources[(int)m_simpleDisplays[i].m_type].ToString();
            }

            for (int i = 0; i < m_complexDisplays.Count; i += 4)
            {
                m_complexDisplays[i].m_text.text    = m_resources[(int)m_complexDisplays[i].m_type].ToString();

                m_complexDisplays[i+1].m_text.text  = "+" + m_maxPosDeltaR[(int)m_complexDisplays[i].m_type].ToString();

                string deltaText = m_DeltaR[(int)m_complexDisplays[i].m_type].ToString();
                m_complexDisplays[i+2].m_text.text  = m_DeltaR[(int)m_complexDisplays[i].m_type] < 0 ? deltaText : "+" + deltaText;
                m_complexDisplays[i+2].m_text.color = m_resourceIsLimited[(int)m_complexDisplays[i].m_type] ? Color.red : Color.black;

                m_complexDisplays[i+3].m_text.text  = m_maxNegDeltaR[(int)m_complexDisplays[i].m_type].ToString();
            }

            for (int i = 0; i < m_customDisplays.Count; i++)
            {
                switch (m_customDisplays[i].m_type)
                {
                    case ResourceTypes.Firepower:
                        if ((float)m_maxNegDeltaR[(int)m_customDisplays[i].m_type] > 0.0f)
                        {
                            m_customDisplays[i].m_text.text = (((float)m_resources[(int)m_customDisplays[i].m_type] / (float)m_maxNegDeltaR[(int)m_customDisplays[i].m_type]) * 100.0f).ToString() + "%";
                        }
                        else
                        {
                            m_customDisplays[i].m_text.text = "---";
                        }
                        
                        m_customDisplays[i].m_text.color = m_resourceIsLimited[(int)m_customDisplays[i].m_type] ? Color.red : Color.black;
                        break;
                    default:
                        m_customDisplays[i].m_text.text = "---";
                        break;
                }
                
            }

            /*if (m_displays.Count == m_values.Count)
            {
                for (int i = 0; i < m_displays.Count; i++)
                {
                    m_displays[i].text = m_values[i].ToString();
                }
            }
            else
            {
                Debug.Log("INVENTORY_DISPLAY: List sizes dont match");
            }*/
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(InventoryDisplay))]
    public class InventoryDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InventoryDisplay myScript = (InventoryDisplay)target;
            if (GUILayout.Button("Set Values"))
            {
                myScript.SetDisplayValues();
            }
        }
    }
#endif
}