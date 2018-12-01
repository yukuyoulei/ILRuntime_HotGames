using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
	class CLRBindings
	{
		/// <summary>
		/// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
		/// </summary>
		public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
		{
			System_Int32_Binding.Register(app);
			System_Single_Binding.Register(app);
			System_Int64_Binding.Register(app);
			System_Object_Binding.Register(app);
			System_String_Binding.Register(app);
			System_Console_Binding.Register(app);
			System_Array_Binding.Register(app);
			UnityEngine_Vector3_Binding.Register(app);

		}

		/// <summary>
		/// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
		/// </summary>
		public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
		{
		}
	}
}
