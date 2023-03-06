using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollBallNameHover : MonoBehaviour
{
    public TMPro.TextMeshPro nameText;

    bool _visible = false;
    public bool Visible
    {
        get { return _visible; }
        set
        { 
            _visible = value;
            nameText.enabled = value;
        }
    }

    string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            nameText.text = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Visible = false;

        //detach the text box so it doesn't move with us
        nameText.transform.SetParent(null);
    }

    //hide and show the text box on mouse hover using the built-in mouse callbacks for unity
    private void OnMouseEnter()
    {
        Visible = true;
    }
    private void OnMouseExit() 
    {
        Visible = false;
    }

    //ensure the text always follows the ball position and doesn't rotate
    private void LateUpdate()
    {
        if (_visible)
        {
            nameText.transform.position = transform.position + new Vector3(0, 0.5f, -2);
            nameText.transform.rotation = Quaternion.identity;
        }
    }
}
