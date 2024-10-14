using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionUIController : MonoBehaviour
{
    void Update()
    {
       transform.LookAt(Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + 180f,
            transform.eulerAngles.z);
    }
}
