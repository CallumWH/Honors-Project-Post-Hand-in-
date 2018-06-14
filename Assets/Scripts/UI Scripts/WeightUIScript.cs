using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightUIScript : MonoBehaviour
{

    private NeedsMonitor needsMonitor;

    private TextMesh textMesh;
    public List<string> textUI = new List<string>();

    // Use this for initialization
    void Awake()
    {
        textMesh = gameObject.GetComponent<TextMesh>();
        needsMonitor = gameObject.GetComponentInParent<NeedsMonitor>();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < needsMonitor.GetListSize(); i++)
        {
            textUI[i] = needsMonitor.needList[i].needName + " Weight :" + string.Format("{0:0.0}", needsMonitor.needList[i].GetWeight());
        }

        textMesh.text = string.Join("\n", textUI.ToArray());

    }

    public void addToList()
    {
        for (int i = 0; i < needsMonitor.GetListSize(); i++)
        {
            textUI.Add("Debug");
        }
    }
}