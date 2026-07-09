using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Warzone.Tests.Architecture
{
    public sealed class AssemblyDefinitionTests
    {
        [Test]
        public void CoreAssembly_DisablesEngineReferences()
        {
            string text = ReadAsmdef("Core", "Warzone.Core.asmdef");
            StringAssert.Contains("\"noEngineReferences\": true", text);
        }

        [Test]
        public void ContentAssembly_DisablesEngineReferences()
        {
            string text = ReadAsmdef("Content", "Warzone.Content.asmdef");
            StringAssert.Contains("\"noEngineReferences\": true", text);
        }

        [Test]
        public void CombatAssembly_HasExpectedDependencyBoundary()
        {
            string text = ReadAsmdef("Combat", "Warzone.Combat.asmdef");
            StringAssert.Contains("\"noEngineReferences\": true", text);
            StringAssert.DoesNotContain("Warzone.Campaign", text);
            StringAssert.DoesNotContain("Warzone.Application", text);
            StringAssert.DoesNotContain("Warzone.Runtime", text);
            StringAssert.DoesNotContain("Warzone.Sandbox", text);
        }

        [Test]
        public void CampaignAssembly_HasExpectedDependencyBoundary()
        {
            string text = ReadAsmdef("Campaign", "Warzone.Campaign.asmdef");
            StringAssert.Contains("\"noEngineReferences\": true", text);
            StringAssert.DoesNotContain("Warzone.Combat", text);
            StringAssert.DoesNotContain("Warzone.Application", text);
            StringAssert.DoesNotContain("Warzone.Runtime", text);
            StringAssert.DoesNotContain("Warzone.Sandbox", text);
        }

        [Test]
        public void ApplicationAssembly_HasExpectedDependencyBoundary()
        {
            string text = ReadAsmdef("Application", "Warzone.Application.asmdef");
            StringAssert.Contains("\"noEngineReferences\": true", text);
            StringAssert.DoesNotContain("Warzone.Runtime", text);
            StringAssert.DoesNotContain("Warzone.Sandbox", text);
        }

        [Test]
        public void RuntimeAssembly_DoesNotReferenceSandbox()
        {
            string text = ReadAsmdef("Runtime", "Warzone.Runtime.asmdef");
            StringAssert.DoesNotContain("Warzone.Sandbox", text);
        }

        [Test]
        public void SandboxAssembly_CanReferenceRuntime()
        {
            string text = ReadAsmdef("Sandbox", "Warzone.Sandbox.asmdef");
            StringAssert.Contains("Warzone.Runtime", text);
        }

        [Test]
        public void EditorAssembly_IsEditorOnly()
        {
            string text = ReadAsmdef("Editor", "Warzone.Editor.asmdef");
            StringAssert.Contains("\"includePlatforms\": [", text);
            StringAssert.Contains("Editor", text);
        }

        [Test]
        public void NonSandboxAssemblies_DoNotReferenceSandbox()
        {
            AssertNoSandboxReference("Core", "Warzone.Core.asmdef");
            AssertNoSandboxReference("Content", "Warzone.Content.asmdef");
            AssertNoSandboxReference("Combat", "Warzone.Combat.asmdef");
            AssertNoSandboxReference("Campaign", "Warzone.Campaign.asmdef");
            AssertNoSandboxReference("Application", "Warzone.Application.asmdef");
            AssertNoSandboxReference("Runtime", "Warzone.Runtime.asmdef");
        }

        [Test]
        public void LegacyAsmdefs_AreRemovedInFavorOfNewModuleLayout()
        {
            Assert.That(File.Exists(Path.Combine("Assets", "_Project", "Adapters", "Warzone.Adapters.asmdef")), Is.False);
            Assert.That(File.Exists(Path.Combine("Assets", "_Project", "Controls", "Warzone.Controls.asmdef")), Is.False);
            Assert.That(File.Exists(Path.Combine("Assets", "_Project", "Presentation", "Warzone.Presentation.asmdef")), Is.False);
            Assert.That(File.Exists(Path.Combine("Assets", "_Project", "Framework", "Warzone.Framework.asmdef")), Is.False);
            Assert.That(File.Exists(Path.Combine("Assets", "_Project", "Foundation", "Warzone.Foundation.asmdef")), Is.False);
            Assert.That(File.Exists(Path.Combine("Assets", "_Project", "Meta", "Warzone.Meta.asmdef")), Is.False);
        }

        [Test]
        public void ProjectRoot_ContainsTargetModuleDirectories()
        {
            string projectRoot = Path.Combine("Assets", "_Project");
            string[] requiredDirectories =
            {
                "Core",
                "Content",
                "Combat",
                "Campaign",
                "Application",
                "Runtime",
                "Sandbox",
                "Editor",
                "Tests"
            };

            foreach (string directory in requiredDirectories)
            {
                Assert.That(Directory.Exists(Path.Combine(projectRoot, directory)), Is.True, "Missing directory: " + directory);
            }
        }

        [Test]
        public void LegacyTopLevelDirectories_AreRemoved()
        {
            string projectRoot = Path.Combine("Assets", "_Project");
            string[] legacyDirectories =
            {
                "Foundation",
                "Meta",
                "Adapters",
                "Controls",
                "Presentation",
                "Framework"
            };

            foreach (string directory in legacyDirectories)
            {
                Assert.That(Directory.Exists(Path.Combine(projectRoot, directory)), Is.False, "Legacy directory still exists: " + directory);
            }
        }

        [Test]
        public void DomainLayers_DoNotReferenceUnityEngineInSource()
        {
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Core"), "UnityEngine");
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Content", "Definitions"), "UnityEngine");
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Content", "Catalog"), "UnityEngine");
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Content", "Queries"), "UnityEngine");
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Content", "Validation"), "UnityEngine");
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Combat"), "UnityEngine");
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Campaign"), "UnityEngine");
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Application"), "UnityEngine");
        }

        [Test]
        public void CombatSource_DoesNotReferenceRuntimeOrCampaignLayers()
        {
            string combatDirectory = Path.Combine("Assets", "_Project", "Combat");
            AssertDirectoryDoesNotContain(combatDirectory, "Warzone.Campaign");
            AssertDirectoryDoesNotContain(combatDirectory, "Warzone.Application");
            AssertDirectoryDoesNotContain(combatDirectory, "Warzone.Runtime");
            AssertDirectoryDoesNotContain(combatDirectory, "Warzone.Sandbox");
        }

        [Test]
        public void CampaignSource_DoesNotReferenceCombatOrRuntimeLayers()
        {
            string campaignDirectory = Path.Combine("Assets", "_Project", "Campaign");
            AssertDirectoryDoesNotContain(campaignDirectory, "Warzone.Combat");
            AssertDirectoryDoesNotContain(campaignDirectory, "Warzone.Application");
            AssertDirectoryDoesNotContain(campaignDirectory, "Warzone.Runtime");
            AssertDirectoryDoesNotContain(campaignDirectory, "Warzone.Sandbox");
        }

        [Test]
        public void ApplicationSource_DoesNotReferenceRuntimeOrSandboxLayers()
        {
            string applicationDirectory = Path.Combine("Assets", "_Project", "Application");
            AssertDirectoryDoesNotContain(applicationDirectory, "Warzone.Runtime");
            AssertDirectoryDoesNotContain(applicationDirectory, "Warzone.Sandbox");
        }

        [Test]
        public void RuntimeAndSandbox_DoNotUseLegacyNamespaces()
        {
            string adaptersNamespace = BuildNamespaceToken("Adapters");
            string controlsNamespace = BuildNamespaceToken("Controls");
            string presentationNamespace = BuildNamespaceToken("Presentation");
            string adaptersUsing = BuildUsingToken("Adapters");
            string controlsUsing = BuildUsingToken("Controls");
            string presentationUsing = BuildUsingToken("Presentation");

            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Runtime"), adaptersNamespace);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Runtime"), controlsNamespace);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Runtime"), presentationNamespace);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Runtime"), adaptersUsing);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Runtime"), controlsUsing);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Runtime"), presentationUsing);

            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Sandbox"), adaptersNamespace);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Sandbox"), controlsNamespace);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Sandbox"), presentationNamespace);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Sandbox"), adaptersUsing);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Sandbox"), controlsUsing);
            AssertDirectoryDoesNotContain(Path.Combine("Assets", "_Project", "Sandbox"), presentationUsing);
        }

        private static void AssertNoSandboxReference(string folder, string fileName)
        {
            string text = ReadAsmdef(folder, fileName);
            StringAssert.DoesNotContain("Warzone.Sandbox", text);
        }

        private static string ReadAsmdef(string folder, string fileName)
        {
            string asmdefPath = Path.Combine("Assets", "_Project", folder, fileName);
            Assert.That(File.Exists(asmdefPath), Is.True, "Missing asmdef: " + asmdefPath);
            return File.ReadAllText(asmdefPath);
        }

        private static void AssertDirectoryDoesNotContain(string directoryPath, string text)
        {
            Assert.That(Directory.Exists(directoryPath), Is.True, "Missing directory: " + directoryPath);

            string[] sourceFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
            string[] matchingFiles = sourceFiles
                .Where(path => File.ReadAllText(path).Contains(text))
                .ToArray();

            Assert.That(matchingFiles, Is.Empty, "Unexpected text found in: " + string.Join(", ", matchingFiles));
        }

        private static string BuildNamespaceToken(string suffix)
        {
            return "namespace " + "Warzone." + suffix;
        }

        private static string BuildUsingToken(string suffix)
        {
            return "using " + "Warzone." + suffix;
        }
    }
}


