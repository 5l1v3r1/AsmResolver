using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Code.Native;
using AsmResolver.DotNet.Signatures;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE;
using AsmResolver.PE.DotNet.Builder;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Strings;
using AsmResolver.PE.DotNet.Metadata.Tables;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using AsmResolver.PE.Imports;
using AsmResolver.Tests.Runners;
using Xunit;

namespace AsmResolver.DotNet.Tests.Code.Native
{
    public class NativeMethodBodyTest : IClassFixture<TemporaryDirectoryFixture>
    {
        private readonly TemporaryDirectoryFixture _fixture;
        private readonly ModuleDefinition _module;
        private MethodDefinition _method;

        public NativeMethodBodyTest(TemporaryDirectoryFixture fixture)
        {
            _fixture = fixture;

            var assembly = new AssemblyDefinition("MyModule", new Version(1, 0, 0, 0));
            _module = new ModuleDefinition("MyModule.exe");
            _module.IsILOnly = false;
            _module.IsBit32Required = true;
            assembly.Modules.Add(_module);
            
            var moduleType = _module.GetOrCreateModuleType();

            var importer = new ReferenceImporter(_module);

            var returnType = importer.ImportTypeSignature(new CustomModifierTypeSignature(
                new TypeReference(_module.CorLibTypeFactory.CorLibScope,
                    "System.Runtime.CompilerServices", "CallConvCdecl"), false, _module.CorLibTypeFactory.Int32));
            
            _method = new MethodDefinition("NativeMethod",
                MethodAttributes.Static | MethodAttributes.PInvokeImpl,
                MethodSignature.CreateStatic(returnType));
            
            _method.ImplAttributes = MethodImplAttributes.Native | MethodImplAttributes.Unmanaged | MethodImplAttributes.PreserveSig;
            moduleType.Methods.Add(_method);
        }

        private MethodDefinitionRow FindMethodRow(IPEImage image, string methodName)
        {
            var metadata = image.DotNetDirectory.Metadata;
            var stringsStream = metadata.GetStream<StringsStream>();
            var methodRow = metadata
                .GetStream<TablesStream>()
                .GetTable<MethodDefinitionRow>(TableIndex.Method)
                .First(m => stringsStream.GetStringByIndex(m.Name) == methodName);
            return methodRow;
        }

        [Fact]
        public void SerializingNativeMethodBodyShouldCopyOverNativeCodeStream()
        {
            var nativeCode = new byte[]
            {
                0xb8, 0x39, 0x05, 0x00, 0x00,   // mov eax, 1337
                0xc3                            // ret
            };
            _method.MethodBody = new NativeMethodBody(nativeCode);

            var image = _module.ToPEImage();
            var methodRow = FindMethodRow(image, _method.Name);

            var actualCode = methodRow.Body.CreateReader().ReadToEnd();
            Assert.Equal(nativeCode, actualCode);
        }

        [Fact]
        public void SerializingNativeMethodBodyShouldRun()
        {
            var nativeCode = new byte[]
            {
                0xb8, 0x39, 0x05, 0x00, 0x00,   // mov eax, 1337
                0xc3                            // ret
            };
            _method.MethodBody = new NativeMethodBody(nativeCode);

            var importer = new ReferenceImporter(_module);
            var writeLine = importer.ImportMethod(
                new MemberReference(new TypeReference(_module.CorLibTypeFactory.CorLibScope, "System", "Console"),
                    "WriteLine",
                    MethodSignature.CreateStatic(_module.CorLibTypeFactory.Void, _module.CorLibTypeFactory.Int32)));
            
            var main = new MethodDefinition("Main", MethodAttributes.Static,
                MethodSignature.CreateStatic(_module.CorLibTypeFactory.Void));
            main.CilMethodBody = new CilMethodBody(main)
            {
                Instructions =
                {
                    new CilInstruction(CilOpCodes.Call, _method),
                    new CilInstruction(CilOpCodes.Call, writeLine),
                    new CilInstruction(CilOpCodes.Ret)
                }
            };
            _module.GetModuleType().Methods.Add(main);
            _module.ManagedEntrypointMethod = main;
            
            _fixture
                .GetRunner<FrameworkPERunner>()
                .RebuildAndRun(_module, "Print1337.exe", "1337");
        }

        [Fact]
        public void Import()
        {
            var nativeCode = new byte[]
            {
                0x51,                                // push ecx 
                0xff, 0x15, 0x00, 0x00, 0x00, 0x00,  // push dword [rva_printf]
                0xc3                                 // ret
            };
            
            var msvcrt = new ImportedModule("msvcrt.dll");
            var printf = new ImportedSymbol(0, "printf");
            msvcrt.Symbols.Add(printf);

            _method.MethodBody = new NativeMethodBody(nativeCode)
            {
                ImportAddressFixups = {new ImportAddressFixup(printf, 3)}
            };
            
            var image = _module.ToPEImage();
            Assert.Contains(image.Imports, m => 
                m.Name == msvcrt.Name && m.Symbols.Any(s => s.Name == printf.Name));

            using var fs = File.Create("/home/washi/Desktop/output.exe");
            new ManagedPEFileBuilder().CreateFile(image).Write(new BinaryStreamWriter(fs));
        }
    }
}