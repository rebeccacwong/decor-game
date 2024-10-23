using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildModeButton : MonoBehaviour
{
    private Button button;
    private Image btnImage;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        Debug.Assert(button != null);

        btnImage = gameObject.GetComponent<Image>();
        Debug.Assert(btnImage != null);
    }

    public void MarkButtonAsNotSelected()
    {
        btnImage.color = button.colors.normalColor;
    }

    public void MarkButtonAsSelected()
    {
        btnImage.color = button.colors.selectedColor;
    }
}