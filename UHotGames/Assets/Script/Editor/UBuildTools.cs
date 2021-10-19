using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
/*
using UnityEngine.VR;
*/
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
public class UBuildTools : EditorWindow
{
#if UNITY_ANDROID
	BuildTargetGroup target
	{
		get
		{
			return BuildTargetGroup.Android;
		}
	}
#elif UNITY_IOS
	BuildTargetGroup target
	{
		get
		{
			return BuildTargetGroup.iOS;
		}
	}
#elif UNITY_STANDALONE
    BuildTargetGroup target
    {
        get
        {
            return BuildTargetGroup.Standalone;
        }
    }
#elif UNITY_WEBGL
	BuildTargetGroup target
	{
		get
		{
			return BuildTargetGroup.WebGL;
		}
	}
#endif
	[MenuItem("MyTools/打包工具")]
	public static void ShowWindow()
	{
		EditorWindow thisWindow = EditorWindow.GetWindow(typeof(UBuildTools));
		thisWindow.titleContent = new GUIContent("打包工具");
	}

	void OnInspectorUpdate()
	{
		this.Repaint();
	}

	static string sversion
	{
		get
		{
#if UNITY_STANDALONE && UNITY_EDITOR
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            if (File.Exists(Application.streamingAssetsPath + "/Version"))
            {
                return File.ReadAllText(Application.streamingAssetsPath + "/Version");
            }
            return PlayerSettings.bundleVersion;
#else
			return PlayerSettings.bundleVersion;
#endif
		}
		set
		{
#if UNITY_STANDALONE && UNITY_EDITOR
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            if (File.Exists(Application.streamingAssetsPath + "/Version"))
            {
                File.WriteAllText(Application.streamingAssetsPath + "/Version", value);
            }
#else
			PlayerSettings.bundleVersion = value;
#endif
		}
	}
	string buildCode
	{
		get
		{
#if UNITY_ANDROID
			return PlayerSettings.Android.bundleVersionCode.ToString();
#elif UNITY_IOS
			return PlayerSettings.iOS.buildNumber;
#elif UNITY_STANDALONE && UNITY_EDITOR
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            if (File.Exists(Application.streamingAssetsPath + "/Version"))
            {
                return File.ReadAllText(Application.streamingAssetsPath + "/Version");
            }
            return PlayerSettings.bundleVersion;
#else
			return PlayerSettings.bundleVersion;
#endif
		}
		set
		{
#if UNITY_ANDROID
			PlayerSettings.Android.bundleVersionCode = int.Parse(value);
#elif UNITY_IOS
			PlayerSettings.iOS.buildNumber = value;
#elif UNITY_STANDALONE && UNITY_EDITOR
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            if (File.Exists(Application.streamingAssetsPath + "/Version"))
            {
                File.WriteAllText(Application.streamingAssetsPath + "/Version", value);
            }
#endif
		}
	}

	static string gameName
	{
		get
		{
			return PlayerSettings.productName;
		}
		set
		{
			PlayerSettings.productName = value;
		}
	}
	static bool vrsupport
	{
		get
		{
			return PlayerSettings.virtualRealitySupported;
		}
		set
		{
			if (vrsupport != value)
			{
				PlayerSettings.virtualRealitySupported = value;
			}
		}
	}
	bool bUsingLocalCDN
	{
		get
		{
			return PlayerPrefs.GetInt("USE_LOCAL_CDN") == 1;
		}
		set
		{
			PlayerPrefs.SetInt("USE_LOCAL_CDN", value ? 1 : 0);
		}
	}
	bool bUsingIL2CPP
	{
		get
		{
			return PlayerSettings.GetScriptingBackend(target) == ScriptingImplementation.IL2CPP;
		}
		set
		{
			PlayerSettings.SetScriptingBackend(target, value ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
			if (value && target == BuildTargetGroup.iOS)
			{
				PlayerSettings.SetArchitecture(target, 2);
			}
		}
	}
	bool UseAB
	{
		get
		{
			return Enter.bUsingAb;
		}
		set
		{
			Enter.bUsingAb = value;
		}
	}

	const string sILRUNTIME = "ILRUNTIME";
	bool ILRUNTIME
	{
		get
		{
			return PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(';').Contains(sILRUNTIME);
		}
		set
		{
			var asymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(';').ToList();
			if (value)
			{
				if (asymbols.Contains(sILRUNTIME)) return;
				asymbols.Add(sILRUNTIME);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", asymbols));
			}
			else
			{
				if (!asymbols.Contains(sILRUNTIME)) return;
				asymbols.Remove(sILRUNTIME);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(",", asymbols));
			}
		}
	}

