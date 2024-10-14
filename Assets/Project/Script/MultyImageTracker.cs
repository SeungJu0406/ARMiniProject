using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultyImageTracker : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager imageManager;

    [SerializeField] GameObject chessUI;

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnImageChange;
    }
    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnImageChange;
    }

    void OnImageChange(ARTrackedImagesChangedEventArgs args)
    {
        foreach(ARTrackedImage image in args.added)
        {
            string imageName = image.referenceImage.name;
            switch (imageName)
            {
                case "Chess":
                    GameObject chess = Instantiate(chessUI, image.transform.position, image.transform.rotation);
                    chess.transform.SetParent(image.transform);
                    break;
            }
        }
        foreach(ARTrackedImage image in args.updated)
        {
            image.transform.GetChild(0).position = image.transform.position;
            image.transform.GetChild(0).rotation = image.transform.rotation;
        }
        foreach(ARTrackedImage image in args.removed)
        {
            Destroy(image.transform.GetChild(0).gameObject);
        }
    }
}
