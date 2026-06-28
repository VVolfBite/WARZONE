using System.IO;
using NUnit.Framework;

namespace Warzone.Tests.Architecture
{
    public sealed class AssemblyDefinitionTests
    {
        [Test]
        public void BattleDomainAssembly_DisablesEngineReferences()
        {
            string asmdefPath = Path.Combine("Assets", "_Project", "Runtime", "BattleDomain", "Warzone.BattleDomain.asmdef");
            string text = File.ReadAllText(asmdefPath);

            StringAssert.Contains("\"noEngineReferences\": true", text);
        }
    }
}
