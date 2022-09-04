using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitReceiver : MonoBehaviour
{
	/*
     * Function to handle all (UI) raycast hits
     */
	public void OnRaycastHit()
    {
        if(gameObject.CompareTag("Button"))
        {
            if (gameObject.GetComponent<ButtonController>() != null)
            {
                gameObject.GetComponent<ButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<MainMenuButtonController>() != null)
            {
                gameObject.GetComponent<MainMenuButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<KeyboardButtonController>() != null)
            {
                gameObject.GetComponent<KeyboardButtonController>().Hovering();
            }

            else if (gameObject.GetComponent<BoatButtonController>() != null)
            {
                gameObject.GetComponent<BoatButtonController>().Hovering();
            }

            else if(gameObject.GetComponent<ShipWrightButtonController>() != null)
            {
                gameObject.GetComponent<ShipWrightButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<WeaponShopButtonController>() != null)
            {
                gameObject.GetComponent<WeaponShopButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<FenceButtonController>() != null)
            {
                gameObject.GetComponent<FenceButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<QuartermasterButtonController>() != null)
            {
                gameObject.GetComponent<QuartermasterButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<QuestButtonController>() != null)
            {
                gameObject.GetComponent<QuestButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<VendorButtonController>() != null)
            {
                gameObject.GetComponent<VendorButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<CraftingButtonController>() != null)
            {
                gameObject.GetComponent<CraftingButtonController>().Hovering();
            }

            else if(gameObject.GetComponent<MenuSceneButtonController>() != null)
            {
                gameObject.GetComponent<MenuSceneButtonController>().Hovering();
            }
            else if(gameObject.GetComponent<ItemSpawnerButton>() != null)
            {
                gameObject.GetComponent<ItemSpawnerButton>().Hovering();
            }
        }
        else if(gameObject.CompareTag("Toggle"))
        {
            if (gameObject.GetComponent<ToggleControllerUI>() != null)
            {
                gameObject.GetComponent<ToggleControllerUI>().Hovering();
            }
        }
    }

    /*
     * Function to handle all (UI) raycast end hits
     */
    public void OnRaycastEnd()
    {
        if (gameObject.CompareTag("Button"))
        {
            if (gameObject.GetComponent<ButtonController>() != null)
            {
                gameObject.GetComponent<ButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<MainMenuButtonController>() != null)
            {
                gameObject.GetComponent<MainMenuButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<KeyboardButtonController>() != null)
            {
                gameObject.GetComponent<KeyboardButtonController>().DoneHovering();
            }

            else if (gameObject.GetComponent<BoatButtonController>() != null)
            {
                gameObject.GetComponent<BoatButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<ShipWrightButtonController>() != null)
            {
                gameObject.GetComponent<ShipWrightButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<WeaponShopButtonController>() != null)
            {
                gameObject.GetComponent<WeaponShopButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<FenceButtonController>() != null)
            {
                gameObject.GetComponent<FenceButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<QuartermasterButtonController>() != null)
            {
                gameObject.GetComponent<QuartermasterButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<QuestButtonController>() != null)
            {
                gameObject.GetComponent<QuestButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<VendorButtonController>() != null)
            {
                gameObject.GetComponent<VendorButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<CraftingButtonController>() != null)
            {
                gameObject.GetComponent<CraftingButtonController>().DoneHovering();
            }

            else if (gameObject.GetComponent<MenuSceneButtonController>() != null)
            {
                gameObject.GetComponent<MenuSceneButtonController>().DoneHovering();
            }
            else if (gameObject.GetComponent<ItemSpawnerButton>() != null)
            {
                gameObject.GetComponent<ItemSpawnerButton>().DoneHovering();
            }
        }

        else if (gameObject.CompareTag("Toggle"))
        {
            if (gameObject.GetComponent<ToggleControllerUI>() != null)
            {
                gameObject.GetComponent<ToggleControllerUI>().DoneHovering();
            }
        }
    }

    /*
     * Function to handle all (UI) raycast selects
     */
    public void OnRaycastSelect()
    {
        if (gameObject.CompareTag("Button"))
        {
            if (gameObject.GetComponent<ButtonController>() != null)
            {
                gameObject.GetComponent<ButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<MainMenuButtonController>() != null)
            {
                gameObject.GetComponent<MainMenuButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<KeyboardButtonController>() != null)
            {
                gameObject.GetComponent<KeyboardButtonController>().PressButton();
            }

            else if(gameObject.GetComponent<BoatButtonController>() != null)
            {
                gameObject.GetComponent<BoatButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<ShipWrightButtonController>() != null)
            {
                gameObject.GetComponent<ShipWrightButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<WeaponShopButtonController>() != null)
            {
                gameObject.GetComponent<WeaponShopButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<FenceButtonController>() != null)
            {
                gameObject.GetComponent<FenceButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<QuartermasterButtonController>() != null)
            {
                gameObject.GetComponent<QuartermasterButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<QuestButtonController>() != null)
            {
                gameObject.GetComponent<QuestButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<VendorButtonController>() != null)
            {
                gameObject.GetComponent<VendorButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<CraftingButtonController>() != null)
            {
                gameObject.GetComponent<CraftingButtonController>().PressButton();
            }

            else if (gameObject.GetComponent<MenuSceneButtonController>() != null)
            {
                gameObject.GetComponent<MenuSceneButtonController>().PressButton();
            }
            else if (gameObject.GetComponent<ItemSpawnerButton>() != null)
            {
                gameObject.GetComponent<ItemSpawnerButton>().PressButton();
            }
        }

        else if (gameObject.CompareTag("Toggle"))
        {
            if (gameObject.GetComponent<ToggleControllerUI>() != null)
            {
                gameObject.GetComponent<ToggleControllerUI>().PressButton();
            }
        }
    }
}
