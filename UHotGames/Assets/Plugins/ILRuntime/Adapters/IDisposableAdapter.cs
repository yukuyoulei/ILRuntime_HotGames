using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class IDisposableAdapter : CrossBindingAdaptor
{
	public override Type BaseCLRType
	{
		get
		{
			return typeof(IDisposable);
		}
	}

	public override Type AdaptorType
	{
		get
		{
			return typeof(Adaptor);
		}
	}

	public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
	{
		return new Adaptor(appdomain, instance);
	}

	internal class Adaptor : IDisposable
	{
		ILTypeInstance instance;
		ILRuntime.Runtime.Enviorment.AppDomain appdomain;
		IMethod mDisposeMethod;
		bool mDisposeMethodGot;
		public void Dispose()
		{
			if (!mDisposeMethodGot)
			{
				mDisposeMethod = instance.Type.GetMethod("Dispose", 0);
				mDisposeMethodGot = true;
			}

			if (mDisposeMethod != null)
			{
				appdomain.Invoke(mDisposeMethod, instance, null);
			}
		}
		public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
		{
			this.appdomain = appdomain;
			this.instance = instance;
		}

		public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

		public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }


	}
}
