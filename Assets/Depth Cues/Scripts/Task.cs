using UnityEngine;

public abstract class Task : MonoBehaviour
{
    public GameObject environmentScene;
    public EnvironmentAnimationController environmentAnimationController;

    public void AnimateShowScene()
    {
        environmentAnimationController.AnimateShowObjects(environmentScene);
    }

    public void AnimateHideScene()
    {
        environmentAnimationController.AnimateHideObjects(environmentScene);
    }

    public void TaskFullfilled()
    {

    }
}
