using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

	public GameObject menuToInstantiate;
	GameObject instantiatedToppingsOption;
	public AudioSource soundSource;
	List<GameObject> finalList;
	bool displaySandwich = false;

	/*
	 * Instantiates the menu which contains the various toppings to make a sandwich.
	 * Plays a sound to ensure the user knows where to look when the menu is instantiated
	 * as it does not appear on screen.
	 */
	public void MakeSandwich()
	{
		Vector3 menuPosition = new Vector3 (-0.8418117f, 2.784424f, -9.94758f);
		instantiatedToppingsOption = Instantiate (menuToInstantiate, menuPosition, Quaternion.identity);
		soundSource.Play ();
	}

	/*
	 * Displays the final sandwich that the user has made. It also destorys the menu game object.
	 */ 
	public void DisplaySandwich()
	{
		if (!displaySandwich) 
		{
			displaySandwich = true;
			finalList = DragDropObject.selectedItemsInSandwich ();
			if (finalList.Count >= 1) {
				for (int i = 0; i < finalList.Count; i++) {
					finalList [i].transform.Translate (new Vector3 (1.659f, 0f, 0f));
				}
			}
		}
	}
}