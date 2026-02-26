using UnityEngine;

public class RenderQueueSwitcher : MonoBehaviour
{
    [Header("Zu wechselnde Materialien")]
    public Material materialA;
    public Material materialB;

    [Header("Render Queue Werte")]
    public int queueA = 3000;
    public int queueB = 3001;

    [Header("Wechsel-Interval in Sekunden")]
    public float switchInterval = 0.1f;

    private bool toggle = false;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= switchInterval)
        {
            timer = 0f;
            toggle = !toggle;

            if (toggle)
            {
                materialA.renderQueue = queueA;
                materialB.renderQueue = queueB;
            }
            else
            {
                materialA.renderQueue = queueB;
                materialB.renderQueue = queueA;
            }
        }
    }
}
