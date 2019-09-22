using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UShake : MonoBehaviour
{
    public float maxZ = 10;
    public float speed = 1;
    bool bUp;
    float lastZ;
    void Update()
    {
        var er = transform.eulerAngles;
        if (bUp && lastZ < maxZ)
        {
            lastZ += Time.deltaTime * speed;
            if (lastZ >= maxZ)
            {
                lastZ = maxZ;
                bUp = !bUp;
            }
        }
        else if (!bUp && lastZ > -maxZ)
        {
            lastZ -= Time.deltaTime * speed;
            if (lastZ <= -maxZ)
            {
                lastZ = -maxZ;
                bUp = !bUp;
            }
        }
        er.z = lastZ;
        transform.eulerAngles = er;
    }
}
