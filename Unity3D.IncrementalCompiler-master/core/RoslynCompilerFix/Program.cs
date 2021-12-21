﻿using System;
using System.IO;
using Mono.Cecil;

namespace RoslynCompilerFix
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("RoslynCompilerFix orignal-exe new-exe");
                return 1;
            }

            ProcessDll(args[0], args[1]);
            Console.WriteLine("Done!");
            return 0;
        }

        private static void ProcessDll(string dllPath, string dllPathNew)
        {
            AssemblyDefinition assemblyDef;

            using (var assemblyStream = new MemoryStream(File.ReadAllBytes(dllPath)))
            {
                assemblyDef = AssemblyDefinition.ReadAssembly(assemblyStream);
            }

            CSharpCompilerFix.Process(assemblyDef);

            using (var assemblyStream = File.Create(dllPathNew))
            {
                assemblyDef.Write(assemblyStream);
            }
        }
    }
}
