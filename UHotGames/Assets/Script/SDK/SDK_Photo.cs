using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDK_Photo : MonoBehaviour
{
    void SelectPhoto()
    {
#if UNITY_EDITOR
        Debug.Log("SelectPhoto In Editor");
        return;
#endif
        NativeGallery.GetImageFromGallery(OnGetPhotoCb, "Select Photo");
    }
    void SelectVideo()
    {
#if UNITY_EDITOR
        Debug.Log("SelectVideo In Editor");
        return;
#endif
        NativeGallery.GetVideoFromGallery(OnGetVideoCb, "Select Video");
    }


    private void OnGetPhotoCb(string path)
    {
        ILRuntimeHandler.Instance.EmitMessage("photo:" + path);
    }
    private void OnGetVideoCb(string path)
    {
        ILRuntimeHandler.Instance.EmitMessage("video:" + path);
    }
}
