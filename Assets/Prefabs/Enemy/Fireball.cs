using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : AbstractProjectile<Fireball>
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
        transform.position += direction * speed * Time.deltaTime;
    }
}
