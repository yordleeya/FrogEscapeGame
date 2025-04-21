using UnityEngine;
using UnityEngine.InputSystem;

public class TongueShotRange : MonoBehaviour
{
    [SerializeField]
    PlayerStats stat;
    float expandSpeed;

    [SerializeField]
    RectTransform circleTransform;

    bool isHolding;

    private void Start()
    {
        expandSpeed = stat.TongueRangeExpandSpeed;
    }

    void Update()
    {
        if(isHolding)
        {
            float delta = expandSpeed * Time.deltaTime;
            circleTransform.sizeDelta += new Vector2(delta, delta);
        }
    }


    public void OnInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            circleTransform.gameObject.SetActive(true);
            isHolding = true;
        }
        else if(context.canceled)
        {
            circleTransform.gameObject.SetActive(false);
            isHolding = false;
        }
    }


}
