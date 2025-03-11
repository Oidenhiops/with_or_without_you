using System.Collections.Generic;
using UnityEngine;

public class GrassTrack : MonoBehaviour
{
    [SerializeField] Vector3 trackerPos;
    [SerializeField] List<Material> grassMats;
    [SerializeField] GameObject[] objects;
    float size = 0.65f;
    Vector3 offset = new Vector3(0, 0.32f, 0);
    [SerializeField] float rayDistance = 1;
    [SerializeField] LayerMask layerMask;
    float time = 0;
    public float speed = 2;
    public float dist;
    void Start()
    {
        foreach (Transform child in transform)
        {
            grassMats.Add(child.GetComponent<Renderer>().material);
        }
    }
    void Update()
    {
        RaycastHit[] objects = Physics.BoxCastAll(transform.position + offset, Vector3.one * size, Vector3.up, Quaternion.identity, rayDistance, layerMask);

        if (objects.Length > 0)
        {
            time = 0;
            trackerPos = GetMidpoint(objects);
            ApplyMovement(trackerPos);
        }
        else
        {
            time += Time.deltaTime * speed;
            float t = (Mathf.Sin(time * Mathf.PI * 2) + 1) * 0.5f;
            trackerPos = Vector3.Lerp(transform.position + Vector3.left * dist, transform.position + Vector3.right * dist, t);
            ApplyMovement(trackerPos);
        }
    }
    void ApplyMovement(Vector3 pos)
    {
        foreach (Material grass in grassMats)
        {
            grass.SetVector("_TrakerPosition", pos);
        }

    }
    Vector3 GetMidpoint(RaycastHit[] transforms)
    {
        Vector3 sum = Vector3.zero;
        foreach (var t in transforms)
        {
            sum += t.transform.position;
        }
        return sum / transforms.Length;
    }
}
