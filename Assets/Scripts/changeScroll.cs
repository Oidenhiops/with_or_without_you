using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class changeScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float value;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scrollRect.verticalNormalizedPosition = value;
    }
}
