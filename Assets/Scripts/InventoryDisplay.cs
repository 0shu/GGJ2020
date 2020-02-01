using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryDisplay : MonoBehaviour
{
    public List<Text> m_displays;
    public List<int> m_values;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //changes the stored values
    public void SetValues(List<int> vals)
    {
        m_values = vals;
    }

    //Sets the texts to display the stored values
    public void SetDisplayValues()
    {
        if(m_displays.Count == m_values.Count)
        {
            for (int i = 0; i < m_displays.Count; i++)
            {
                m_displays[i].text = m_values[i].ToString();
            }
        }
        else
        {
            Debug.Log("INVENTORY_DISPLAY: List sizes dont match");
        }
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
