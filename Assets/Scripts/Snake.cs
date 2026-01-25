using UnityEngine.InputSystem;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;

    void Start()
    {
        
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard.wKey.wasPressedThisFrame)
        {
            _direction = Vector2.up;
        }
        else if (keyboard.sKey.wasPressedThisFrame)
        {
            _direction = Vector2.down;
        }
        else if (keyboard.aKey.wasPressedThisFrame)
        {
            _direction = Vector2.left;
        }
        else if (keyboard.dKey.wasPressedThisFrame)
        {
            _direction = Vector2.right;
        }
    }

    private void FixedUpdate() 
    {
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f
        );
    }
}   
