using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Texture2D originalTexture; // The original texture
    public Texture2D changedTexture; // The texture to switch to

    private Image uiImage; // Reference to the UI Image component
    private Sprite originalSprite; // Sprite created from the original texture
    private Sprite changedSprite; // Sprite created from the changed texture

    void Start()
    {
        // Find the UI Image component on the same GameObject
        uiImage = GetComponent<Image>();

        // Ensure the UI Image component is found
        if (uiImage == null)
        {
            Debug.LogError("UI Image component is not found on the GameObject.");
            return;
        }

        // Create sprites from the textures
        originalSprite = Sprite.Create(originalTexture, new Rect(0, 0, originalTexture.width, originalTexture.height), new Vector2(0.5f, 0.5f));
        changedSprite = Sprite.Create(changedTexture, new Rect(0, 0, changedTexture.width, changedTexture.height), new Vector2(0.5f, 0.5f));

        // Set the initial sprite to the original texture
        uiImage.sprite = originalSprite;
    }

    void Update()
    {
        // Check if the left mouse button is held down
        if (Input.GetMouseButton(0))
        {
            uiImage.sprite = changedSprite;
        }
        else
        {
            uiImage.sprite = originalSprite;
        }
    }
}