using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAndSetItemDescription : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private GameObject itemUIComponent;

    void Start()
    {
        if(itemName == null)
            itemName = this.gameObject.name;
    }
		
    void Update()
    {}

	public string retrieveItemDescription()     {return itemName + ": " + itemDescription;  }
    public string retrieveItemName()            {return itemName;                           }
    public string retrieveItemUIComponentName() {return itemUIComponent.name;               }
    public GameObject retrieveItemUIComponent() {return itemUIComponent;                    }
}
