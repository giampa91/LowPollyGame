using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Spell : AbstractProjectile<Spell>
{

    // Start is called before the first frame update
    void Start()
    {
        if (!isDirectionFixed()) direction = (PlayerTransform.position + new Vector3(0,0.3f,0) - transform.position).normalized;
        transform.up = -direction;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Math.Max(Math.Abs((PlayerTransform.position-transform.position).magnitude), 0.1f);
        float speedIncrease = (distance<1) ? 1 : (1/distance)*4;

        Vector3 directiona = (PlayerTransform.position + new Vector3(0,0.3f,0) - transform.position).normalized;
        float angle = Vector3.Angle(transform.up, -directiona);
        angle = angle/360;
        //direction = ((transform.up - directiona) * 1).normalized;
        direction = Vector3.Slerp (transform.up, ((transform.up - directiona) * 1).normalized, 4*speedIncrease*Time.deltaTime);
        transform.up = direction;
        
        transform.position += -transform.up * speed * Time.deltaTime;
    }
}
