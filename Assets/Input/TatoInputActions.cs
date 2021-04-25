// GENERATED AUTOMATICALLY FROM 'Assets/Input/TatoInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @TatoInputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @TatoInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TatoInputActions"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""54195521-d744-4eb8-a106-51d04141b8f5"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""e71cc057-e0b5-4c00-8dad-d257e10a91e3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""StickDeadzone,NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveMouse"",
                    ""type"": ""Value"",
                    ""id"": ""75a6b9fd-3d34-426f-bdcc-d8878b736ae3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseStart"",
                    ""type"": ""Button"",
                    ""id"": ""c5a27d6b-9823-41ae-9193-ec90a822f87e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bbe13480-7dc2-461a-9d6d-8e5da19637a5"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef73c157-b2cf-4faf-85f5-9545fce43859"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13e1703f-ca36-4bd4-a803-465552bf29be"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_Move = m_Default.FindAction("Move", throwIfNotFound: true);
        m_Default_MoveMouse = m_Default.FindAction("MoveMouse", throwIfNotFound: true);
        m_Default_MouseStart = m_Default.FindAction("MouseStart", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Default
    private readonly InputActionMap m_Default;
    private IDefaultActions m_DefaultActionsCallbackInterface;
    private readonly InputAction m_Default_Move;
    private readonly InputAction m_Default_MoveMouse;
    private readonly InputAction m_Default_MouseStart;
    public struct DefaultActions
    {
        private @TatoInputActions m_Wrapper;
        public DefaultActions(@TatoInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Default_Move;
        public InputAction @MoveMouse => m_Wrapper.m_Default_MoveMouse;
        public InputAction @MouseStart => m_Wrapper.m_Default_MouseStart;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMove;
                @MoveMouse.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMoveMouse;
                @MoveMouse.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMoveMouse;
                @MoveMouse.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMoveMouse;
                @MouseStart.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMouseStart;
                @MouseStart.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMouseStart;
                @MouseStart.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMouseStart;
            }
            m_Wrapper.m_DefaultActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @MoveMouse.started += instance.OnMoveMouse;
                @MoveMouse.performed += instance.OnMoveMouse;
                @MoveMouse.canceled += instance.OnMoveMouse;
                @MouseStart.started += instance.OnMouseStart;
                @MouseStart.performed += instance.OnMouseStart;
                @MouseStart.canceled += instance.OnMouseStart;
            }
        }
    }
    public DefaultActions @Default => new DefaultActions(this);
    public interface IDefaultActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnMoveMouse(InputAction.CallbackContext context);
        void OnMouseStart(InputAction.CallbackContext context);
    }
}
