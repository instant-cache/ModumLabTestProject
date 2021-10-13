using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool IsHidden = false;
    public bool IsPortrait = true;
    public float HideTranslation = 1;

    public TabletDragController tabletDragController;
    public TabletUIController tabletUIController;
    private PlayerController Player;
    void Start()
    {
        Player = Camera.main.GetComponent<GameManager>().playerController;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void Show()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ToggleHide()
    {
        if (Player.ControlsEnabled)
        {
            IsHidden = !IsHidden;
            if (IsHidden)
                Hide();
            else Show();
        }
    }

    public bool Rotate()
    {
        if (IsHidden)
        {
            return false;
        }
        IsPortrait = !IsPortrait;
        Camera TopCamera = GameObject.FindGameObjectWithTag("SecondaryCamera").GetComponent<Camera>();
        float rotation = -90;
        if (IsPortrait)
            rotation = 90;
        this.transform.Rotate(Vector3.forward, rotation);
        TopCamera.transform.Rotate(Vector3.forward, rotation);
        GetComponentInChildren<TabletUIController>().ChangeOrientation();
        return true;
    }
}
