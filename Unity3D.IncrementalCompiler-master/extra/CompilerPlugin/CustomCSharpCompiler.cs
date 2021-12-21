﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEngine;

internal class CustomCSharpCompiler : MonoCSharpCompiler
{
#if UNITY4
	public CustomCSharpCompiler(MonoIsland island, bool runUpdater) : base(island)
	{
	}
#else
	public CustomCSharpCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
	{
	}
#endif

	private string[] GetAdditionalReferences()
	{
		// calling base method via reflection
		var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
		var methodInfo = GetType().BaseType.GetMethod(nameof(GetAdditionalReferences), bindingFlags);
		var result = (string[])methodInfo.Invoke(this, null);
		return result;
	}

	private string GetCompilerPath(List<string> arguments)
	{
		// calling base method via reflection
		var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
		var methodInfo = GetType().BaseType.GetMethod(nameof(GetCompilerPath), bindingFlags);
		var result = (string)methodInfo.Invoke(this, new object[] {arguments});
		return result;
	}

	private string GetUniversalCompilerPath()
	{
		var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Compiler");
		var compilerPath = Path.Combine(basePath, "UniversalCompiler.exe");
		return File.Exists(compilerPath) ? compilerPath : null;
	}

	// Copy of MonoCSharpCompiler.StartCompiler()
	// The only reason it exists is to call the new implementation
	// of GetCompilerPath(...) which is non-virtual unfortunately.
	protected override Program StartCompiler()
	{
		var arguments = new List<string>
		{
			"-debug",
			"-target:library",
			"-nowarn:0169",
			"-out:" + PrepareFileName(_island._output),
			"-unsafe"
		};
		foreach (var reference in _island._references)
		{
			arguments.Add("-r:" + PrepareFileName(reference));
		}

		foreach (var define in _island._defines.Distinct())
		{
			arguments.Add("-define:" + define);
		}

		foreach (var file in _island._files)
		{
			arguments.Add(PrepareFileName(file));
		}

		var additionalReferences = GetAdditionalReferences();
		foreach (string path in additionalReferences)
		{
			var text = Path.Combine(GetProfileDirectoryViaReflection(), path);
			if (File.Exists(text))
			{
				arguments.Add("-r:" + PrepareFileName(text));
			}
		}

		var universalCompilerPath = GetUniversalCompilerPath();
		if (universalCompilerPath != null)
		{
			// use universal compiler.
			arguments.Add("-define:__UNITY_PROCESSID__" + System.Diagnostics.Process.GetCurrentProcess().Id);

			// this function should be run because it addes an item to arguments
			var compilerPath = GetCompilerPath(arguments);

			var rspFileName = "Assets/mcs.rsp";
			if (File.Exists(rspFileName))
			{
				arguments.Add("@" + rspFileName);
			}
			else
			{
				var defaultCompilerName = Path.GetFileNameWithoutExtension(compilerPath);
				rspFileName = "Assets/" + defaultCompilerName + ".rsp";
				if (File.Exists(rspFileName))
					arguments.Add("@" + rspFileName);
			}

			return StartCompiler(_island._target, universalCompilerPath, arguments);
		}
		else
		{
			// fallback to the default compiler.
			Debug.LogWarning($"Universal C# compiler not found in project directory. Use the default compiler");
			return base.StartCompiler();
		}
	}

	// In Unity 5.5 and earlier GetProfileDirectory() was an instance method of MonoScriptCompilerBase class.
	// In Unity 5.6 the method is removed and the profile directory is detected differently.
	private string GetProfileDirectoryViaReflection()
	{
		var monoScriptCompilerBaseType = typeof(MonoScriptCompilerBase);
		var getProfileDirectoryMethodInfo = monoScriptCompilerBaseType.GetMethod("GetProfileDirectory", BindingFlags.NonPublic | BindingFlags.Instance);
		if (getProfileDirectoryMethodInfo != null)
		{
			// For any Unity version prior to 5.6
			string result = (string)getProfileDirectoryMethodInfo.Invoke(this, null);
			return result;
		}

		// For Unity 5.6
		var monoIslandType = typeof(MonoIsland);
		var apiCompatibilityLevelFieldInfo = monoIslandType.GetField("_api_compatibility_level");
		var apiCompatibilityLevel = (ApiCompatibilityLevel)apiCompatibilityLevelFieldInfo.GetValue(_island);

		string profile;
		if (apiCompatibilityLevel != ApiCompatibilityLevel.NET_2_0)
		{
			profile = GetMonoProfileLibDirectory(apiCompatibilityLevel);
		}
		else
		{
			profile = "2.0-api";
		}

		string profileDirectory = GetProfileDirectory(profile, "MonoBleedingEdge");
		return profileDirectory;
	}

	private static string GetMonoProfileLibDirectory(ApiCompatibilityLevel apiCompatibilityLevel)
	{
		var buildPipelineType = typeof(BuildPipeline);
		var compatibilityProfileToClassLibFolderMethodInfo = buildPipelineType.GetMethod("CompatibilityProfileToClassLibFolder", BindingFlags.NonPublic | BindingFlags.Static);
		string profile = (string)compatibilityProfileToClassLibFolderMethodInfo.Invoke(null, new object[] { apiCompatibilityLevel });

		var apiCompatibilityLevelNet46 = (ApiCompatibilityLevel)3;
		string monoInstallation = apiCompatibilityLevel != apiCompatibilityLevelNet46 ? "Mono" : "MonoBleedingEdge";
		return GetProfileDirectory(profile, monoInstallation);
	}

	private static string GetProfileDirectory(string profile, string monoInstallation)
	{
		string monoInstallation2 = MonoInstallationFinder.GetMonoInstallation(monoInstallation);
		return Path.Combine(monoInstallation2, Path.Combine("lib", Path.Combine("mono", profile)));
	}
}