	string sBuildLog = "";
	Vector2 scrollPos;
	void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		if (GUILayout.Button("Remove PlayerPref", GUILayout.Width(160)))
		{
			PlayerPrefs.DeleteAll();
		}

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Version:", GUILayout.Width(70));
		sversion = EditorGUILayout.TextField(sversion);
		if (GUILayout.Button("-", GUILayout.Width(20)))
		{
			var abc = sversion.Split('.');
			if (abc[abc.Length - 1] == "0")
			{
				return;
			}
			abc[abc.Length - 1] = (int.Parse(abc[abc.Length - 1]) - 1).ToString();
			var res = "";
			foreach (var r in abc)
			{
				if (!string.IsNullOrEmpty(res))
				{
					res += ".";
				}
				res += r;
			}
			sversion = res;
		}
		if (GUILayout.Button("+", GUILayout.Width(20)))
		{
			sversion = sversion.Substring(0, sversion.LastIndexOf(".") + 1) + (int.Parse(sversion.Substring(sversion.LastIndexOf(".") + 1)) + 1);
		}
		EditorGUILayout.LabelField(" ", GUILayout.Width(20));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("BuildCode:", GUILayout.Width(70));
		buildCode = EditorGUILayout.TextField(buildCode);
		if (GUILayout.Button("-", GUILayout.Width(20)))
		{
			var abc = buildCode.Split('.');
			if (abc[abc.Length - 1] == "0")
			{
				return;
			}
			abc[abc.Length - 1] = (int.Parse(abc[abc.Length - 1]) - 1).ToString();
			var res = "";
			foreach (var r in abc)
			{
				if (!string.IsNullOrEmpty(res))
				{
					res += ".";
				}
				res += r;
			}
			buildCode = res;
		}
		if (GUILayout.Button("+", GUILayout.Width(20)))
		{
			buildCode = buildCode.Substring(0, buildCode.LastIndexOf(".") + 1) + (int.Parse(buildCode.Substring(buildCode.LastIndexOf(".") + 1)) + 1);
		}
		EditorGUILayout.LabelField(" ", GUILayout.Width(20));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField($"Final version:{Application.version}", GUILayout.Width(170));
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(60));
		gameName = EditorGUILayout.TextField(gameName);
		EditorGUILayout.LabelField(" ", GUILayout.Width(20));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID:", GUILayout.Width(60));
		PlayerSettings.SetApplicationIdentifier(target, EditorGUILayout.TextField(PlayerSettings.applicationIdentifier));
		EditorGUILayout.LabelField(" ", GUILayout.Width(20));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		/*

				vrsupport = EditorGUILayout.Toggle("VR Support", vrsupport);
				if (vrsupport)
				{
#if UNITY_2017
					EditorGUILayout.LabelField("└ Supported Devices:" + string.Join(",", UnityEngine.XR.XRSettings.supportedDevices));
#else
					EditorGUILayout.LabelField("└ Supported Devices:" + string.Join(",", UnityEngine.XR.XRSettings.supportedDevices));
#endif
				}
		*/

		EditorGUILayout.Space();
		UseAB = EditorGUILayout.ToggleLeft("编辑器下是否使用AB", UseAB);
		ILRUNTIME = EditorGUILayout.ToggleLeft("是否使用ILRUNTIME", ILRUNTIME);
		bUsingLocalCDN = EditorGUILayout.ToggleLeft("是否使用本地CDN", bUsingLocalCDN);

