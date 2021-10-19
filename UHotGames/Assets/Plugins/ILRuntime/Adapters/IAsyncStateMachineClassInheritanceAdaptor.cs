using System;
using System.Runtime.CompilerServices;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IAsyncStateMachineClassInheritanceAdaptor : CrossBindingAdaptor
{
	public override Type BaseCLRType => typeof(IAsyncStateMachine);
	public override Type AdaptorType => typeof(IAsyncStateMachineAdaptor);
	public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
	{
		return new IAsyncStateMachineAdaptor(appdomain, instance);
	}
	public class IAsyncStateMachineAdaptor : IAsyncStateMachine, CrossBindingAdaptorType
	{
		private readonly ILRuntime.Runtime.Enviorment.AppDomain _appDomain;
		public ILTypeInstance ILInstance { get; }
		private readonly IMethod _mMoveNext;
		private readonly IMethod _mSetStateMachine;
		public IAsyncStateMachineAdaptor() { }
		public IAsyncStateMachineAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appDomain, ILTypeInstance instance)
		{
			_appDomain = appDomain;
			ILInstance = instance;
			_mMoveNext = ILInstance.Type.GetMethod("MoveNext", 0);
			_mSetStateMachine = ILInstance.Type.GetMethod("SetStateMachine", 1);
		}
		public void MoveNext()
		{
			using (var ctx = _appDomain.BeginInvoke(_mMoveNext))
			{
				ctx.PushObject(ILInstance);
				ctx.Invoke();
			}
		}
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			using (var ctx = _appDomain.BeginInvoke(_mSetStateMachine))
			{
				ctx.PushObject(ILInstance);
				ctx.PushObject(stateMachine);
				ctx.Invoke();
			}
		}
		public override string ToString()
		{
			var m = _appDomain.ObjectType.GetMethod("ToString", 0);
			m = ILInstance.Type.GetVirtualMethod(m);
			if (m == null || m is ILMethod)
			{
				return ILInstance.ToString();
			}
			return ILInstance.Type.FullName;
		}
	}
}

