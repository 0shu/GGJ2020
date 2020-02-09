using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildGetter : MonoBehaviour
{
    public Text m_display;

    // Start is called before the first frame update
    void Start()
    {
        m_display.text = "Build " + Application.version;
    }
}
