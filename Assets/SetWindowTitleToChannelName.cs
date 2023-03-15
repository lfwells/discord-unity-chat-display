using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SetWindowTitleToChannelName : MonoBehaviour
{
    //Import the following.
    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern System.IntPtr FindWindow(System.String className, System.String windowName);

    public void OnChannelUpdate(DiscordUnityChatDisplay.ChannelUpdateEvent evt)
    {
        //Get the window handle.
        var windowPtr = FindWindow(null, "Old Window Title");
        //Set the title text using the window handle.
        SetWindowText(windowPtr, $"Discord Poll - #{name}");
    }
}
