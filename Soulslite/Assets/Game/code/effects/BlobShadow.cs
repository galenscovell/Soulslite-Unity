using UnityEngine;


public class BlobShadow : MonoBehaviour
{

    public void LerpLocalPosition(Vector2 target, float overTime)
    {
        LeanTween.moveLocal(gameObject, target, overTime);
    }

    public void LerpWorldPosition(Vector2 target, float overTime)
    {
        LeanTween.move(gameObject, target, overTime);
    }

    public void LerpScale(Vector2 target, float overTime)
    {
        LeanTween.scale(gameObject, target, overTime);
    }

    public void LerpScaleInThenOut(Vector2 original, Vector2 target, float overTime)
    {
        LeanTween.scale(gameObject, target, overTime / 2);
        LeanTween.scale(gameObject, original, overTime / 2).setDelay(overTime / 2);
    }


    public void SetWorldPosition(Vector2 target)
    {
        transform.position = target;
    }


    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
