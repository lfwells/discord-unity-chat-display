using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class StudentCountField : MonoBehaviour
{
    public PollVisuals pollVisuals;
    public TMP_InputField inputField;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Space))
        {
            inputField.DeactivateInputField();
        }
        else
        {
            //ensure input field gets focus
            inputField.ActivateInputField();
        }
    }

    public void OnValueSet(string value)
    {
        if (int.TryParse(value, out int count))
        {
            pollVisuals.hardCodedTotalCount = count;
            pollVisuals.countType = PollVisuals.CountType.CountOutOfHardCodedNumber;
            pollVisuals.UpdateAllAnswerTexts();
        }

        inputField.text = string.Empty;
    }
}
