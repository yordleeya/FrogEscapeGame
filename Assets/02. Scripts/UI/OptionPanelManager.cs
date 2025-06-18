using UnityEngine;
using UnityEngine.InputSystem;

public class OptionPanelManager : MonoBehaviour
{
    public GameObject optionPanel; // OptionPanel 오브젝트를 드래그해서 연결

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.UI.Cancel.performed += OnCancel;
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.UI.Cancel.performed -= OnCancel;
        inputActions.UI.Disable();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        bool isActive = !optionPanel.activeSelf;
        optionPanel.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
