using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MoveNetHandle : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string fileName = default;

    [SerializeField]
    private RawImage cameraView = null;

    [SerializeField]
    private AppCameraDevice appCameraDevice;

    [SerializeField, Range(0, 1)]
    private float threshold = 0.3f;
    private PoseNet poseNet;
    private PoseNet.Result[] results;
    private CancellationToken cancellationToken;
    private UniTask<bool> task;

    private readonly Vector3[] rtCorners = new Vector3[4];

    private RectTransform rect;

    [SerializeField]
    private Blade blade;

    [SerializeField]
    private bool runBackground;

    void Start()
    {
        rect = cameraView.GetComponent<RectTransform>();
        appCameraDevice.OnTextureUpdate.AddListener(OnTextureUpdate);
        poseNet = new PoseNet(fileName);
    }

    // Update is called once per frame
    void Update()
    {
        HandleResult();
    }

    private void OnTextureUpdate(Texture texture)
    {
        if (runBackground)
        {
            if (task.Status.IsCompleted())
            {
                task = InvokeAsync(texture);
            }
        }
        else
        {
            HandleMoveNet(texture);
        }
    }

    private async UniTask<bool> InvokeAsync(Texture texture)
    {
        results = await poseNet.InvokeAsync(texture, cancellationToken);
        return true;
    }
    private void HandleMoveNet(Texture texture)
    {
        poseNet.Invoke(texture);
        results = poseNet.GetResults();
    }

    private void HandleResult()
    {
        if (results == null || results.Length == 0)
        {
            return;
        }
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        var connections = MoveNet.Connections;
        int len = connections.GetLength(0);
        var resultNose = results[(int)MoveNet.Part.NOSE];
        if (resultNose.confidence >= threshold)
        {
            Vector3 nosePositionFix = MathTF.Lerp(min, max, new Vector3(1 - resultNose.x, 1f - resultNose.y, 0));
            blade.SetCuttingPosition(new Vector2(nosePositionFix.x, nosePositionFix.y));
        }
    }

    private void OnDestroy()
    {
        poseNet?.Dispose();
    }

}
