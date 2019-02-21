using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using UnityEngine.UI;

public class MaskableGraphicAdapter : CrossBindingAdaptor
{
	public override Type BaseCLRType
	{
		get
		{
			return typeof(MaskableGraphic);
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
	//为了完整实现MaskableGraphic的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
	public class Adaptor : MaskableGraphic, CrossBindingAdaptorType
	{
		ILTypeInstance instance;
		ILRuntime.Runtime.Enviorment.AppDomain appdomain;

		public Adaptor()
		{

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