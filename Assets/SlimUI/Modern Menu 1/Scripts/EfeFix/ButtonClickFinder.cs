using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]


public class ButtonClickFinder : MonoBehaviour
{
    public bool Refresh = false;
    [SerializeField] GameObject[] GOsWithButtons;

    private void OnValidate()
    {
        if (Refresh == true)
        {

            foreach (GameObject go in GOsWithButtons)
            {

            }


            Refresh = false;
        }
    }
}
