using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSavingSystem : MonoBehaviour
{
    private void Start()
    {
        if (!PlayerPrefs.HasKey("Energy"))
        {
            //Crear clave arrancar de forma normal
        }
        else
        {
            //restaurar
        }
    }
}