#if UNITY_ANDROID
		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("包地址", GUILayout.Width(50));
		targetBuildDir = EditorGUILayout.TextField(targetBuildDir);
		GUILayout.EndHorizontal();

		bUsingIL2CPP = EditorGUILayout.ToggleLeft("是否使用IL2CPP", bUsingIL2CPP);
		if (GUILayout.Button("Build"))
		{
			StartBuild("None");
		}
#elif UNITY_IOS
		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("包地址", GUILayout.Width(50));
		targetBuildDir = EditorGUILayout.TextField(targetBuildDir);
		GUILayout.EndHorizontal();

		bUsingIL2CPP = EditorGUILayout.ToggleLeft("是否使用IL2CPP", bUsingIL2CPP);
		if (GUILayout.Button("Build"))
		{
			QualitySettings.lodBias = 2.4f;
			StartBuild("None");
		}
		EditorGUILayout.LabelField(" ", GUILayout.Width(20));
#elif UNITY_STANDALONE
        if (GUILayout.Button("Build"))
        {
            StartBuild("PC");
        }
#endif
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("SourcePath:", GUILayout.Width(100));
		assetBundleSourcePath = EditorGUILayout.TextField(assetBundleSourcePath);
		EditorGUILayout.LabelField(" ", GUILayout.Width(20));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Build AssetBundles", GUILayout.Width(position.width - 200)))
		{
			StartBuildAssetBundles();
		}
		if (GUILayout.Button("Clear AssetBundle Names", GUILayout.Width(200)))
		{
			ClearAssetBundlesName();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("From:	" + GetAssetBundleTargetPath());
		EditorGUILayout.LabelField("To:	" + GetAssetBundleStreamingPath());
		if (GUILayout.Button("Copy AssetBundles", GUILayout.Width(position.width - 20)))
		{
			CopyDir(GetAssetBundleTargetPath(), GetAssetBundleStreamingPath());
			EditorUtility.DisplayDialog("完成", "拷贝完成。", "确定");
			AssetDatabase.Refresh();
		}


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("相册提示:");
		photo = GUILayout.TextArea(photo, GUILayout.Width(position.width - 4));

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Build Log:");
		sBuildLog = GUILayout.TextArea(sBuildLog, GUILayout.Width(position.width - 4));

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("场景列表:");

		bool bSet = false;
		GUILayout.BeginHorizontal();
		bool bSelectAll = false;
		if (GUILayout.Button("全选", GUILayout.Width(120)))
		{
			bSet = true;
			bSelectAll = true;
		}
		bool bDeselectAll = false;
		if (GUILayout.Button("全不选", GUILayout.Width(120)))
		{
			bSet = true;
			bDeselectAll = true;
		}
		GUILayout.EndHorizontal();

		var maps = EditorBuildSettings.scenes;
		if (containsMaps == null)
		{
			containsMaps = new List<string>();
			foreach (var m in maps)
			{
				if (m.enabled)
				{
					if (!containsMaps.Contains(m.path))
					{
						containsMaps.Add(m.path);
					}
				}
			}
		}
		foreach (var m in maps)
		{
			bool bm = containsMaps.Contains(m.path);
			bm = EditorGUILayout.ToggleLeft(m.path, bm);
			if (bm && !containsMaps.Contains(m.path))
			{
				if (!containsMaps.Contains(m.path))
				{
					containsMaps.Add(m.path);
				}
				m.enabled = true;
				bSet = true;
				break;
			}
			else if (!bm && containsMaps.Contains(m.path))
			{
				containsMaps.Remove(m.path);
				m.enabled = false;
				bSet = true;
				break;
			}
		}
		if (bSet)
		{
			if (bSelectAll || bDeselectAll)
			{
				containsMaps.Clear();
			}
			var amap = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length];
			for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				var enable = bDeselectAll ? false : (bSelectAll ? true : containsMaps.Contains(EditorBuildSettings.scenes[i].path));
				amap[i] = new EditorBuildSettingsScene(EditorBuildSettings.scenes[i].path, enable);
				if (enable)
				{
					containsMaps.Add(EditorBuildSettings.scenes[i].path);
				}
			}
			EditorBuildSettings.scenes = amap;
		}
		EditorGUILayout.EndScrollView();
	}

	private void ClearAssetBundlesName()
	{
		int length = AssetDatabase.GetAllAssetBundleNames().Length;
		string[] oldAssetBundleNames = new string[length];
		for (int i = 0; i < length; i++)
		{
			oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
		}
		for (int j = 0; j < oldAssetBundleNames.Length; j++)
		{
			AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
		}
	}

	private static string GetAssetBundleStreamingPath()
	{
		return Application.streamingAssetsPath + "/" + Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget);
	}
	private static string GetAssetBundleSourcePath()
	{
		return Application.dataPath + "/" + assetBundleSourcePath;
	}
	private static string GetAssetBundleTargetPath()
	{
		var p = Path.GetFullPath(Path.Combine(Application.dataPath, "../../ab1/"));
		return Path.Combine(p, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget)); ;
	}

	static string assetBundleSourcePath
	{
		get
		{
			var path = PlayerPrefs.GetString("sourcepath");
			if (string.IsNullOrEmpty(path))
			{
				return "RemoteResources";
			}
			return path;
		}
		set
		{
			PlayerPrefs.SetString("sourcepath", value);
		}
	}
	public static void StartBuildAssetBundles()
	{
		string sourcePath = GetAssetBundleSourcePath();
		string outputPath = GetAssetBundleTargetPath();

		packedFile.Clear();

		Pack(sourcePath);
		if (!Directory.Exists(outputPath))
		{
			Directory.CreateDirectory(outputPath);
		}
		//根据BuildSetting里面所激活的平台进行打包 设置过AssetBundleName的都会进行打包
		BuildPipeline.BuildAssetBundles(outputPath

#if UNITY_ANDROID
			, BuildAssetBundleOptions.ChunkBasedCompression
#else
			, BuildAssetBundleOptions.ChunkBasedCompression
#endif
			, EditorUserBuildSettings.activeBuildTarget);
		var versions = EnumFileMd5(outputPath, outputPath);
		File.WriteAllText(outputPath + "/versions.txt", versions);
		System.Diagnostics.Process.Start(outputPath);

		//ClearAssetBundlesName();

		AssetDatabase.Refresh();
	}
	static void Pack(string source)
	{
		//Debug.Log("Pack source " + source);
		DirectoryInfo folder = new DirectoryInfo(source);
		FileSystemInfo[] files = folder.GetFileSystemInfos();
		int length = files.Length;
		for (int i = 0; i < length; i++)
		{
			if (files[i] is DirectoryInfo)
			{
				Pack(files[i].FullName);
			}
			else
			{
				if (!files[i].Name.EndsWith(".meta")
					&& !files[i].Name.EndsWith(".xlsx"))
				{
					fileWithDepends(files[i].FullName);
				}
			}
		}

		foreach (var d in dDependences)
		{
			var imp = AssetImporter.GetAtPath(d.Key);
			if (imp == null)
			{
				Debug.Log($"Invalid importer {d.Key}");
				continue;
			}
			PackItem(d.Key, imp);
		}
	}
	public List<string> indepentPrefabs;
	static List<string> packedFile = new List<string>();
	static Dictionary<string, int> dDependences = new Dictionary<string, int>();
	//设置要打包的文件
	static void fileWithDepends(string source)
	{
		if (packedFile.Contains(source))
		{
			return;
		}
		packedFile.Add(source);

		string _source = Replace(source);
		string _assetPath = _source;
		if (_source.StartsWith(Application.dataPath))
		{
			_assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
		}
		if (dDependences.ContainsKey(_assetPath))
		{
			dDependences[_assetPath]++;
		}
		else
		{
			dDependences.Add(_assetPath, 1);
		}
		//PackItem(_assetPath, AssetImporter.GetAtPath(_assetPath));

		//自动获取依赖项并给其资源设置AssetBundleName
		string[] dps = AssetDatabase.GetDependencies(_assetPath);
		foreach (var dp in dps)
		{
			if (dp.EndsWith(".cs"))
				continue;
			AssetImporter assetImporter = AssetImporter.GetAtPath(dp);
			if (assetImporter is TextureImporter)
			{
				TextureImporter tai = assetImporter as TextureImporter;
				if (!string.IsNullOrEmpty(tai.spritePackingTag))
				{
					tai.SetAssetBundleNameAndVariant(tai.spritePackingTag + UAssetBundleDownloader.AssetBundleSuffix, null);
				}
				else
				{
					tai.SetAssetBundleNameAndVariant("tdefault" + UAssetBundleDownloader.AssetBundleSuffix, null);
				}
			}
			else
			{
				if (dDependences.ContainsKey(dp))
				{
					dDependences[dp]++;
				}
				else
				{
					dDependences.Add(dp, 1);
				}
				//PackItem(dp, assetImporter);
			}
		}
	}

	private static void PackItem(string dp, AssetImporter assetImporter)
	{
		string pathTmp = dp.Substring("Assets".Length + 1);
		string assetName = pathTmp.Substring(pathTmp.IndexOf("/") + 1).Replace(" ", "_");
		assetName = assetName.Replace(Path.GetExtension(assetName), UAssetBundleDownloader.AssetBundleSuffix);
		assetImporter.SetAssetBundleNameAndVariant(assetName, string.Empty);

		//fileWithDepends(assetImporter.assetPath);
	}

	static string Replace(string s)
	{
		return s.Replace("\\", "/");
	}



	List<string> containsMaps;
	string sdir
	{
		get
		{
			return Application.dataPath.Replace("/Assets", "/Release/");
		}
	}
	string targetBuildDir
	{
		get
		{
			return PlayerPrefs.GetString("tbd" + target);
		}
		set
		{
			PlayerPrefs.SetString("tbd" + target, value);
		}
	}
	string _photo;
	string photo
	{
		get
		{
			if (string.IsNullOrEmpty(_photo))
			{
				_photo = "APP需要获得您的允许才能访问相册。";
			}
			return _photo;
		}
		set
		{
			_photo = value;
		}
	}

	string sbuilddir;
	void StartBuild(string type)
	{
		// 		OneKeyBuild.DoOneKey(false);
		/*if (!UseAB)
		{
			EditorUtility.DisplayDialog("error", "正式包必须使用AB", "确定");
			return;
		}*/
		sbuilddir = sdir;
		if (!string.IsNullOrEmpty(targetBuildDir))
		{
			sbuilddir = targetBuildDir + "/";
		}
		if (!Directory.Exists(sbuilddir))
		{
			Directory.CreateDirectory(sbuilddir);
		}

#if UNITY_ANDROID && Cardboard
		string aarfilename = "gvr-permissionsupport-release.aar";
		string aarpath = Application.dataPath + "/Plugins/Android/";
		string aartopath = new DirectoryInfo(Application.dataPath).Parent.FullName + "/";
		if (File.Exists(aarpath + aarfilename))
		{
			if (File.Exists(aartopath + aarfilename))
			{
				File.Delete(aartopath + aarfilename);
			}
			File.Move(aarpath + aarfilename, aartopath + aarfilename);
			AssetDatabase.Refresh(ImportAssetOptions.Default);
		}
#endif

		string sbuildChannel = "";

		string fullName = "";
		fullName = PlayerSettings.applicationIdentifier + "." + sversion + "." + (bUsingIL2CPP ? "x64" : "x86");
		string spath = sbuilddir + fullName;
		BuildTarget bt = BuildTarget.Android;
#if UNITY_ANDROID
		spath += ".apk";
		bt = BuildTarget.Android;
#elif UNITY_IOS
		//ios needs a directory path rather than a file path;
		bt = BuildTarget.iOS;

#elif UNITY_STANDALONE
        sbuilddir += "Game";
        spath = sbuilddir;
        if (Directory.Exists(sbuilddir))
        {
            Directory.Delete(sbuilddir, true);
        }
        Directory.CreateDirectory(sbuilddir);
        spath += "/Game.exe";
        bt = BuildTarget.StandaloneWindows64;
#else
		var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
		if (!string.IsNullOrEmpty(defines))
		{
			var adef = defines.Replace(";", ".");
			spath += adef;
		}
		if (Directory.Exists(spath))
		{
			Directory.Delete(spath, true);
		}
		if (File.Exists(spath + ".zip"))
		{
			File.Delete(spath + ".zip");
		}
		Directory.CreateDirectory(spath);
#endif
		LogWithTime(string.Format("Build to {0}", spath));
		string sresult = "";
#if UNITY_2018_1_OR_NEWER
		var r = BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), spath, bt, BuildOptions.None);
		sresult = r.summary.result.ToString();
