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
            //use regex to online include alphanumeric characters and spaces
            _name = System.Text.RegularExpressions.Regex.Replace(_name, @"[^A-Za-z0-9 ]", "");
        
            //only include text up to but not including the first bracket
            var bracketIndex = _name.IndexOf('(');
            if (bracketIndex > 0)
                _name = _name.Substring(0, bracketIndex);


            nameText.text = value;
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        //detach the text box so it doesn't move with us
        nameText.transform.SetParent(null);

        //show the text for the first 2 seconds
        Visible = true;
        yield return new WaitForSeconds(2);
        Visible = false;
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
