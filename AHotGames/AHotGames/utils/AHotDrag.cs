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
	public bool disableDragImage { get; set; }
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
				_dragcamera = transform.root.GetComponentInChildren<Canvas>().worldCamera;
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
	}

	public void OnDestroy()
	{
		onDragEnd = null;
		onDragBegin = null;
		onDraging = null;
		onDragDrop = null;
	}

	private bool bDraging;
	private Vector3 lastPosition;
	public float scale
	{
		get
		{
			return dragcamera == null ? 1 : canvas.scaleFactor;
		}
	}

	bool RaycastMe()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		foreach (var r in results)
		{
			if (r.gameObject == this.gameObject)
			{
				return true;
			}
		}
		return false;
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
			if (bDraging)
			{
				lastPosition = Vector3.zero;
				onDragEnd?.Invoke();
			}
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (disableDraging)
			{
				return;
			}

			if (!RaycastMe())
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
			}
			return;
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (!RaycastMe())
			{
				lastPosition = Vector3.zero;
				return;
			}
			if (lastPosition.Equals(Vector3.zero))
			{
				if (onDragDrop != null)
				{
					onDragDrop();
				}
			}
			else
			{
				if (onDragEnd != null)
				{
					onDragEnd();
				}
			}
			bDraging = false;
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
			if (Vector2.Distance(lastPosition, Input.mousePosition) < 0.5f)
			{
				return;
			}
			if (!disableDragImage)
			{
				var p = transform.position;
				p.x += (Input.mousePosition.x - lastPosition.x) / (scale);
				p.y += (Input.mousePosition.y - lastPosition.y) / (scale);
				transform.position = p;

				lastPosition = Input.mousePosition;
			}
            if (onDraging != null)
            {
                onDraging();
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
		}
	}
}
