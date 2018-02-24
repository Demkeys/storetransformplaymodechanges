/// <summary>
/// -----------------------------
/// Transform Changes tool
/// -----------------------------
/// This is a simple tool that allows you to store any changes made to the Position, Rotation and Scale of all Gameobjects in the scene
/// during PlayMode, and apply those changes to all the Gameobjects after exiting PlayMode.
/// Instructions:
/// -Place this script in the Editor folder in your project. Then the 'MyTools' menu should appear in your MenuBar with the 
/// Store and Apply options.
/// -Enter PlayMode. Make changes to the Postion, Rotation and Scale of gameobjects in the scene. Click 'MyTools/Store Transform PlayMode Changes'.
/// -Exit PlayMode. Click 'MyTools/Apply Transform PlayMode Changes'. All the changes you made to the Position, Rotation and Scale of 
/// any gameobjects in the scene, will be applied.
/// -----------------------------
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformChanges :  Editor
{
	static GameObject[] allGameobjectsArr; // Store all the gameobjects present during PlayMode
	static Dictionary<int, Vector3> allPositionsDict; // Stores gameobject InstanceIDs and positions
	static Dictionary<int, Quaternion> allRotationsDict; // Stores gameobject InstanceIDs and rotations
	static Dictionary<int, Vector3> allScalesDict; // Stores gameobject InstanceIDs and scales

	// This method should be called when Editor is in PlayMode. It stores the Position, Rotation and Scale of all Gameobjects in the scene.
	[MenuItem("My Tools/Store Transform PlayMode Changes")]
	public static void StoreChanges()
	{
		// If Editor is in PlayMode
		if(EditorApplication.isPlaying)
		{
			allGameobjectsArr = GameObject.FindObjectsOfType<GameObject>(); // Find all objects of type Gameobject in scene
			allPositionsDict = new Dictionary<int, Vector3>(); 
			allRotationsDict = new Dictionary<int, Quaternion>();
			allScalesDict = new Dictionary<int, Vector3>();

			// Populate all three dictionaries with initial InstanceIDs along with positions, rotations and scales during PlayMode
			foreach(GameObject g in allGameobjectsArr)
			{
				allPositionsDict.Add(g.GetInstanceID(), g.transform.localPosition);
				allRotationsDict.Add(g.GetInstanceID(), g.transform.localRotation);
				allScalesDict.Add(g.GetInstanceID(), g.transform.localScale);
			}
		}
		// If Editor is not in PlayMode
		else
		{
			Debug.LogWarning("You can only Store changes when Editor is in Play Mode");
		}
	}

	// This method should be called after exiting PlayMode. It sets the Position, Rotation and Scale of all Gameobjects back to
	// whatever they were when the Editor was in PlayMode.
	[MenuItem("My Tools/Apply Transform PlayMode Changes")]
	public static void ApplyChanges()
	{
		// If Editor is not in PlayMode
		if(!EditorApplication.isPlaying)
		{
			// Dictionary to store instanceIDs and gameobjects that exist in the scene outside of PlayMode.
			Dictionary<int, GameObject> nonPlayModeGameobjectsDict = new Dictionary<int, GameObject>();

			// Make sure the array isn't null. Just in case...
			if(allGameobjectsArr != null)
			{
				// Populate nonPlayModeGameobejctsDict with gameobjects and their InstanceIDs from allGameobjectsArr.
				foreach(GameObject g in allGameobjectsArr)
				{
					nonPlayModeGameobjectsDict.Add(g.GetInstanceID(), g);
				}

				// Go over every keyvaluepair in nonPlayModeGameobjectsDict
				foreach(KeyValuePair<int, GameObject> pair in nonPlayModeGameobjectsDict)
				{
					// Temp variables to pass in as 'out' arguments to the TryGetValue method
					Vector3 pos;
					Quaternion rot;
					Vector3 scale;

					// Check to see gameobject still exists in scene becaue gameobjects added to scene during PlayMode will
					// get destroyed after stopping PlayMode, so this check makes sure the gameobject still exists in scene.
					if(pair.Value != null)
					{
						// If InstanceID is found, store the corresponding localposition in pos
						if(allPositionsDict.TryGetValue(pair.Key, out pos))
						{
							pair.Value.transform.localPosition = pos;
						}

						// If InstanceID is found, store the corresponding localrotation in rot
						if(allRotationsDict.TryGetValue(pair.Key, out rot))
						{
							pair.Value.transform.localRotation = rot;
						}

						// If InstanceID is found, store the corresponding localscale in scale
						if(allScalesDict.TryGetValue(pair.Key, out scale))
						{
							pair.Value.transform.localScale = scale;
						}
					}
				}

				// Clear the all the dictionaries. Also set them to null just in case.
				allGameobjectsArr = null;
				allPositionsDict.Clear();
				allPositionsDict = null;
				allRotationsDict.Clear();
				allRotationsDict = null;
				allScalesDict.Clear();
				allScalesDict = null;
			}
		}
		// If Editor is in PlayMode
		else
		{
			Debug.LogWarning("Cannot apply changes when Editor is in Play Mode.");
			
		}
	}
}
