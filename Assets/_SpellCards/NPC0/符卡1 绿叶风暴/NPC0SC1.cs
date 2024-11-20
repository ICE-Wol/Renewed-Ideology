using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC0SC1 : MonoBehaviour
{
    public NPC0SC1JadeG jadeGPrefab;
    
    public int bulletWays;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < bulletWays; i++) {
            NPC0SC1JadeG jadeG = Instantiate(jadeGPrefab, transform);
            jadeG.transform.position = transform.position;
            jadeG.dir = 360f / bulletWays * i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
