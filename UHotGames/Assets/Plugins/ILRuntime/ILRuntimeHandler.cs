using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class ILRuntimeHandler
{
	private static ILRuntime.Runtime.Enviorment.AppDomain appdomain;
	public static void OnStartILRuntime(byte[] dllbytes, byte[] pdbbytes = null)
	{
		appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
		appdomain.LoadAssembly(new MemoryStream(dllbytes)
			, pdbbytes == null ? null : new MemoryStream(pdbbytes)
			, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#if UNITY_EDITOR
		appdomain.DebugService.StartDebugService(56000);
#endif
		InitAdaptors();
		InitDelegates();
		SetupCLRRedirection();
		ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
		OnILRuntimeInitialized();
	}
	private static void InitDelegates()
	{
		appdomain.DelegateManager.RegisterMethodDelegate<global::IDisposableAdapter.Adapter>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Component, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Component, UnityEngine.Component, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Object>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.DateTime, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Object, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Single, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Single, System.Single, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector4, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector4, UnityEngine.Vector4, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Matrix4x4, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Matrix4x4, UnityEngine.Matrix4x4, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject, UnityEngine.GameObject, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.EventSystems.RaycastResult, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.EventSystems.RaycastResult, UnityEngine.EventSystems.RaycastResult, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.EventSystems.EventTrigger.Entry, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.EventSystems.EventTrigger.Entry, UnityEngine.EventSystems.EventTrigger.Entry, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector3, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector3, UnityEngine.Vector3, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Color, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Color, UnityEngine.Color, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Color32, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Color32, UnityEngine.Color32, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector2, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector2, UnityEngine.Vector2, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.BoneWeight, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.BoneWeight, UnityEngine.BoneWeight, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UIVertex, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UIVertex, UnityEngine.UIVertex, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Rect, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Rect, UnityEngine.Rect, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.AnimatorClipInfo, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.AnimatorClipInfo, UnityEngine.AnimatorClipInfo, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.Selectable, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.Selectable, UnityEngine.UI.Selectable, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UICharInfo, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UICharInfo, UnityEngine.UICharInfo, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UILineInfo, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UILineInfo, UnityEngine.UILineInfo, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.Dropdown.OptionData, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.Dropdown.OptionData, UnityEngine.UI.Dropdown.OptionData, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.String, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Sprite, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Sprite, UnityEngine.Sprite, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Int32, System.Char, System.Char>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.RectMask2D, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.RectMask2D, UnityEngine.UI.RectMask2D, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.ILayoutElement, System.Single>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Type, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Type, System.Type, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Object, System.Boolean>();
		appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Object, UnityEngine.Object, System.Int32>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.Threading.Tasks.Task>();
		appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
		appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.AsyncOperation>();
		appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.Threading.Tasks.UnobservedTaskExceptionEventArgs>();
		appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();
		appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.String, System.Int32>();
		appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector2>();


		appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Vector2>>((act) =>
		{
			return new UnityEngine.Events.UnityAction<UnityEngine.Vector2>((arg0) =>
			{
				((Action<UnityEngine.Vector2>)act)(arg0);
			});
		});

		appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.String>>((act) =>
		{
			return new System.Comparison<System.String>((x, y) =>
			{
				return ((Func<System.String, System.String, System.Int32>)act)(x, y);
			});
		});

		appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
		{
			return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
			{
				((Action<System.Single>)act)(arg0);
			});
		});

		appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<System.Threading.Tasks.UnobservedTaskExceptionEventArgs>>((act) =>
		{
			return new System.EventHandler<System.Threading.Tasks.UnobservedTaskExceptionEventArgs>((sender, e) =>
			{
				((Action<System.Object, System.Threading.Tasks.UnobservedTaskExceptionEventArgs>)act)(sender, e);
			});
		});

		appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ParameterizedThreadStart>((act) =>
		{
			return new System.Threading.ParameterizedThreadStart((obj) =>
			{
				((Action<System.Object>)act)(obj);
			});
		});
		appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.SendOrPostCallback>((act) =>
		{
			return new System.Threading.SendOrPostCallback((state) =>
			{
				((Action<System.Object>)act)(state);
			});
		});

		appdomain.DelegateManager.RegisterFunctionDelegate<System.Reflection.MethodInfo, System.Boolean>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Reflection.MethodInfo>>((act) =>
		{
			return new System.Predicate<System.Reflection.MethodInfo>((obj) =>
			{
				return ((Func<System.Reflection.MethodInfo, System.Boolean>)act)(obj);
			});
		});

		appdomain.DelegateManager.RegisterMethodDelegate<System.IAsyncResult>();

		appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.UnhandledExceptionEventArgs>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.UnhandledExceptionEventHandler>((act) =>
		{
			return new System.UnhandledExceptionEventHandler((sender, e) =>
			{
				((Action<System.Object, System.UnhandledExceptionEventArgs>)act)(sender, e);
			});
		});

		appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
		appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.WaitCallback>((act) =>
		{
			return new System.Threading.WaitCallback((state) =>
			{
				((Action<System.Object>)act)(state);
			});
		});

		appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
		appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
		{
			return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) =>
			{
				((Action<System.Boolean>)act)(arg0);
			});
		});

		appdomain.DelegateManager.RegisterMethodDelegate<System.String>();
		appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();

		appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
		{
			return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
			{
				return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
			});
		});


		appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ThreadStart>((act) =>
		{
			return new System.Threading.ThreadStart(() =>
			{
				((Action)act)();
			});
		});
		appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();
		appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>>((act) =>
		{
			return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>((arg0, arg1) =>
			{
				((Action<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>)act)(arg0, arg1);
			});
		});
		appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.String, UnityEngine.LogType>();
		appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Application.LogCallback>((act) =>
		{
			return new UnityEngine.Application.LogCallback((condition, stackTrace, type) =>
			{
				((Action<System.String, System.String, UnityEngine.LogType>)act)(condition, stackTrace, type);
			});
		});
		appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
		{
			return new UnityEngine.Events.UnityAction(() =>
			{
				((Action)act)();
			});
		});
	}
	unsafe private static void SetupCLRRedirection()
	{
		//这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
		var arr = typeof(GameObject).GetMethods();
		foreach (var i in arr)
		{
			if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
			{
				appdomain.RegisterCLRMethodRedirection(i, AddComponent);
			}
			else if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
			{
				appdomain.RegisterCLRMethodRedirection(i, GetComponent);
			}
		}
	}
	unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
	{
		//CLR重定向的说明请看相关文档和教程，这里不多做解释
		ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

		var ptr = __esp - 1;
		//成员方法的第一个参数为this
		GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
		if (instance == null)
			throw new System.NullReferenceException();
		__intp.Free(ptr);

		var genericArgument = __method.GenericArguments;
		//AddComponent应该有且只有1个泛型参数
		if (genericArgument != null && genericArgument.Length == 1)
		{
			var type = genericArgument[0];
			object res;
			if (type is CLRType)
			{
				//Unity主工程的类不需要任何特殊处理，直接调用Unity接口
				res = instance.AddComponent(type.TypeForCLR);
			}
			else
			{
				//热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
				var ilInstance = new ILTypeInstance(type as ILType, false);//手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
																		   //接下来创建Adapter实例
				var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
				//unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
				clrInstance.ILInstance = ilInstance;
				clrInstance.AppDomain = __domain;
				//这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
				ilInstance.CLRInstance = clrInstance;

				res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance

				clrInstance.Awake();//因为Unity调用这个方法时还没准备好所以这里补调一次
			}

			return ILIntepreter.PushObject(ptr, __mStack, res);
		}

		return __esp;
	}
	unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
	{
		//CLR重定向的说明请看相关文档和教程，这里不多做解释
		ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

		var ptr = __esp - 1;
		//成员方法的第一个参数为this
		GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
		if (instance == null)
			throw new System.NullReferenceException();
		__intp.Free(ptr);

		var genericArgument = __method.GenericArguments;
		//AddComponent应该有且只有1个泛型参数
		if (genericArgument != null && genericArgument.Length == 1)
		{
			var type = genericArgument[0];
			object res = null;
			if (type is CLRType)
			{
				//Unity主工程的类不需要任何特殊处理，直接调用Unity接口
				res = instance.GetComponent(type.TypeForCLR);
			}
			else
			{
				//因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
				var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
				for (int i = 0; i < clrInstances.Length; i++)
				{
					var clrInstance = clrInstances[i];
					if (clrInstance.ILInstance != null)//ILInstance为null, 表示是无效的MonoBehaviour，要略过
					{
						if (clrInstance.ILInstance.Type == type)
						{
							res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance
							break;
						}
					}
				}
			}

			return ILIntepreter.PushObject(ptr, __mStack, res);
		}

		return __esp;
	}

	private static void InitAdaptors()
	{
		appdomain.RegisterCrossBindingAdaptor(new IDisposableAdapter());
		appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
		appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
		appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
		appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
		appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
	}
	public static void OnILRUpdate()
	{
		if (appdomain == null)
			return;
		appdomain.Invoke("AEntrance", "Update", null);
	}
	private static void OnILRuntimeInitialized()
	{
		appdomain.Invoke("AEntrance", "Initialize", null,
#if UNITY_ANDROID
			"android"
#elif UNITY_IOS
			"ios"
#else
			"pc"
#endif
);
	}
	public static void OnInvoke(string className, string methodName, params string[] args)
	{
		appdomain.Invoke(className, methodName, null, args);
	}
}
