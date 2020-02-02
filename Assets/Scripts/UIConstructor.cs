using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2020 {
    public class UIConstructor : MonoBehaviour
    {
        [SerializeField]
        GameObject m_resourceContainer;

        [SerializeField]
        GameObject m_slotPrefab;

        [System.Serializable]
        public class ResourceImagePair
        {
            public ResourceTypes m_type;
            public Sprite m_sprite;
        }
        [SerializeField]
        ResourceImagePair[] sprites;

        Sprite[] m_baseIcons = new Sprite[ResourceManager.ResourceTypeCount];

        List<GameObject> m_slots = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            foreach (var slot in sprites)
            {
                m_baseIcons[(int)slot.m_type] = slot.m_sprite;
            }
        }

        public void Flush()
        {
            foreach (var go in m_slots) { GameObject.Destroy(go); }
        }

        public void AddSlot(ResourceTypes type, string text, Color textColour)
        {
            GameObject newSlot = Instantiate(m_slotPrefab, m_resourceContainer.transform);
            newSlot.GetComponentInChildren<Image>().sprite = m_baseIcons[(int)type];

            var textObj = newSlot.GetComponentInChildren<Text>();
            textObj.text = text;
            textObj.color = textColour;

            m_slots.Add(newSlot);
        }
    }
}