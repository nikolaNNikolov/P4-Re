using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Helper_OpenClose : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("h"))
		{
			if(canvasGroup.alpha == 0)
                canvasGroup.alpha = 1;
            else
                canvasGroup.alpha = 0;
		}
    }
}
