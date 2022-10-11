using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif

public class IEquatable_1_ILTypeInstanceAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(System.IEquatable<ILRuntime.Runtime.Intepreter.ILTypeInstance>);
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

    public class Adapter : System.IEquatable<ILRuntime.Runtime.Intepreter.ILTypeInstance>, CrossBindingAdaptorType
    {
        CrossBindingFunctionInfo<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean> mEquals_0 = new CrossBindingFunctionInfo<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>("Equals");

        bool isInvokingToString;
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adapter()
        {

        }

        public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }

        public System.Boolean Equals(ILRuntime.Runtime.Intepreter.ILTypeInstance other)
        {
            return mEquals_0.Invoke(this.instance, other);
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                if (!isInvokingToString)
                {
                    isInvokingToString = true;
                    string res = instance.ToString();
                    isInvokingToString = false;
                    return res;
                }
                else
                    return instance.Type.FullName;
            }
            else
                return instance.Type.FullName;
        }
    }
}

