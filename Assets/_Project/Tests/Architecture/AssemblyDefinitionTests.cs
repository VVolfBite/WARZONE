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

        [Test]
        public void CombatAssembly_DoesNotReferenceControlsOrAdapters()
        {
            string asmdefPath = Path.Combine("Assets", "_Project", "Combat", "Warzone.Combat.asmdef");
            string text = File.ReadAllText(asmdefPath);

            StringAssert.DoesNotContain("Warzone.Controls", text);
            StringAssert.DoesNotContain("Warzone.Adapters", text);
        }

        [Test]
        public void MetaAssembly_DoesNotReferenceAdapters()
        {
            string asmdefPath = Path.Combine("Assets", "_Project", "Meta", "Warzone.Meta.asmdef");
            string text = File.ReadAllText(asmdefPath);

            StringAssert.DoesNotContain("Warzone.Adapters", text);
        }

        [Test]
        public void ControlsAssembly_DoesNotReferenceAdapters()
        {
            string asmdefPath = Path.Combine("Assets", "_Project", "Controls", "Warzone.Controls.asmdef");
            string text = File.ReadAllText(asmdefPath);

            StringAssert.DoesNotContain("Warzone.Adapters", text);
        }
    }
}
