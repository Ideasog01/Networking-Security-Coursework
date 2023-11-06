using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerClass : PlayerActions
{
    private void Awake()
    {
        PrimaryAction += ShootArrow;
    }

    public void ShootArrow()
    {
        Debug.Log("Shoot Arrow!");
    }
}
