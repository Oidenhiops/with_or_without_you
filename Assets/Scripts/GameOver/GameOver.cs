using UnityEngine;

public class GameOver : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("�Colisi�n detectada con: " + collision.gameObject.name + "!");
        Destroy(gameObject);
    }
}
