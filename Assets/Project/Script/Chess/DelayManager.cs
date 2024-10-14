using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayManager : MonoBehaviour
{
    public static DelayManager Instance;

    public WaitForSeconds ms05 = new WaitForSeconds(0.05f); 

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
