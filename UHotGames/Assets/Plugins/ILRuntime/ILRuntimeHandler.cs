using ILRuntime.Runtime.Intepreter;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ILRuntime.Runtime.Stack;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;

public class ILRuntimeHandler
{
	private static ILRuntimeHandler sinstance;
	public static ILRuntimeHandler Instance
	{
		get
		{
			if (sinstance == null)
			{
				sinstance = new ILRuntimeHandler();
				sinstance.Init();
			}
			return sinstance;
		}
	}
	ILRuntime.Runtime.Enviorment.AppDomain appdomain;
	void Init()
	{
		appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
#if UNITY_EDITOR
		appdomain.DebugService.StartDebugService(56000);
#endif
		InitializeILRuntime();
	}

	List<string> loadedDlls = new List<string>();
	public void DoLoadDll(string dll, byte[] dllBytes, byte[] pdbBytes = null)
	{
		if (loadedDlls.Contains(dll))
		{
			return;
		}
		loadedDlls.Add(dll);

		MemoryStream pdb = null;
#if UNITY_EDITOR
		if (pdbBytes != null)
		{
			pdb = new MemoryStream(pdbBytes);
		}
#endif
		System.IO.MemoryStream fs = new MemoryStream(dllBytes);
		{
			appdomain.LoadAssembly(fs, pdb, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
		}
	}
	unsafe void InitializeILRuntime()
	{
		InitDelegates();

		SetupCLRRedirection();

		//这里做一些ILRuntime的注册
		appdomain.RegisterCrossBindingAdaptor(new IDisposableAdapter());
		appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
		appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());

		ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
	}

	private void InitDelegates()
	{
		ILRegType.RegisterFunctionDelegate(appdomain);

		#region WebSocket相关
		appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.EventArgs>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler>((act) =>
		{
			return new System.EventHandler((sender, e) =>
			{
				((Action<System.Object, System.EventArgs>)act)(sender, e);
			});
		});
		appdomain.DelegateManager.RegisterMethodDelegate<System.Object, WebSocketSharp.MessageEventArgs>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<WebSocketSharp.MessageEventArgs>>((act) =>
		{
			return new System.EventHandler<WebSocketSharp.MessageEventArgs>((sender, e) =>
			{
				((Action<System.Object, WebSocketSharp.MessageEventArgs>)act)(sender, e);
			});
		});
		appdomain.DelegateManager.RegisterMethodDelegate<System.Object, WebSocketSharp.ErrorEventArgs>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<WebSocketSharp.ErrorEventArgs>>((act) =>
		{
			return new System.EventHandler<WebSocketSharp.ErrorEventArgs>((sender, e) =>
			{
				((Action<System.Object, WebSocketSharp.ErrorEventArgs>)act)(sender, e);
			});
		});
		appdomain.DelegateManager.RegisterMethodDelegate<System.Object, WebSocketSharp.CloseEventArgs>();
		appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<WebSocketSharp.CloseEventArgs>>((act) =>
		{
			return new System.EventHandler<WebSocketSharp.CloseEventArgs>((sender, e) =>
			{
				((Action<System.Object, WebSocketSharp.CloseEventArgs>)act)(sender, e);
			});
		});









		#endregion
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
		appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();		appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.WaitCallback>((act) =>
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
	unsafe void SetupCLRRedirection()
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

	public void EmitMessage(string message, string gameObjectName = "")
	{
		appdomain.Invoke("AHotBase", "EmitMessage", null, gameObjectName, message);
	}
	public void EmitGameObject(string prefabName, GameObject obj, string gameObjectName = "")
	{
		appdomain.Invoke("AHotBase", "EmitGameObject", null, gameObjectName, prefabName, obj);
	}
	private Dictionary<string, GameObject> dObjs = new Dictionary<string, GameObject>();
	public void OnLoadClass(string entranceClass, GameObject rootObj, bool CanBeUnloaded = true, string arg = "")
	{
		if (CanBeUnloaded)
		{
			OnUnloadClass(entranceClass);
			dObjs.Add(entranceClass, rootObj);
		}
		appdomain.Invoke(entranceClass, "SetGameObj", appdomain.Instantiate(entranceClass), rootObj, arg);
	}
	public void OnUnloadClass(string entranceClass)
	{
		if (dObjs.ContainsKey(entranceClass))
		{
			GameObject.Destroy(dObjs[entranceClass]);
			dObjs.Remove(entranceClass);
		}
	}
	public void OnUnloadAllClasses()
	{
		foreach (var d in dObjs)
		{
			GameObject.Destroy(d.Value);
		}
		dObjs.Clear();
	}

	public void SetUnityMessageReceiver(GameObject receiver)
	{
		appdomain.Invoke("AHotBase", "SetUnityMessageReceiver", null, receiver);
	}
}
