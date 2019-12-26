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
            Newtonsoft_Json_JsonConvert_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_List_1_Action_Binding.Register(app);
            System_Collections_Generic_List_1_Action_Binding.Register(app);
            System_Collections_Generic_List_1_Action_Binding_Enumerator_Binding.Register(app);
            System_Action_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding.Register(app);
            Newtonsoft_Json_Linq_JObject_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_KeyValuePair_2_String_JToken_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_JToken_Binding.Register(app);
            Newtonsoft_Json_Linq_JToken_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Transform_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_RawImage_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            UnityEngine_UI_Graphic_Binding.Register(app);
            System_Random_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int32_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_RawImage_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_RawImage_Binding.Register(app);
            UnityEngine_Behaviour_Binding.Register(app);
            System_Collections_Generic_List_1_KeyValuePair_2_Int32_RawImage_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);
            System_Collections_Generic_List_1_KeyValuePair_2_Int32_RawImage_Binding_Enumerator_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            System_Math_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Collections_Generic_Queue_1_Transform_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Dictionary_2_Int32_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_String_Binding.Register(app);
            UnityEngine_UI_Toggle_Binding.Register(app);
            UnityEngine_Events_UnityEvent_1_Boolean_Binding.Register(app);
            UnityEngine_PlayerPrefs_Binding.Register(app);
            System_Collections_Generic_List_1_Transform_Binding.Register(app);
            System_Collections_Generic_List_1_Transform_Binding_Enumerator_Binding.Register(app);
            UnityEngine_UI_RawImage_Binding.Register(app);
            System_Char_Binding.Register(app);
            Newtonsoft_Json_Linq_JArray_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_JToken_Binding.Register(app);
            UnityEngine_UI_InputField_Binding.Register(app);
            System_DateTime_Binding.Register(app);
            System_TimeSpan_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_Application_Binding.Register(app);
            UnityEngine_UI_Image_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Action_1_JObject_Binding.Register(app);
            System_Action_1_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Action_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Action_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_Action_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            UnityEngine_Canvas_Binding.Register(app);
            UnityEngine_EventSystems_EventSystem_Binding.Register(app);
            UnityEngine_EventSystems_PointerEventData_Binding.Register(app);
            System_Collections_Generic_List_1_RaycastResult_Binding.Register(app);
            System_Collections_Generic_List_1_RaycastResult_Binding_Enumerator_Binding.Register(app);
            UnityEngine_EventSystems_RaycastResult_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_GameObject_Binding.Register(app);
            System_Collections_Generic_List_1_Func_1_Boolean_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            System_Collections_Generic_List_1_GameObject_Binding.Register(app);
            System_Collections_Generic_List_1_GameObject_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_List_1_Action_1_GameObject_Binding.Register(app);
            System_Collections_Generic_List_1_Action_1_GameObject_Binding.Register(app);
            System_Collections_Generic_List_1_Action_1_GameObject_Binding_Enumerator_Binding.Register(app);
            System_Action_1_GameObject_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_Func_1_Boolean_Binding_Enumerator_Binding.Register(app);
            System_Func_1_Boolean_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Button_Binding.Register(app);
            System_Text_Encoding_Binding.Register(app);
            System_Convert_Binding.Register(app);
            System_Byte_Binding.Register(app);
            System_UInt16_Binding.Register(app);
            System_Double_Binding.Register(app);
            System_Int64_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            UnityEngine_AssetBundle_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_AssetBundle_Binding.Register(app);
            System_IO_File_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding_Enumerator_Binding.Register(app);
            UnityEngine_AssetBundleManifest_Binding.Register(app);
            System_Action_1_Single_Binding.Register(app);
            UnityEngine_WWW_Binding.Register(app);
            System_IO_FileInfo_Binding.Register(app);
            System_IO_FileSystemInfo_Binding.Register(app);
            System_IO_DirectoryInfo_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            UnityEngine_GUISkin_Binding.Register(app);
            UnityEngine_GUIStyle_Binding.Register(app);
            UnityEngine_EventSystems_ExecuteEvents_Binding.Register(app);
            System_Security_Cryptography_MD5CryptoServiceProvider_Binding.Register(app);
            System_Security_Cryptography_HashAlgorithm_Binding.Register(app);
            System_BitConverter_Binding.Register(app);
            System_IO_FileStream_Binding.Register(app);
            System_IO_Stream_Binding.Register(app);
            System_Text_StringBuilder_Binding.Register(app);
            System_Exception_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_AudioSource_Binding.Register(app);
            UnityEngine_Networking_UnityWebRequest_Binding.Register(app);
            UnityEngine_AudioClip_Binding.Register(app);
            System_Int16_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Collections_Generic_List_1_Single_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            System_Net_WebRequest_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_String_Binding.Register(app);
            System_Collections_Specialized_NameValueCollection_Binding.Register(app);
            System_Net_WebResponse_Binding.Register(app);
            System_IO_StreamReader_Binding.Register(app);
            System_IO_TextReader_Binding.Register(app);
            System_IO_Directory_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_String_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_String_Binding.Register(app);
            UnityEditor_AssetDatabase_Binding.Register(app);
            System_Threading_Thread_Binding.Register(app);
            System_Action_1_Byte_Array_Binding.Register(app);
            WebSocketSharp_WebSocket_Binding.Register(app);
            System_Threading_Tasks_Task_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Action_1_String_Binding.Register(app);
            WebSocketSharp_MessageEventArgs_Binding.Register(app);
            WebSocketSharp_ErrorEventArgs_Binding.Register(app);
            WebSocketSharp_CloseEventArgs_Binding.Register(app);
            System_Action_1_EventArgs_Binding.Register(app);
            System_Action_1_MessageEventArgs_Binding.Register(app);
            System_Action_1_ErrorEventArgs_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
