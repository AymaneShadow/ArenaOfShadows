using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject gameObj)
    {        
        if (gameObj.name == "RED" || gameObj.name == "GREEN" || gameObj.name == "YELLOW" || gameObj.name == "BLUE" || gameObj.name.StartsWith("Slender"))
        {
            if (gameObj.transform.Find("InnerChannelingParticleSystem").gameObject.activeSelf == false)
            {
                Debug.Log($"Collision with {gameObj.name} detected.");
                gameObj.transform.Find("InnerChannelingParticleSystem").gameObject.SetActive(true);
            }
        }
    }
}
