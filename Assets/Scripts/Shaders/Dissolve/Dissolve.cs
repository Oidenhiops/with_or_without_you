using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] GameObject objectToDisolve;
    List<Material> materials = new List<Material>();
    float refreshRate = 0.025f;
    float dissolveRate = 0.0125f;
    private void Awake()
    {
        foreach (Material material in objectToDisolve.GetComponent<Renderer>().materials)
        {
            materials.Add(material);
        }
    }
    public void AppearObject()
    {
        StartCoroutine(AppearObj());
    }
    public void DissolveObject()
    {
        StartCoroutine(DissolveObj());
    }
    IEnumerator DissolveObj()
    {
        if (materials.Count > 0)
        {
            float counter = 0;
            while (materials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < materials.Count; i++)
                {
                    materials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
    IEnumerator AppearObj()
    {
        if (materials.Count > 0)
        {
            float counter = 1;
            while (materials[0].GetFloat("_DissolveAmount") > 0)
            {
                counter -= dissolveRate;
                for (int i = 0; i < materials.Count; i++)
                {
                    materials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
