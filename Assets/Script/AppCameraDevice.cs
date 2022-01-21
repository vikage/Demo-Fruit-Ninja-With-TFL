using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;

public class AppCameraDevice : MonoBehaviour
{
    [System.Serializable]
    public class TextureUpdateEvent : UnityEvent<Texture> { }

    private bool camAvailable;
    private WebCamTexture camera;

    [SerializeField]
    private RawImage background;
    [SerializeField]
    private AspectRatioFitter fit;

    [SerializeField] private int requestFps = 60;

    public TextureUpdateEvent OnTextureUpdate = new TextureUpdateEvent();

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
                camera = new WebCamTexture(camName, Screen.width, Screen.height, requestFps);
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
    }

    // Update is called once per frame
    private void Update()
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

        OnTextureUpdate.Invoke(background.texture);
    }

}
