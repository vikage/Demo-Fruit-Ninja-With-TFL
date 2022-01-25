using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using Cysharp.Threading.Tasks;
using System.Threading;

public class IndentifyHandle : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string fileNamePoseNet = default;

    [SerializeField, FilePopup("*.tflite")]
    private string fileNameMoveNet = default;

    [SerializeField]
    private RawImage cameraView = null;

    [SerializeField]
    private AppCameraDevice appCameraDevice;

    [SerializeField, Range(0, 1)]
    private float threshold = 0.3f;
    private PoseNet poseNet;
    private PoseNet.Result[] resultsPoseNet;
    private MoveNet moveNet;
    private MoveNet.Result[] resultsMoveNet;
    private CancellationToken cancellationToken;
    private UniTask<bool> task;

    private readonly Vector3[] rtCorners = new Vector3[4];

    private RectTransform rect;

    [SerializeField]
    private Blade blade;

    [SerializeField]
    private bool runBackground;

    [SerializeField]
    private bool usingPoseNet = false;

    void Start()
    {
        rect = cameraView.GetComponent<RectTransform>();
        appCameraDevice.OnTextureUpdate.AddListener(OnTextureUpdate);
        if (usingPoseNet)
            poseNet = new PoseNet(fileNamePoseNet);
        else
            moveNet = new MoveNet(fileNameMoveNet);
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
            if (usingPoseNet)
            {
                CheckAsyncPoseNet(texture);
            }
            else
            {
                CheckAsyncMoveNet(texture);
            }

        }
        else
        {
            HandleTextureSync(texture);
        }
    }

    private void CheckAsyncMoveNet(Texture texture)
    {
        if (task.Status.IsCompleted())
        {
            task = HandleMoveNetAsync(texture);
        }
    }
    private void CheckAsyncPoseNet(Texture texture)
    {
        if (task.Status.IsCompleted())
        {
            task = HandlePoseNetAsync(texture);
        }
    }

    private async UniTask<bool> HandlePoseNetAsync(Texture texture)
    {
        resultsPoseNet = await poseNet.InvokeAsync(texture, cancellationToken);
        return true;
    }

    private async UniTask<bool> HandleMoveNetAsync(Texture texture)
    {
        resultsMoveNet = await moveNet.InvokeAsync(texture, cancellationToken);
        return true;
    }

    private void HandleTextureSync(Texture texture)
    {
        if (usingPoseNet)
        {
            poseNet.Invoke(texture);
            resultsPoseNet = poseNet.GetResults();
        }
        else
        {
            moveNet.Invoke(texture);
            resultsMoveNet = moveNet.GetResults();
        }
       
    }

    private void HandleResult()
    {
        HandleResultPoseNet();
        HandleResultMoveNet();
    }

    private void HandleResultMoveNet()
    {
        if (resultsMoveNet == null || resultsMoveNet.Length == 0)
        {
            return;
        }
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        var connections = MoveNet.Connections;
        int len = connections.GetLength(0);
        var resultNose = resultsMoveNet[(int)MoveNet.Part.NOSE];
        if (resultNose.confidence >= threshold)
        {
            Vector3 nosePositionFix = MathTF.Lerp(min, max, new Vector3(1 - resultNose.x, 1f - resultNose.y, 0));
            blade.SetCuttingPosition(new Vector2(nosePositionFix.x, nosePositionFix.y));
        }
    }

    private void HandleResultPoseNet()
    {
        if (resultsPoseNet == null || resultsPoseNet.Length == 0)
        {
            return;
        }
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        var connections = PoseNet.Connections;
        int len = connections.GetLength(0);
        var resultNose = resultsPoseNet[(int)PoseNet.Part.NOSE];
        if (resultNose.confidence >= threshold)
        {
            Vector3 nosePositionFix = MathTF.Lerp(min, max, new Vector3(1 - resultNose.x, 1f - resultNose.y, 0));
            blade.SetCuttingPosition(new Vector2(nosePositionFix.x, nosePositionFix.y));
        }
    }

    private void OnDestroy()
    {
        poseNet?.Dispose();
        moveNet?.Dispose();
    }

}
