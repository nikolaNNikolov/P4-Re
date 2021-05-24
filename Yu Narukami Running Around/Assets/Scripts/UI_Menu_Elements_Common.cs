using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Menu_Elements_Common : MonoBehaviour
{
    [SerializeField] private UI_Menu UIMenu;
    [SerializeField] private AnimationClip animationShow;
    
    //void OnDisable()
    //{
	//	UIBattle.isChildElementActive = false;
    //}

   // void OnEnable()
    //{
	//	UIBattle.isChildElementActive = true;  
	//}
    
    void Start()
    { }

    void Update()
    { 
       // if(Input.GetButtonDown("Cancel") && !UIMenu.getUIMenuCenter().gameObject.activeInHierarchy
       //     && !UIMenu.getIsInMenu())
       // {
        //    Debug.Log("fecalfunny");
        //    UIMenu.showMenu(this.gameObject);
        //}
    }

    public void hidePanel()
    {
        if(this.gameObject.GetComponent<Animator>() == null)
            this.gameObject.SetActive(false);
        else
        {
         StartCoroutine(Dialogue_Manager.de_activateDialogueAfterAnimation(this.gameObject, this.gameObject, 
            "P4_UI_Menu_Skill_Show", "Hide", false));
        }
    }
}
