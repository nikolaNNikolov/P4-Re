using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Battle_Elements_Common : MonoBehaviour
{
    [SerializeField] private UI_Battle UIBattle;
    [SerializeField] private AnimationClip animationShow;
    
    void OnDisable()
    {
		UIBattle.isChildElementActive = false;
    }

    void OnEnable()
    {
		UIBattle.isChildElementActive = true;  
	}
    
    void Start()
    { }

    void Update()
    { 
        if(Input.GetButtonDown("Cancel") && !UIBattle.UI_Wheel.gameObject.activeInHierarchy
            && !UIBattle.UI_TV.gameObject.activeInHierarchy)
        {
            UIBattle.toggleTV(true);
            UIBattle.deactivateWheelElementAfterAnimation(this.gameObject, animationShow.name);
            UIBattle.activateUIWheel();
        }
    }
}
