using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Runtime.CompilerServices;

public class IDisposableAdapter : CrossBindingAdaptor
{
	//定义访问方法的方法信息
	static CrossBindingMethodInfo mDispose = new CrossBindingMethodInfo("Dispose");
	public override Type BaseCLRType
	{
		get
		{
			return typeof(IDisposable);//这里是你想继承的类型
		}
	}

	public override Type AdaptorType
	{
		get
		{
			return typeof(Adapter);
		}
	}

	public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
	{
		return new Adapter(appdomain, instance);
	}

	public class Adapter : IDisposable, CrossBindingAdaptorType
	{
		ILTypeInstance instance;
		ILRuntime.Runtime.Enviorment.AppDomain appdomain;

		//必须要提供一个无参数的构造函数
		public Adapter()
		{

		}

		public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
		{
			this.appdomain = appdomain;
			this.instance = instance;
		}

		public ILTypeInstance ILInstance { get { return instance; } }

		public void Dispose()
		{
			if (!mDispose.CheckShouldInvokeBase(instance))
				mDispose.Invoke(this.instance);
		}
	}
}