using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class UnityEngine_UI_Toggle_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.Toggle);

            field = type.GetField("onValueChanged", flag);
            app.RegisterCLRFieldGetter(field, get_onValueChanged_0);
            app.RegisterCLRFieldSetter(field, set_onValueChanged_0);


        }



        static object get_onValueChanged_0(ref object o)
        {
            return ((UnityEngine.UI.Toggle)o).onValueChanged;
        }
        static void set_onValueChanged_0(ref object o, object v)
        {
            ((UnityEngine.UI.Toggle)o).onValueChanged = (UnityEngine.UI.Toggle.ToggleEvent)v;
        }


    }
}
