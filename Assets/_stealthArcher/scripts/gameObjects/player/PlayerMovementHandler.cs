using System.Collections;
using System.Collections.Generic;
using _stealthArcher;
using _stealthArcher.scripts;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementHandler : MonoBehaviour {

    private Joystick joystick;
    private CharacterController characterController;

    [SerializeField] private float speed;
    [SerializeField] private GameObject playerMovementVisualizerObj;
    [SerializeField] private float inputVisualizerMultiplier;       // Max distance visualizer can be from player
    
    [SerializeField] private float gravity = -9.81f;
    private float currentVerticalVelocity;

    
    void Start() {
        this.joystick = new Joystick(SpecificTouch.Left, (joystickEvent, joystickData) => {
            switch (joystickEvent) {
                case JoystickEvent.Hold:
                    Move(joystickData.TiltPercentage, joystickData.AroundPlayerPosition);
                    break;
                case JoystickEvent.Up:
                    HideMovementVisualizer();
                    break;
            }
        });
        this.characterController = GetComponent<CharacterController>();
        playerMovementVisualizerObj.transform.position = PlayerFeetPosition();
    }

    public void HandleMovement() {
        HandleKeyboardControls();
        joystick.HandleInput();
        applyGravity();
    }

    // Needed because the CharacterController component doesn't apply gravity itself
    private void applyGravity() {
        if (characterController.isGrounded) {
            if (currentVerticalVelocity < 0) {
                // Player is on ground, maintain some gravity so they stay there
                float groundedGravity = -2;
                currentVerticalVelocity = groundedGravity;
            }
        } else {
            // Player is not on ground, have them fall
            currentVerticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 gravityMovement = new Vector3(0, currentVerticalVelocity, 0);
        characterController.Move(gravityMovement * Time.deltaTime);
    }

    // Returns true if keyboard input was detected
    private bool HandleKeyboardControls() {
        // Angled movement
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) {
            Vector3 angledDirection = Vector3.Lerp(characterController.transform.forward, -characterController.transform.right, 0.5f);
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, angledDirection, 1));
            return true;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) {
            Vector3 angledDirection = Vector3.Lerp(characterController.transform.forward, characterController.transform.right, 0.5f);
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, angledDirection, 1));
            return true;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) {
            Vector3 angledDirection = Vector3.Lerp(-characterController.transform.right, -characterController.transform.forward, 0.5f);
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, angledDirection, 1));
            return true;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) {
            Vector3 angledDirection = Vector3.Lerp(characterController.transform.right, -characterController.transform.forward, 0.5f);
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, angledDirection, 1));
            return true;
        }
        
        // One-directional movement
        if (Input.GetKey(KeyCode.W)) {
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, characterController.transform.forward, 1));
            return true;
        }

        if (Input.GetKey(KeyCode.A)) {
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, -characterController.transform.right, 1));
            return true;
        }

        if (Input.GetKey(KeyCode.S)) {
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, -characterController.transform.forward, 1));
            return true;
        }

        if (Input.GetKey(KeyCode.D)) {
            Move(1, VectorUtil.NewPointInDirection(characterController.transform.position, characterController.transform.right, 1));
            return true;
        }
        
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) {
            HideMovementVisualizer();
        }
        
        return false;
    }
    
    private void Move(float tiltPercentage, Vector3 aroundPlayerPosition) {
        Vector3 direction = (aroundPlayerPosition - this.transform.position).normalized;
        direction.y = 0;        // Prevent vertical movement along the Y-axis
        characterController.Move(direction * (tiltPercentage * speed) * Time.deltaTime);
        
        DisplayMovementVisualizer(tiltPercentage, direction);
    }

    private void DisplayMovementVisualizer(float tiltPercentage, Vector3 direction) {
        Vector3 position = VectorUtil.NewPointInDirection(PlayerFeetPosition(), direction, tiltPercentage * inputVisualizerMultiplier);
        playerMovementVisualizerObj.transform.position = position;
    }
    
    private void HideMovementVisualizer() {
        playerMovementVisualizerObj.transform.position = PlayerFeetPosition();
    }

    private Vector3 PlayerFeetPosition() {
        float halfPlayerHeight = characterController.transform.localScale.y * 0.5f;
        return new Vector3(
            characterController.transform.position.x,
            characterController.transform.position.y - halfPlayerHeight,
            characterController.transform.position.z
        );
    }
}
        
