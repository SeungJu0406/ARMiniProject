using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
//        if (Input.touchCount == 0) return;
//        if (Input.GetTouch(0).phase == TouchPhase.Began)
//        {
//#if UNITY_EDITOR
//            Vector3 touchPos = Input.mousePosition;
//#else
//        Vector3 touchPos = Input.touches[0].position;
//#endif
//            Ray ray = Camera.main.ScreenPointToRay(touchPos);
//            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f))
//            {
//            }
        

    }


}
