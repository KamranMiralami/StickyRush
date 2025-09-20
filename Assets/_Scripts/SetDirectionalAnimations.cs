using UnityEngine;

public class SetDirectionalAnimations : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] GameObject upGameObject;
    [SerializeField] GameObject downGameObject;
    [SerializeField] GameObject leftGameObject;
    [SerializeField] GameObject rightGameObject;
    
    public void SetDirection(Vector2 dir)
    {
        //Change animation according to direction 
        if (dir == Vector2.up && upGameObject)
        {
            TurnOffAllAnimations();
            upGameObject.SetActive(true);
        }
        else if (dir == Vector2.down && downGameObject)
        {
            TurnOffAllAnimations();
            downGameObject.SetActive(true);
        }
        else if (dir == Vector2.left && leftGameObject)
        {
            TurnOffAllAnimations();
            leftGameObject.SetActive(true);
        }
        else if (dir == Vector2.right && rightGameObject)
        {
            TurnOffAllAnimations();
            rightGameObject.SetActive(true);
        }
    }
    
    private void TurnOffAllAnimations()
    {
        if (upGameObject)
            upGameObject.SetActive(false);
        if (downGameObject)
            downGameObject.SetActive(false);
        if (leftGameObject)
            leftGameObject.SetActive(false);
        if (rightGameObject)
            rightGameObject.SetActive(false);
    }
}
