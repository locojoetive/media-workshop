using System.Runtime.InteropServices;
using UnityEngine;

public class DetectMobileBrowser : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool IsMobileBrowser();

    void Start()
    {
        bool isMobile = false;

        #if !UNITY_EDITOR && UNITY_WEBGL
            isMobile = IsMobileBrowser();
        #endif

        if (!isMobile)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
