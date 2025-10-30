using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public Transform target;
    public RectTransform arrowUI;
    public Camera cam;
    public float edgeOffset = 0;

    void Update()
    {
        if (!target) return;

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);
        bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if (!isOffScreen)
        {
            arrowUI.gameObject.SetActive(false);
            return;
        }

        arrowUI.gameObject.SetActive(true);

        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
        screenPos -= screenCenter;

        if (screenPos.z < 0) screenPos *= -1;

        float angle = Mathf.Atan2(screenPos.y, screenPos.x) * Mathf.Rad2Deg;
        arrowUI.rotation = Quaternion.Euler(0, 0, angle);

        float x = Mathf.Clamp(screenPos.x, -screenCenter.x + edgeOffset, screenCenter.x - edgeOffset);
        float y = Mathf.Clamp(screenPos.y, -screenCenter.y + edgeOffset, screenCenter.y - edgeOffset);

        arrowUI.position = screenCenter + new Vector3(x, y, 0);
    }
}
