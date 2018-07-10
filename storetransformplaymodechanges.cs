/// <summary>
/// -----------------------------
/// Transform Changes tool
/// -----------------------------
/// This is a simple tool that allows you to store any changes made to the Position, Rotation and Scale of all Gameobjects in the scene
/// during PlayMode, and apply those changes to all the Gameobjects after exiting PlayMode.
/// Instructions:
/// -Place this script in the Editor folder in your project. Then the 'MyTools' menu should appear in your MenuBar with the 
/// option to open the TransformChangesTool window. The window has two buttons - Store Transform Changes and Apply Transform Changes.
/// The window is dockable so it can be placed anywhere in the editor.
/// In Play Mode the Store Transform Changes button is enabled. In Edit Mode the Apply Transform Changes button is enabled.
/// -Enter PlayMode. Make changes to the Postion, Rotation and Scale of gameobjects in the scene. Click 'Store Transform Changes'.
/// -Exit PlayMode. Click 'Apply Transform Changes'. All the changes you made to the Position, Rotation and Scale of any 
/// gameobjects in the scene, will be applied.
/// -----------------------------
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformChangesTool : EditorWindow {

	GameObject[] allGameobjectsArr; // Stores all the gameobjects present during PlayMode
	Dictionary<int, Vector3> allPositionsDict; // Stores gameobject InstanceIDs and positions
	Dictionary<int, Quaternion> allRotationsDict; // Stores gameobject InstanceIDs and rotations
	Dictionary<int, Vector3> allScalesDict; // Stores gameobject InstanceIDs and scales

	[MenuItem("My Tools/Transform Changes Tool")]
	static void ShowWindow()
	{
		EditorWindow window = EditorWindow.GetWindow<TransformChangesTool>();
		window.minSize = new Vector2(100, 50);
		// window.maxSize = new Vector2(200, 200);
		window.Show();
	}

	void OnGUI()
	{
		// If Editor is in Play Mode, enable "Store Transform Changes" button, otherwise disable it.
		EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
		if(GUILayout.Button("Store Transform Changes")) { StoreChanges(); }
		EditorGUI.EndDisabledGroup();

		// If Editor is in Play Mode, disable "Apply Transform Changes" button, otherwise enable it.
		EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
		if(GUILayout.Button("Apply Transform Changes")) { ApplyChanges(); }
		EditorGUI.EndDisabledGroup();

	}

	// This method should be called when Editor is in PlayMode. It stores the Position, Rotation and Scale of all Gameobjects in the scene.
	void StoreChanges()
	{
		// Retrieve all objects. This includes more than just the active and inactive gameobjects in the scene.
		Object[] allObjectsArr = Resources.FindObjectsOfTypeAll<GameObject>();
		List<GameObject> allGameObjectsList = new List<GameObject>(); // List to store the gameobjects in the scene.
		foreach(Object o in allObjectsArr)
		{
			// Go through allObjectsArr and pick out the ones that are gameobjects in the scene, and add to allGameObjectsList.
			if(o.hideFlags == HideFlags.None && PrefabUtility.GetPrefabType(o) != PrefabType.Prefab) allGameObjectsList.Add((GameObject)o);
		}
		allGameobjectsArr = allGameObjectsList.ToArray(); // Convert allGameObjectsList to array and store in allGameobjectsArr.
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

	// This method should be called after exiting PlayMode. It sets the Position, Rotation and Scale of all Gameobjects back to
	// whatever they were when the Editor was in PlayMode.
	void ApplyChanges()
	{
		// This line stops the method from progressing if the user randomly clicks the "Apply Transform Changes" button.
		if(allPositionsDict == null || allRotationsDict == null || allScalesDict == null) return;

		// Dictionary to store instanceIDs and gameobjects that exist in the scene outside of PlayMode.
		Dictionary<int, GameObject> nonPlayModeGameobjectsDict = new Dictionary<int, GameObject>();

		// Make sure the array isn't null. Just in case...
		if(allGameobjectsArr != null)
		{
			Transform[] allGameobjectsTransformsArr = new Transform[allGameobjectsArr.Length];
			for(int i = 0; i < allGameobjectsTransformsArr.Length; i++)
			{ allGameobjectsTransformsArr[i] = allGameobjectsArr[i].transform; }
			Undo.RecordObjects(allGameobjectsTransformsArr, "All Transforms");
			
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
}
