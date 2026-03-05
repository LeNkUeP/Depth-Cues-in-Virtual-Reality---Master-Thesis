using UnityEngine;

public class DiplopiaEffect : MonoBehaviour
{
    public OVRCameraRig ovrCameraRig;
    public Transform leftEyeObj;   // Child für linkes Auge
    public Transform rightEyeObj;  // Child für rechtes Auge

    public float maxOffset = 0.03f;   // volle Verschiebung
    public float minDistance = 0.2f;  // Abstand, ab dem der Effekt beginnt

    private Transform leftEye;
    private Transform rightEye;

    void Start()
    {
        if (ovrCameraRig == null) { Debug.LogError("OVRCameraRig fehlt!"); enabled = false; return; }
        leftEye = ovrCameraRig.leftEyeAnchor;
        rightEye = ovrCameraRig.rightEyeAnchor;
        if (leftEyeObj == null || rightEyeObj == null) { Debug.LogError("Child-Objekte fehlen!"); enabled = false; return; }
    }

    void Update()
    {
        Vector3 center = (leftEye.position + rightEye.position) / 2f;
        Vector3 objPos = transform.position;

        float distanceToCenter = Vector3.Distance(objPos, center);

        // 0 wenn weiter als minDistance, 1 wenn direkt am Kopf
        float proximityFactor = Mathf.Clamp01(1f - (distanceToCenter / minDistance));

        // Smooth Übergang
        proximityFactor = Mathf.SmoothStep(0f, 1f, proximityFactor);

        Vector3 eyeAxis = (rightEye.position - leftEye.position).normalized;
        float horizontalDistance = Vector3.Dot(objPos - center, eyeAxis);

        // Maximale Stärke wenn exakt mittig
        float centerFactor = 1f - Mathf.Clamp01(Mathf.Abs(horizontalDistance) / minDistance);

        float diplopiaAmount = maxOffset * proximityFactor * centerFactor;

        Vector3 eyeOffset = eyeAxis * diplopiaAmount;

        leftEyeObj.position = transform.position - eyeOffset;
        rightEyeObj.position = transform.position + eyeOffset;
    }
}