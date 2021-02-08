//Gameready assets, tips & tricks, tutorials www.not-lonely.com

//Usage:
//Put this script to Assets/Editor/

//Hotkeys:
//Deselect all by pressing Shift + D
//Create a Cube ahead the scene camera by pressing Shift + 1
//Create a Point Light ahead the scene camera by pressing Shift + 2
//Create a Spotlight ahead the scene camera by pressing Shift + 3

using UnityEngine;
using UnityEditor;

public class ExtendedHotkeys : ScriptableObject {
    /*
	public static GameObject go;
	public static Vector3 goPos;
	
    //Deselect all by pressing Shift + D
	[MenuItem ("NOT_Lonely/Deselect All #_d")]
	static void DoDeselect(){
		Selection.objects = new UnityEngine.Object[0];
	}

	//Create a Cube ahead the scene camera by pressing Shift + 1
	[MenuItem ("NOT_Lonely/Create Cube #_1")]
	static void AddCube(){
		go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		AfterCreation ();
	}

	//Create a Point Light ahead the scene camera by pressing Shift + 2
	[MenuItem ("NOT_Lonely/Create Point Light #_2")]
	static void AddPointLight(){
		go = new GameObject ("Point Light");
		go.AddComponent<Light> ().type = LightType.Point;
		AfterCreation ();
	}

	//Create a Spotlight ahead the scene camera by pressing Shift + 3
	[MenuItem ("NOT_Lonely/Create Spotlight #_3")]
	static void AddSpotlight(){
		go = new GameObject ("Spotlight");
		go.AddComponent<Light> ().type = LightType.Spot;
		go.transform.eulerAngles = new Vector3 (90, 0, 0);
		AfterCreation ();
	}
	static void AfterCreation(){
		goPos = SceneView.currentDrawingSceneView.camera.transform.TransformPoint (Vector3.forward * 1.1f);
		go.transform.position = goPos;
		Undo.RegisterCreatedObjectUndo (go, "Create " + go.name);
		Selection.activeObject = go;
	}
		*/
}