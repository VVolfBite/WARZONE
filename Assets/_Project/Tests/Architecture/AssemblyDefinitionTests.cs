using System.IO;
using NUnit.Framework;

namespace Warzone.Tests.Architecture
{
    public sealed class AssemblyDefinitionTests
    {
        [Test]
        public void CombatAssembly_DisablesEngineReferences()
        {
            string asmdefPath = Path.Combine("Assets", "_Project", "Combat", "Warzone.Combat.asmdef");
            string text = File.ReadAllText(asmdefPath);

            StringAssert.Contains("\"noEngineReferences\": true", text);
        }
    }
}
