using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AHotDrag : MonoBehaviour
{
	public bool DontSyncDrag { get; set; }
	public Action onDragEnd;
	public Action onDragBegin;
	public Action onDragDrop;
	public Action onDraging;
	private Camera _dragcamera;
	public Camera dragcamera
	{
		get
		{
			if (_dragcamera == null)
			{
				_dragcamera = transform.root.GetComponentInChildren<Camera>();
			}
			return _dragcamera;
		}
	}
	private bool disableDraging
	{
		get
		{
			return onDragDrop != null;
		}
	}
	private Canvas _canvas;
	public Canvas canvas
	{
		get
		{
			if (_canvas == null)
			{
				_canvas = GetComponentInParent<Canvas>();
			}
			return _canvas;
		}
	}
	private RectTransform rectTransform;
	private Graphic graphic;
	public void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		graphic = GetComponent<Graphic>();

		RegistDragMessage();
	}

	private void RegistDragMessage()
	{
		if (AHotBase.strArg=="teacher")
		{
			AHotBase.curGame.OnRegistAction(new Action<string>((msg) =>
			{
				var amsg = msg.Split(':');
				if (amsg.Length < 2)
				{
					return;
				}
				if (amsg[1] != gameObject.name)
				{
					return;
				}
				if (amsg[0] == MessageHeadDragBegin)
				{
					onDragBegin?.Invoke();
				}
				else if (amsg[0] == MessageHeadDragEnd)
				{
					onDragEnd?.Invoke();
				}
				else if (amsg[0] == MessageHeadDragDrop)
				{
					onDragDrop?.Invoke();
				}
				else if (amsg[0] == MessageHeadDraging)
				{
					onDraging?.Invoke();
					var adetail = amsg[2].Split(',');
					if (adetail.Length != 2)
					{
						return;
					}
					var v = new Vector3();
					v.x = int.Parse(adetail[0]);
					v.y = int.Parse(adetail[1]);
					transform.localPosition = v;
				}
			}));
		}
	}

	private bool bDraging;
	private Vector3 lastPosition;
	private Vector3 mouseWorldPoint
	{
		get
		{
			if (dragcamera == null)
			{
				return new Vector3(Input.mousePosition.x, Input.mousePosition.y);
			}
			return dragcamera.ScreenToWorldPoint(Input.mousePosition);
		}
	}
	const string MessageHeadDragBegin = "dragbegin";
	const string MessageHeadDragEnd = "dragend";
	const string MessageHeadDraging = "draging";
	const string MessageHeadDragDrop = "dragdrop";
	public void Update()
	{
		if (graphic == null)
		{
			return;
		}
		if (!graphic.raycastTarget)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (disableDraging)
			{
				return;
			}
			if (EventSystem.current.IsPointerOverGameObject(0))
			{
				if (EventSystem.current.currentSelectedGameObject != gameObject)
				{
					return;
				}
			}

			if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mouseWorldPoint))
			{
				lastPosition = Vector3.zero;
				return;
			}
			lastPosition = Input.mousePosition;
			if (lastPosition.Equals(Vector3.zero))
			{
				lastPosition = new Vector3(0.001f, 0);
			}
			if (!bDraging)
			{
				bDraging = true;
				if (onDragBegin != null)
				{
					onDragBegin();
				}

                if (AHotBase.strArg == "teacher")
                {
                    if (!DontSyncDrag)
					{
						AHotBase.SendMessageToUnityReceiver(MessageHeadDragBegin + ":" + gameObject.name);
					}
				}
			}
			return;
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (EventSystem.current.IsPointerOverGameObject(0))
			{
				if (EventSystem.current.currentSelectedGameObject != gameObject)
				{
					return;
				}
			}
			if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mouseWorldPoint))
			{
				return;
			}
			if (lastPosition.Equals(Vector3.zero))
			{
				if (onDragDrop != null)
				{
					onDragDrop();
				}
                if (AHotBase.strArg == "teacher")
                    if (!DontSyncDrag)
						AHotBase.SendMessageToUnityReceiver(MessageHeadDragDrop + ":" + gameObject.name);
			}
			else
			{
				if (onDragEnd != null)
				{
					onDragEnd();
				}
                if (AHotBase.strArg == "teacher")
                {
                    if (!DontSyncDrag)
						AHotBase.SendMessageToUnityReceiver(MessageHeadDragEnd + ":" + gameObject.name);
				}
			}
			StartCoroutine(DelayCancel());
			lastPosition = Vector3.zero;
			return;
		}
		if (Input.GetMouseButton(0))
		{
			if (disableDraging)
			{
				return;
			}
			if (lastPosition.Equals(Vector3.zero))
			{
				return;
			}
			if (onDraging != null)
			{
				onDraging();
			}
			else
			{
				var p = transform.position;
				p.x += (Input.mousePosition.x - lastPosition.x) / (dragcamera == null ? 1 : canvas.scaleFactor);
				p.y += (Input.mousePosition.y - lastPosition.y) / (dragcamera == null ? 1 : canvas.scaleFactor);
				transform.position = p;

				lastPosition = Input.mousePosition;
			}

            if (AHotBase.strArg == "teacher")
            {
                if (!DontSyncDrag)
					AHotBase.SendMessageToUnityReceiver(MessageHeadDraging + ":" + gameObject.name + ":" + (int)transform.localPosition.x + "," + (int)transform.localPosition.y);
			}
		}
		else
		{
			if (disableDraging)
			{
				return;
			}
			if (!bDraging)
			{
				return;
			}
			if (onDragDrop != null)
			{
				onDragDrop();
			}
			if (onDragEnd != null)
			{
				onDragEnd();
			}
			lastPosition = Vector3.zero;
			bDraging = false;
			StartCoroutine(DelayCancel());
		}
	}

	private IEnumerator DelayCancel()
	{
		yield return new WaitForSeconds(0.1f);
		bDraging = false;
		lastPosition = Vector3.zero;
	}
}
