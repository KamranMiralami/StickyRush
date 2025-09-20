using UnityEngine;

public class SetDirectionalAnimations : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] GameObject upGameObject;
    [SerializeField] GameObject downGameObject;
    
    public void SetDirection(Vector2 dir)
    {
        //Change animation according to direction 
        if (dir == Vector2.up)
        {
            TurnOffAllAnimations();
            upGameObject.SetActive(true);
        }
        else if (dir == Vector2.down)
        {
            TurnOffAllAnimations();
            downGameObject.SetActive(true);
        }
    }
    
    private void TurnOffAllAnimations()
    {
        upGameObject.SetActive(false);
        downGameObject.SetActive(false);
    }
}
