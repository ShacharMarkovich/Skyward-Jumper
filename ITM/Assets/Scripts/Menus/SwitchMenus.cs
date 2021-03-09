using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchMenus : MonoBehaviour
{
    public GameObject _OnMenu;              //The active menu
    public GameObject _OffMenu;             //The unactive menu
    public GameObject _FirstButton;         //The first button in the unactive menu

    /// <summary>
    /// Change between menus.
    /// </summary>
    public void Switch()
    {
        if (!Input.GetKey(KeyCode.LeftArrow) || !Input.GetKey(KeyCode.LeftArrow))
        {
            _OnMenu.SetActive(false);
            _OffMenu.SetActive(true);

            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(_FirstButton, null);
        }
    }
}