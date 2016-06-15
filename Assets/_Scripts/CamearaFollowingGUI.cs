using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CamearaFollowingGUI : MonoBehaviour
{

    [SerializeField] float sensitivity = 1F;
    [SerializeField] float smoothTime = 0.3F;
    [SerializeField] float rayLength = 3F;
    [SerializeField] LayerMask targetLayers;

    Camera mainCamera;
    float yTargetAngle;
    float yAngleVelocity;
    bool isRecentering;

    void Start()
    {
        mainCamera = Camera.main;

        this.UpdateAsObservable()
            .Subscribe(_ => transform.position = mainCamera.transform.position);

        this.UpdateAsObservable()
            .Select(_ => mainCamera.transform.eulerAngles.y)
            .Buffer(60, 1)
            .Select(samples => !isRecentering && !isPlayerFocusingToUI() && isPlayerStaring(samples))
            .Buffer(30, 1)
            .Where(samples => samples.All(b => b))
            .Subscribe(_ =>
            {
                yTargetAngle = mainCamera.transform.eulerAngles.y;
                isRecentering = true;
            });

        this.UpdateAsObservable()
            .Where(_ => isRecentering)
            .Select(_ => Mathf.SmoothDampAngle(transform.eulerAngles.y, yTargetAngle, ref yAngleVelocity, smoothTime))
            .Subscribe(y => transform.rotation = Quaternion.Euler(0F, y, 0F));

        this.UpdateAsObservable()
            .Where(_ => Mathf.Abs(yAngleVelocity) < 0.1F)
            .Subscribe(_ => isRecentering = false);
    }

    bool isPlayerFocusingToUI()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        return Physics.Raycast(ray, rayLength, targetLayers);
    }

    bool isPlayerStaring(IList<float> samples)
    {
        var average = samples.Average();
        var dispersion = samples.Select(d => Mathf.Pow(d - average, 2F)).Average();
        return dispersion < sensitivity;
    }
}
