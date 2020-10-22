#if UNITY_EDITOR
using System.IO;
using UnityEditor.Callbacks;

public class AssemblyReferenceFix {

  [DidReloadScripts]
  private static void OnScriptsReloaded() {
    foreach (var filename in Directory.GetFiles(".", "*.csproj")) {
      var content = File.ReadAllText(filename);
      var newContent = content.Replace("<ReferenceOutputAssembly>false</ReferenceOutputAssembly>", "<ReferenceOutputAssembly>true</ReferenceOutputAssembly>");
      if (content != newContent) {
        File.WriteAllText(filename, newContent);
      }
    }
  }
}
#endif