#else
		sresult = BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), spath, bt, BuildOptions.None);
#endif
#if UNITY_IOS

		string projPath = PBXProject.GetPBXProjectPath(spath);
		PBXProject proj = new PBXProject();
		proj.ReadFromFile(projPath);

		string targetGuid = proj.TargetGuidByName("Unity-iPhone");
		proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libstdc++.tbd", "Frameworks/libstdc++.tbd", PBXSourceTree.Sdk));
		proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libicucore.tbd", "Frameworks/libicucore.tbd", PBXSourceTree.Sdk));
		proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
		proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libsqlite3.tbd", "Frameworks/libsqlite3.tbd", PBXSourceTree.Sdk));
		proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.tbd", PBXSourceTree.Sdk));
		proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libresolv.tbd", "Frameworks/libresolv.tbd", PBXSourceTree.Sdk));

		proj.AddFrameworkToProject(targetGuid, "JavaScriptCore.framework", false);
		proj.AddFrameworkToProject(targetGuid, "MessageUI.framework", false);
		proj.AddFrameworkToProject(targetGuid, "AdSupport.framework", false);
		proj.AddFrameworkToProject(targetGuid, "VideoToolbox.framework", false);
		proj.AddFrameworkToProject(targetGuid, "CoreText.framework", false);
		proj.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
		proj.AddFrameworkToProject(targetGuid, "Accelerate.framework", false);
		proj.AddFrameworkToProject(targetGuid, "QuartzCore.framework", false);
		proj.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);

		proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "false");
		proj.SetBuildProperty(targetGuid, "OTHER_LDFLAGS", "$(inherited) -weak_framework CoreMotion -weak-lSystem -weak_framework Photos -framework AssetsLibrary -framework MobileCoreServices -framework ImageIO -all_load -ObjC");

		proj.WriteToFile(projPath);

		PlistDocument plistDocument = new PlistDocument();
		plistDocument.ReadFromFile(spath + "/Info.plist");

		plistDocument.root.SetString("CFBundleDevelopmentRegion", "China");

		PlistElementDict dict = plistDocument.root.AsDict();
		dict.SetString("NSPhotoLibraryUsageDescription", "需要访问相册。");
		dict.SetString("NSCameraUsageDescription", "需要启用摄像头。");
		dict.SetString("NSMicrophoneUsageDescription", "需要启用麦克风。");

		{
			PlistElementArray parray = plistDocument.root.CreateArray("CFBundleURLTypes");
			dict = parray.AddDict();
			dict.SetString("CFBundleTypeRole", "Editor");
			dict.SetString("CFBundleURLName", "weixin");
			PlistElementArray array2 = dict.CreateArray("CFBundleURLSchemes");
			array2.AddString(SDK_WeChat.appid);  //weixin
		}

		{
			PlistElementArray parray = plistDocument.root.CreateArray("LSApplicationQueriesSchemes");
			parray.AddString("wechat");
			parray.AddString("weixin");
		}

		plistDocument.WriteToFile(spath + "/Info.plist");
