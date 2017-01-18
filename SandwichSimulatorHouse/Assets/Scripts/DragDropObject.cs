using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Any object that needs to be dragged and dropped will need this script
 * attached to it.
 */

public class DragDropObject : MonoBehaviour, DragDropHandler {

	// List of all selected sandwich toppings. Includes the plate as the first element.
	static List<GameObject> selectedItems = new List<GameObject>();

	// List of appropriate base toppings used to encolse the sandwich.
	static List<GameObject> appropriateEnclosingToppings = new List<GameObject>();

	bool isHeld;
	GameObject Reticle;
	GameObject clone;
	public GameObject objectToInstantiate;

	// Use this for initialization
	void Start () 
	{
		isHeld = false;
		Reticle = GameObject.Find ("DragDropReticle");

		// Adds initial plate object into selectedItems list.
		if (selectedItems.Count == 0) 
		{
			selectedItems.Add (GameObject.Find ("Plate_Large"));
		}

		// Initializes appropriateEnclosingToppings list with base toppings.
		appropriateEnclosingToppings.Add(GameObject.Find("Waffle"));
		appropriateEnclosingToppings.Add(GameObject.Find("Toast"));
		appropriateEnclosingToppings.Add(GameObject.Find("Breadslice"));

	}

	// Update is called once per frame
	void Update () 
	{
		// Drag the selected game object
		if (isHeld) 
		{
			Ray ray = new Ray (Reticle.transform.position, Reticle.transform.forward);
			objectToInstantiate.transform.position = ray.GetPoint (4);
		}
	}

	/*
	 * This method is called when the user has selected the game object they wish to drag.
	 * The user must have the reticle pointing at the game object while simultaneously
	 * pressing the trigger on Google cardboard for this method to be called. 
	 * This method will create a clone so that it can 'replace' the selected game object.
	 * This allows the user to have the same topping more than once in their sandwich.
	 */
	public void StartHandleGazeTrigger ()
	{
		isHeld = true;

		// ensure the clone game object has the correct rotation
		int xRotation;
		if (gameObject.name.Contains ("Toast") || gameObject.name.Contains ("Breadslice")) 
		{
			xRotation = -90;
		} 
		else if (gameObject.name.Contains ("CheeseSlice")) 
		{
			xRotation = 0;
		} 
		else 
		{
			xRotation = 90;
		}

		// clone the game object at the same position
		Vector3 clonePosition = new Vector3(objectToInstantiate.transform.position.x, 
			objectToInstantiate.transform.position.y,
			objectToInstantiate.transform.position.z);

		clone = Object.Instantiate(objectToInstantiate, clonePosition, Quaternion.Euler (xRotation, 0 ,0));
	}

	/*
	 * This allows users to stop dragging a game object to a different position. The user must have
	 * the reticle pointing at the object while simultaneously letting go of the trigger on 
	 * Google cardboard for this to be called.
	 */
	public void StopHandleGazeTrigger ()
	{
		isHeld = false;
		placeToppingOnPlate ();
		Destroy (objectToInstantiate);
	}

	/*
	 * Places the chosen topping on top of all the other previously chosen toppings to make
	 * the sandwich. If this is the first topping chosen, it will be placed on the plate if it
	 * is an appropriate topping for the base of the sandwich. Otherwise, a randomly selected base
	 * will be chosen and the user's selected topping will be placed on top of that.
	 */
	void placeToppingOnPlate()
	{
		GameObject plateClone;

		// This check is done to ensure that the user selects an appropriate topping from the menu
		// if this is their first choice. A user cannot make a sandwich without the correct base and correct
		// top to enclose their fillings.
		if(selectedItems.Count == 1)
		{
			bool validTopping = false;
			for(int i = 0; i < appropriateEnclosingToppings.Count; i++)
			{

				if(objectToInstantiate.name.Contains(appropriateEnclosingToppings[i].name))
				{
					validTopping = true;
					break;
				}
			}

			if(!validTopping)
			{
				int index = Random.Range(0, appropriateEnclosingToppings.Count);
				Vector3 defaultToppingLocation = determinePositionToInstantiate();
				GameObject defaultBase = appropriateEnclosingToppings [index];
				if (defaultBase.name.Contains ("Waffle")) 
				{
					defaultToppingLocation.x = -2.739f;
				}

				plateClone = Object.Instantiate (appropriateEnclosingToppings[index], defaultToppingLocation, Quaternion.identity);
				selectedItems.Add (plateClone);
			}
		}

		// Determine the position of where the topping should be instantiated
		Vector3 chosenToppingLocation = determinePositionToInstantiate();

		// Only the cheese game object needs to be rotated -90 degree when being instantiated for the sandwich.
		if (objectToInstantiate.name.Contains("CheeseSlice")) 
		{
			plateClone = Object.Instantiate (objectToInstantiate, chosenToppingLocation, Quaternion.Euler (-90, 0, 0));
		} 
		else 
		{
			plateClone = Object.Instantiate (objectToInstantiate, chosenToppingLocation, Quaternion.identity);
		}

		selectedItems.Add (plateClone);
	}

