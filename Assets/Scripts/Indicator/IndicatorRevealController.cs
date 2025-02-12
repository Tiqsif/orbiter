using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorRevealController : MonoBehaviour
{
    [SerializeField] private float _sphereRadius = 10f;
    

    void Update()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Indicator");

        for (int i = objects.Length-1; i >= 0; i--)
        {
            GameObject obj = objects[i];
            if (obj.TryGetComponent<Renderer>(out Renderer renderer))
            {
                if (renderer.sharedMaterial?.shader.name == "Shader Graphs/SG_Reveal" ) // sharedMaterial or material ???
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(block);
                    block.SetVector("_SphereCenter", transform.position);
                    block.SetFloat("_SphereRadius", _sphereRadius);
                    renderer.SetPropertyBlock(block);
                    //Debug.Log(block.GetVector("_SphereCenter") + " " + block.GetFloat("_SphereRadius"));

                }
            }
            
        }
    }
}
