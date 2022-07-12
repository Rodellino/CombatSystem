using UnityEngine.InputSystem;
using UnityEngine;

// TODO: Singleton pattern

/// <summary>
/// Receives and handles input to be passed to the <see cref="MovementController"/>
/// </summary>
public sealed class InputHandler
{
    // Singleton implementation
    private static InputHandler _instance;

    private InputHandler(MovementController controller)
    {
        this._controller = controller;
        Init();
    }

    private InputHandler(CombatSystem combat)
    {
        this._combat = combat;
        Init();
    }


    // Reference to the controller
    private MovementController _controller;
    private CombatSystem _combat;

    // Reference to the Unity's Input System custom control set
    private Controls _controls;

    
    public static InputHandler StartInputHandler(MovementController controller)
    {
        if (_instance == null)
        {
            _instance = new InputHandler(controller);
        }
        else
        {
            _instance._controller = controller;
        }
        return _instance;
    }
    public static InputHandler StartInputHandler(CombatSystem combat)
    {
        if (_instance == null)
        {
            _instance = new InputHandler(combat);
        } else
        {
            _instance._combat = combat;
        }
        return _instance;
    }

    private void Init()
    {
        _controls = new Controls();
        _controls.Enable();

        _controls.Gameplay.Move.started += OnMove;
        _controls.Gameplay.Move.performed += OnMove;
        _controls.Gameplay.Move.canceled += OnMove;

        _controls.Gameplay.Dash.started += OnDash;
        _controls.Gameplay.Dash.performed += OnDash;
        _controls.Gameplay.Dash.canceled += OnDash;

        _controls.Gameplay.MeleeAttack.started += OnMeleeAttack;
        //_controls.Gameplay.MeleeAttack.performed += OnMeleeAttack;
        //_controls.Gameplay.MeleeAttack.canceled += OnMeleeAttack;

    }

    private void OnMove(InputAction.CallbackContext context)
    {
        var action = PlayerInput.Action.Move;

        var status = TranslateStatus(context);

        var value = context.ReadValue<Vector2>();

        var input = new PlayerInput.Input(action, status, value);

        _controller.HandleInput(input);
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        var action = PlayerInput.Action.Dash;

        var status = TranslateStatus(context);

        var input = new PlayerInput.Input(action, status);

        _controller.HandleInput(input);
    }

    private void OnMeleeAttack(InputAction.CallbackContext context)
    {
        var action = PlayerInput.Action.MeleeAttack;

        var status = TranslateStatus(context);

        var input = new PlayerInput.Input(action, status);

        _combat.HandleInput(input);
    }

    private PlayerInput.Status TranslateStatus(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            return PlayerInput.Status.Started;
        }
        else if (context.performed)
        {
            return PlayerInput.Status.Performed;
        }
        else
        {
            return PlayerInput.Status.Canceled;
        }
    }


    ~InputHandler()
    {
        _controls.Disable();
        _controls.Gameplay.Move.Disable();
        _controls.Gameplay.Dash.Disable();
    }
}
