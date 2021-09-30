using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.GetComponent<Bullet>();

        if (bullet)
        {
            RecieveDamage();
        }
    }
}
