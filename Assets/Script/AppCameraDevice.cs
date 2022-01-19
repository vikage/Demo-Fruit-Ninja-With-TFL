using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;

public class AppCameraDevice : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string fileName = default;

    private bool camAvailable;
    private WebCamTexture camera;

    [SerializeField]
    private RawImage background;
    [SerializeField]
    private AspectRatioFitter fit;

    private MoveNet moveNet;
    private MoveNet.Result[] results;
    private readonly Vector3[] rtCorners = new Vector3[4];

    [SerializeField]
    private Blade blade;

    private void Start()
    {
        WebCamDevice[] cams = WebCamTexture.devices;

        if (cams.Length == 0)
        {
            Debug.Log("No cam");
            camAvailable = false;
            return;
        }

        string camName = "";
        for(int i = 0; i < cams.Length; i++)
        {
            if (cams[i].isFrontFacing)
            {
                camName = cams[i].name;
                camera = new WebCamTexture(camName, Screen.width, Screen.height);
                break;
            }
        }

        if (camera == null)
        {
            Debug.Log("No front camera");
            camAvailable = false;
            return;
        }
        camera.Play();
        background.texture = camera;

        camAvailable = true;
        moveNet = new MoveNet(fileName);
    }


    // Update is called once per frame
    private void LateUpdate()
    {
        if (!camAvailable)
        {
            return;
        }
        float ratio = (float)camera.width / (float)camera.height;
        fit.aspectRatio = ratio;

        float scaleY = camera.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orientation = -camera.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);

        HandleMoveNet(background.texture);
    }

    private void HandleMoveNet(Texture texture)
    {
        moveNet.Invoke(texture);
        results = moveNet.GetResults();
        HandleResult(results);   
    }

    private void HandleResult(MoveNet.Result[] results)
    {
        if (results == null || results.Length == 0)
        {
            return;
        }

        var rect = background.GetComponent<RectTransform>();
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        var connections = MoveNet.Connections;
        int len = connections.GetLength(0);
        var resultNose = results[(int)MoveNet.Part.NOSE];
        Vector3 nosePositionFix = MathTF.Lerp(min, max, new Vector3(1-resultNose.x, 1f - resultNose.y, 0));
        //Vector3 nosePosition = new Vector3(resultNose.x, resultNose.y, 0);
        blade.SetCuttingPosition(new Vector2(nosePositionFix.x,nosePositionFix.y));
        Debug.Log("Screen: (" + Screen.width + ", " + Screen.height + ")");
        Debug.Log("Nose Position Fix: " + nosePositionFix);

    }

    private void OnDestroy()
    {
        moveNet?.Dispose();
    }
}
