using UnityEngine;

public class Boundary : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector2 _screenBounds;
    private float _objectWidth;
    private float _objectHeight;

    public SpriteRenderer sprite;
    private void Start()
    {
        _mainCamera = Camera.main;
        _screenBounds = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));

        if(!sprite)
        {
            //log error
            Debug.LogError("SpriteRenderer not assigned in Boundary script.");
        }
            

        _objectWidth = sprite.bounds.extents.x;
        _objectHeight = sprite.bounds.extents.y;
    }

    private void LateUpdate()
    {
        Vector3 viewPos = transform.position;

        // Wrap horizontal
        if (viewPos.x < -_screenBounds.x - _objectWidth)
        {
            viewPos.x = _screenBounds.x + _objectWidth;
        }
        else if (viewPos.x > _screenBounds.x + _objectWidth)
        {
            viewPos.x = -_screenBounds.x - _objectWidth;
        }

        // Wrap vertical
        if (viewPos.y < -_screenBounds.y - _objectHeight)
        {
            viewPos.y = _screenBounds.y + _objectHeight;
        }
        else if (viewPos.y > _screenBounds.y + _objectHeight)
        {
            viewPos.y = -_screenBounds.y - _objectHeight;
        }

        transform.position = viewPos;
    }
}