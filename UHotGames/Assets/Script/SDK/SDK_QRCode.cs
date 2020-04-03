using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SDK_QRCode : UIBase
{
    ZXing.BarcodeReader _reader;
    ZXing.BarcodeReader reader
    {
        get
        {
            if (_reader == null)
            {
                _reader = new ZXing.BarcodeReader();
            }
            return _reader;
        }
    }

    RawImage previewImg;
    protected override void InitComponents()
    {
        previewImg = FindWidget<RawImage>("previewImg");
    }

    private void OnDisable()
    {
        if (requestedWebCam.isPlaying)
        {
            requestedWebCam.Stop();
        }
    }
    private void OnStartDecodeQRCode()
    {
        if (!requestedWebCam.isPlaying)
        {
            requestedWebCam.Play();
            previewImg.texture = requestedWebCam;
        }
        var res = reader.Decode(requestedWebCam.GetPixels32(), requestedWebCam.width, requestedWebCam.height);
        if (res != null && !string.IsNullOrEmpty(res.Text))
        {
#if ILRUNTIME
            ILRuntimeHandler.Instance.EmitMessage($"qrcode:{res.Text}");
#endif
        }
    }

    WebCamTexture _requestedWebCam;
    WebCamTexture requestedWebCam
    {
        get
        {
            if (_requestedWebCam == null)
            {
                _requestedWebCam = new WebCamTexture((int)(480f / Screen.width * Screen.height), 480);
            }
            return _requestedWebCam;
        }
    }
}
