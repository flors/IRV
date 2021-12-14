using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class TargetManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public Vector3 scaleFactor = new Vector3(0.1f, 0.1f, 0.1f);
    public Text messageUI;

    private ARTrackedImageManager trackedImageManager; //detects AR events

    // the key is the name of the target/image and the GO is the prefab associate to each image
    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>(); 

    // Start is called before the first frame update
    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        foreach (GameObject arObj in prefabs)
        {
            //Add prefabs to dictionary
            GameObject newARObject = Instantiate(arObj, Vector3.zero, Quaternion.identity);
            newARObject.name = arObj.name;
            arObjects.Add(arObj.name, newARObject);
        }
    }


    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {

        messageUI.text = "There are" + trackedImageManager.trackables.count + "images being tracked.";
        //Event when image get detected
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            //UpdateARImage(trackedImage);
            ListAllImages();
        }

        //Event called when we move the target 
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            //UpdateARImage(trackedImage);
            ListAllImages();
        }

        //Target lost
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            //Hide prefab when the target is lost
            messageUI.text = "Hide prefab:" + trackedImage.referenceImage.name;
            arObjects[trackedImage.referenceImage.name].SetActive(false);
            
        }
    }

    void ListAllImages()
    {
        

        foreach (var trackedImage in trackedImageManager.trackables)
        {

            messageUI.text = "Target: " + trackedImage.referenceImage.name +  "state is = " + trackedImage.trackingState.ToString();

            if (trackedImage.trackingState.ToString().Equals("Limited")) {
                arObjects[trackedImage.referenceImage.name].SetActive(false);
              //  messageUI.text = "Hide prefab with state NONE:" + trackedImage.referenceImage.name;
            }

            if (trackedImage.trackingState.ToString().Equals("Tracking"))
            {
               // messageUI.text = "Show prefab with state TRACKING:" + trackedImage.referenceImage.name;
                arObjects[trackedImage.referenceImage.name].SetActive(true);
                arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
                arObjects[trackedImage.referenceImage.name].transform.localScale = scaleFactor;
            }
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {

        // Assign and Place Game Object
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);

        Debug.Log("trackedImage.referenceImage.name:" + trackedImage.referenceImage.name);
        messageUI.text = "trackedImage.referenceImage.name:" + trackedImage.referenceImage.name;
    }

    void AssignGameObject(string name, Vector3 newPosition)
    {
        if (prefabs != null)
        {

            arObjects[name].SetActive(true);
            arObjects[name].transform.position = newPosition;
            arObjects[name].transform.localScale = scaleFactor;

            /* GameObject goARObject = arObjects[name];
            goARObject.SetActive(true);
            goARObject.transform.position = newPosition;
            goARObject.transform.localScale = scaleFactor;


            foreach (GameObject go in arObjects.Values)
            {
                Debug.Log("Go in arObjects.Values:" + go.name);
                if (go.name != name)
                {
                    go.SetActive(false);
                }
            }*/
        }
    }
}