#endif

		if (string.IsNullOrEmpty(sresult) || sresult == "Succeeded")
		{
			if (!string.IsNullOrEmpty(sBuildLog))
			{
				LogWithTime(string.Format("BuildLog {0}:{1}", buildCode, sBuildLog));
				sBuildLog = "";
			}
			LogWithTime(string.Format("Build complete, {0} {1}({2})", spath, buildCode, buildCode));

			buildCode = buildCode.Substring(0, buildCode.LastIndexOf(".") + 1) + (int.Parse(buildCode.Substring(buildCode.LastIndexOf(".") + 1)) + 1);
#if UNITY_STANDALONE
            var pluginDir = Application.dataPath + "/Plugins/x64";
            var destiPluginDir = sbuilddir + "/Game_Data/Plugins";
            if (Directory.Exists(pluginDir))
            {
                CopyDir(pluginDir, destiPluginDir);
            }

            BuildVersionFile();
#endif
#if UNITY_IOS
			sbuilddir = spath;
#endif
			System.Diagnostics.Process.Start(sbuilddir);

		}
		else
		{
			LogWithTime(string.Format("Build failed, {0}", sresult));
		}
	}

	const string versionFileName = "version.txt";
	private void BuildVersionFile()
	{
		LogWithTime("Start To Build Version File");
		string sVersionFilePath = sbuilddir + "/" + versionFileName;
		if (File.Exists(sVersionFilePath))
		{
			File.Delete(sVersionFilePath);
		}
		string sresu = EnumFileMd5(sbuilddir, sbuilddir);
		string sresult = "version|" + PlayerSettings.bundleVersion + "\r\n" + sresu;
		File.AppendAllText(sVersionFilePath, sresult);
		LogWithTime("Build Version File Success");
	}
	private static string EnumFileMd5(string sdir, string outputDir)
	{
		string sresult = "";
		foreach (var s in Directory.GetFiles(sdir))
		{
			FileInfo fi = new FileInfo(s);
			if (fi.Name.EndsWith(".aspx") || fi.Name.EndsWith(".config") || fi.Name.Contains("/mono/") || fi.Name.Contains("\\mono\\") || fi.Name.EndsWith("version.txt"))
			{
				continue;
			}

			sresult += fi.FullName.Replace("\\", "/").Replace(outputDir.Replace("\\", "/"), "");
			sresult += "|" + MD5String.GetMD5HashFromFile(s) + "|" + fi.Length;
			sresult += "\r\n";
		}
		foreach (var s in Directory.GetDirectories(sdir))
		{
			sresult += EnumFileMd5(s, outputDir);
		}
		return sresult;
	}

	private void CopyDir(string srcPath, string aimPath)
	{
		try
		{
			// 检查目标目录是否以目录分割字符结束如果不是则添加
			if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
			{
				aimPath += System.IO.Path.DirectorySeparatorChar;
			}
			// 判断目标目录是否存在如果不存在则新建
			if (!System.IO.Directory.Exists(aimPath))
			{
				System.IO.Directory.CreateDirectory(aimPath);
			}
			// 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
			// 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
			// string[] fileList = Directory.GetFiles（srcPath）；
			string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
			// 遍历所有的文件和目录
			foreach (string file in fileList)
			{
				// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
				if (System.IO.Directory.Exists(file))
				{
					CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
				}
				// 否则直接Copy文件
				else
				{
					System.IO.File.Copy(file, aimPath + System.IO.Path.GetFileName(file), true);
				}
			}
		}
		catch (Exception e)
		{
			throw;
		}
	}

	void LogWithTime(string slog)
	{
		if (!Directory.Exists(sdir))
		{
			Directory.CreateDirectory(sdir);
		}
		string s = string.Format("[{0}]{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), slog);
		Debug.Log(s);

		string logPath = sdir + "/BuildLog.txt";
		if (File.Exists(logPath))
		{
			using (StreamWriter sw = File.AppendText(logPath))
			{
				sw.Write("\r\n" + s);
			}
		}
		else
		{
			File.WriteAllText(logPath, s);
		}
	}

	private static string[] FindEnabledEditorScenes()
	{
		List<string> EditorScenes = new List<string>();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			if (!scene.enabled)
				continue;
			EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}

}

public class Platform
{
	public static string GetPlatformFolder(BuildTarget target)
	{
		switch (target)
		{
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.iOS:
				return "IOS";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
			case BuildTarget.WebGL:
				return "WebGL";
			default:
				return "Platform";
		}
	}
}
