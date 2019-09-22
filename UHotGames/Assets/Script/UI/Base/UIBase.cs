using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public abstract class UIBase : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	/*
		public virtual UIManager.EUI eUI
		{
			get
			{
				return UIManager.EUI.Z_Num;
			}

		}*/
	protected virtual bool UsingWorldCamera
	{
		get
		{
			return false;
		}
	}
	protected virtual bool bShowInTime
	{
		get
		{
			return false;
		}
	}
	protected virtual string CommonSounds
	{
		get
		{
			return "SOUND/Button_Normal";
		}
	}
	protected virtual Dictionary<string, string> SpecialSounds
	{
		get
		{
			return new Dictionary<string, string>();
		}
	}

	protected virtual bool bFollowCamera
	{
		get
		{
			return false;
		}
	}

	protected virtual bool KeepUsingOverlay
	{
		get
		{
			return false;
		}
	}
	protected virtual bool HijackMouseDown
	{
		get
		{
			return false;
		}
	}

	private Canvas _canvas;
	protected Canvas canvas
	{
		get
		{
			if (_canvas == null)
			{
				_canvas = GetComponent<Canvas>();
			}
			return _canvas;
		}
	}
	protected bool bInited;
	private static GameObject refreshCameraObj;
	protected virtual void Awake()
	{
		InitComponents();
		bInited = true;
	}
	public T FindWidget<T>(string widgetName) where T : Component
	{
		return UStaticFuncs.FindChildComponent<T>(transform, widgetName);
	}
	public T FindWidget<T>(Transform trans, string widgetName) where T : Component
	{
		return UStaticFuncs.FindChildComponent<T>(trans, widgetName);
	}
	public T FindWidgetWithPath<T>(string widgetNameWithPath) where T : Component
	{
		var parent = transform;
		var widgetName = widgetNameWithPath;
		widgetNameWithPath = widgetNameWithPath.Replace("\\", "/");
		if (widgetNameWithPath.Contains("/"))
		{
			var alayers = widgetNameWithPath.Split('/');
			widgetName = alayers[alayers.Length - 1];
			for (var i = 0; i < alayers.Length - 1; i++)
			{
				var tr = FindWidgetWithPath<Transform>(parent, alayers[i]);
				if (tr == null)
				{
					break;
				}
				parent = tr;
			}
		}
		return UStaticFuncs.FindChildComponent<T>(parent, widgetName);
	}
	public T FindWidgetWithPath<T>(Transform trans, string widgetName) where T : Component
	{
		var t = UStaticFuncs.FindChildComponent<T>(trans, widgetName);
		if (t == null)
		{
			AOutput.Log("FindWidget failed " + widgetName);
		}
		return t;
	}

	protected void ShowWidget(Transform tr, string widgetName, bool bShow)
	{
		if (tr == null)
		{
			return;
		}
		var w = UStaticFuncs.FindChildComponent<Component>(tr, widgetName);
		if (w != null)
		{
			w.gameObject.SetActive(bShow);
		}
	}
	protected void ShowWidget(string widgetName, bool bShow)
	{
		if (transform == null)
		{
			return;
		}
		var w = UStaticFuncs.FindChildComponent<Component>(transform, widgetName);
		if (w != null)
		{
			w.gameObject.SetActive(bShow);
		}
	}
	protected virtual void Start()
	{
		if (bShowInTime)
		{
			ShowInTime();
		}
	}
	void ShowInTime()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		if (cg == null)
		{
			cg = gameObject.AddComponent<CanvasGroup>();
		}
		cg.alpha = 0;
		rawScale = cg.transform.localScale;
		StartCoroutine(Show(cg));
	}
	protected virtual void DoShowEnd()
	{

	}

	private Vector3 rawScale;
	private IEnumerator Show(CanvasGroup cg)
	{
		yield return new WaitForEndOfFrame();
		cg.alpha += 1f * Time.deltaTime;
		cg.transform.localScale = rawScale * cg.alpha;
		if (cg.alpha < 1)
		{
			StartCoroutine(Show(cg));
		}
		else
		{
			cg.transform.localScale = rawScale;
			//Image[] images = transform.GetComponentsInChildren<Image>();

			DoShowEnd();
		}
	}

	protected abstract void InitComponents();

	public bool bClosing = false;
	public virtual void OnCloseUI()
	{
		bClosing = true;
		if (gameObject != null)
		{
			Destroy(this.gameObject);
		}
	}

	public virtual void SetVisible(bool bVisible)
	{
		gameObject.SetActive(bVisible);
	}

	protected virtual void OnDestroy()
	{
		CancelInvoke();
		StopAllCoroutines();
	}

	protected void RegistButtonAction(string buttonName, Action clickAction)
	{
		var btn = UStaticFuncs.FindChildComponent<Button>(transform, buttonName);
		if (btn != null)
		{
			btn.onClick.AddListener(() => { clickAction(); });
		}
	}
	protected void ChangeButtonText(string buttonName, string buttonText)
	{
		var btn = UStaticFuncs.FindChildComponent<Button>(transform, buttonName);
		if (btn == null)
		{
			return;
		}
		var t = btn.GetComponentInChildren<Text>();
		t.text = buttonText;
	}
	protected virtual void Update()
	{

	}

	private List<Graphic> dragObjects = new List<Graphic>();
	private List<Graphic> dragObjects2 = new List<Graphic>();
	protected void UnregistDragObject(Graphic obj)
	{
		if (dragObjects.Contains(obj))
		{
			dragObjects.Remove(obj);
		}
	}
	protected void RegistDragObject(params Graphic[] objs)
	{
		dragObjects.AddRange(objs);
	}

	protected void RegistDragObjectToLast(params Graphic[] objs)
	{
		dragObjects2.AddRange(objs);
	}

	protected Graphic draging;
	public bool bEnableDraging = true;
	protected bool isDraging;
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!bEnableDraging)
		{
			return;
		}
		if (draging != null)
		{
			return;
		}
		List<Graphic> remove = new List<Graphic>();
		foreach (var obj in dragObjects)
		{
			if (obj == null)
			{
				remove.Add(obj);
			}
			else if (eventData.hovered.Contains(obj.gameObject))
			{
				draging = obj;
				draging.raycastTarget = false;
				BeginDrag(obj);
				break;
			}
		}
		if (draging == null)
		{
			foreach (var obj in dragObjects2)
			{
				if (obj == null)
				{
					remove.Add(obj);
				}
				else if (eventData.hovered.Contains(obj.gameObject))
				{
					draging = obj;
					draging.raycastTarget = false;
					BeginDrag(obj);
					break;
				}
			}
		}
		foreach (var rm in remove)
		{
			dragObjects.Remove(rm);
		}

		isDraging = true;
	}
	protected virtual void BeginDrag(Graphic dragObj)
	{

	}
	public void OnEndDrag(PointerEventData eventData)
	{
		if (draging != null)
		{
			draging.raycastTarget = true;
			EndDrag(draging, eventData.hovered, eventData);
		}
		draging = null;
		isDraging = false;
	}
	protected virtual void EndDrag(Graphic dragObj, List<GameObject> hoveredObjs, PointerEventData eventData)
	{

	}
	public void OnDrag(PointerEventData eventData)
	{
		if (draging != null)
		{
			Draging(draging, eventData.delta);
		}
	}
	protected virtual void Draging(Graphic dragObj, Vector2 dragDelta)
	{
		if (dragObj != null && (dragObjects.Contains(dragObj) || dragObjects2.Contains(dragObj)))
		{
			dragObj.GetComponent<RectTransform>().anchoredPosition += dragDelta / GetComponent<RectTransform>().lossyScale.x;
		}
	}
	protected IEnumerator DelayDoSth(float delay, Action action)
	{
		yield return new WaitForSeconds(delay);

		action?.Invoke();
	}
	protected IEnumerator DelayDoSth(Action action, float delay)
	{
		yield return new WaitForSeconds(delay);

		action?.Invoke();
	}
}
