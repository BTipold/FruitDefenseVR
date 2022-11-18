using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class BaseMenu : MonoBehaviour
{
    // -----------------------------------------------------------
    // @Summary: wrapper, see below.
    // @Param: g - gameobject to match
    // @Return: GameObject - returns the spotlighted button
    // -----------------------------------------------------------
    public GameObject SpotlightButton(GameObject g)
    {
        return SpotlightButton(g.name);
    }

    // -----------------------------------------------------------
    // @Summary: will supress all buttons except the one matching
    //   the name key. 
    // @Param: nameKey - name of the button object to search - 
    //   needs to match exactly. 
    // @Return: GameObject - returns the spotlighted button
    // -----------------------------------------------------------
    public GameObject SpotlightButton(string nameKey)
    {
        // create list to store all button game objects
        List<Button> allButtons = new List<Button>();
        List<GameObject> allButtonsObjs = new List<GameObject>();

        // get array of all buttons in menu children
        Extension.GetActiveComponentsInChildrenRecursively(gameObject.transform, allButtons);

        // get the game object of each button and store in the list
        allButtons.ForEach((b) => allButtonsObjs.Add(b.gameObject));

        // remove all the button objects with the name key
        // so that they dont get dismissed
        var spotlight = allButtonsObjs.Find((g) => nameKey == g.name);
        allButtonsObjs.RemoveAll((g) => nameKey == g.name);
         
        // hide remaining buttons
        allButtonsObjs.ForEach(b => b.AddComponent<SupressButton>());
        return spotlight;
    }

    // -----------------------------------------------------------
    // @Summary: will remove all the supressions from the 
    //   buttons.
    // @Return: void
    // -----------------------------------------------------------
    public void RestoreButtons()
    {
        // create list to store all SupressButton objects
        List<SupressButton> allButtons = new List<SupressButton>();

        // get array of all buttons in menu children
        Extension.GetComponentsInChildrenRecursively(gameObject.transform, allButtons);

        // destroy the supression scripts
        foreach (SupressButton b in allButtons)
        {
            Destroy(b);
        }
    }

    // -----------------------------------------------------------
    // @Summary: todo 
    // @Param: todo
    // @Return: void.
    // -----------------------------------------------------------
    public void DoOnPress(string nameKey, UnityAction lambda)
    {
        // create list to store all button game objects
        List<Button> allButtons = new List<Button>();
        
        // get array of all buttons in menu children
        Extension.GetActiveComponentsInChildrenRecursively(gameObject.transform, allButtons);

        // Get all buttons that match the namekey
        List<Button> matchingButtons = allButtons.FindAll((b) => b.name == nameKey);
        matchingButtons.ForEach((b) => b.onClick.AddListener(lambda));

    }

    // Start is called before the first frame update
    public virtual void Start()
    { 
    
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