	/*
	 * Determine how high above the plate the chosen topping should be instantiated at.
	 * Returns a Vector3 of the x, y, and z position for the object's instantiation.
	 */
	Vector3 determinePositionToInstantiate()
	{
		GameObject lastToppingAdded = selectedItems [selectedItems.Count - 1];
		Renderer lastToppingRenderer = lastToppingAdded.GetComponentsInChildren<Renderer> ()[0];

		float lastToppingAddedYPosition = lastToppingAdded.transform.position.y;
		float lastToppingAddedSizeY = lastToppingRenderer.bounds.size.y;

		// x and z will have a default position where it's default value is the same as the plate. 
		// This will only need to change for a few objects to centre the game object. 
		// y will change for all objects to ensure that each game object is stacked on the previously 
		// added one.
		float xPos = -2.739f;
		float yPos = lastToppingAddedSizeY + lastToppingAddedYPosition;
		float zPos = -8.182f;

		// Adjust the y-position if the last topping that was added was steak and current topping that the user
		// is not steak otherwise don't do anything
		if ((lastToppingAdded.name.Contains ("Steak")) && (!objectToInstantiate.name.Contains("Steak")))
		{
			yPos += 0.02705f;
		}
		// Adjust the y-position if the last topping that was added was chicken and current topping that the user
		// is not chicken otherwise don't do anything
		else if ((lastToppingAdded.name.Contains ("Chicken")) && (!objectToInstantiate.name.Contains("Chicken"))) 
		{
			yPos -= 0.073629f;
		}
		// Adjust the y-position if the last topping that was added was LettucePiece and current topping that the user
		// is not LettucePiece otherwise don't do anything
		else if ((lastToppingAdded.name.Contains ("LettucePiece")) && (!objectToInstantiate.name.Contains("LettucePiece"))) 
		{
			yPos -= 0.04f;
		}

		// Some imported topping prefabs have an offset and therefore will not be instantiated where I want it to be.
		// To compensate for this, I am manually adjusting its position.
		if (objectToInstantiate.name.Contains ("Bacon")) 
		{
			xPos = -2.6790001f;
			if (lastToppingAdded.name.Contains ("Shrimp")) 
			{
				yPos -= 0.023433f;
			}
		}
		else if (objectToInstantiate.name.Contains ("Waffle")) 
		{
			if (lastToppingAdded.name.Contains ("CheeseSlice")) 
			{
				yPos += 0.018581f;
			}
			else if(lastToppingAdded.name.Contains("Chicken"))
			{
				yPos += 0.02016f;
			}
			else if(lastToppingAdded.name.Contains("Sausage"))
			{
				yPos += 0.01324f;
			}
			else if(lastToppingAdded.name.Contains("Steak"))
			{
				yPos -= 0.00749f;
			}
		} 
		else if (objectToInstantiate.name.Contains ("Toast")) 
		{
			if (lastToppingAdded.name.Contains ("CheeseSlice")) 
			{
				yPos += 0.018581f;
			}
			else if (lastToppingAdded.name.Contains ("Chicken")) 
			{
				yPos += 0.02138f;
			}
		} 
		else if (objectToInstantiate.name.Contains ("Egg")) 
		{
			xPos += 0.159f;
			yPos -= 0.015594f;
			zPos += 0.032f;
			if (lastToppingAdded.name.Contains ("Shrimp")) 
			{
				yPos -= 0.015338f;
			}
		} 
		else if (objectToInstantiate.name.Contains ("Chicken")) 
		{
			if (lastToppingAdded.name.Contains ("Chicken")) 
			{
				xPos += 0.029f;
			}
			else if (lastToppingAdded.name.Contains ("Sausage")) 
			{
				yPos += 0.047395f;
			}
			else 
			{
				yPos += 0.036739f;
				zPos -= 0.028f;
			}
		} 
		else if (objectToInstantiate.name.Contains ("Sausage")) 
		{
			xPos += 0.0099999f;
			zPos += 0.062f;
			if (lastToppingAdded.name.Contains ("Shrimp")) 
			{
				yPos -= 0.026307f;
			}
			else if (lastToppingAdded.name.Contains ("Steak")) 
			{
				yPos -= 0.015335f;
			}
		} 
		else if (objectToInstantiate.name.Contains ("Steak")) 
		{
			if (!(lastToppingAdded.name.Contains ("CheeseSlice"))) 
			{
				yPos -= 0.02246f;
			} 
			else if (lastToppingAdded.name.Contains ("CheeseSlice")) 
			{
				yPos -= 0.016572f;
			}
		}
		else if (objectToInstantiate.name.Contains ("CheeseSlice")) 
		{
			if (lastToppingAdded.name.Contains ("Toast")) 
			{
				yPos -= 0.0148f;
			}
			else if (lastToppingAdded.name.Contains ("Steak")) 
			{
				yPos -= 0.01912f;
			}
		}
		else if (objectToInstantiate.name.Contains ("Breadslice")) 
		{
			if (lastToppingAdded.name.Contains ("Hamslice")) 
			{
				yPos -= 0.012604f;
			}
			else if (lastToppingAdded.name.Contains ("Chicken")) 
			{
				yPos += 0.011396f;
			}
			else if (!(lastToppingAdded.name.Contains ("CheeseSlice"))) 
			{
				yPos -= 0.00607f;
			}
		}
		return new Vector3(xPos, yPos, zPos);
	}

	/*
	 * Returns the list containing all toppings that were selected. 
	 * It is in order of selection: element 0 contains the plate
	 * game object, element 1 contains the first chosen topping and so
	 * on.
	 */
	public static List<GameObject> selectedItemsInSandwich()
	{
		return selectedItems;
	}
}