using System.IO;
using System.Text;

namespace Rx.Unity.Dependency.Patcher {
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = "..";
            var reactiveProjectFolder = Path.Combine(basePath, "reactive/Rx.NET/Source/src/System.Reactive");
            var reactiveCsProj = Path.Combine(reactiveProjectFolder, "System.Reactive.csproj");

            File.WriteAllText(reactiveCsProj,
                File.ReadAllText(reactiveCsProj, Encoding.UTF8)
                    .Replace("<TargetFrameworks>netcoreapp3.1;netstandard2.0;net472;uap10.0.16299;net5.0;net5.0-windows10.0.19041</TargetFrameworks>", "<TargetFramework>netstandard2.0</TargetFramework><LangVersion>9</LangVersion><DefineConstants>NO_NULLABLE_ATTRIBUTES</DefineConstants>")
                    .Replace("<SignAssembly>true</SignAssembly>", "<SignAssembly>false</SignAssembly>"),
                Encoding.UTF8);

            var reactiveTaskObservable = Path.Combine(reactiveProjectFolder, "TaskObservable.cs");
            File.WriteAllText(reactiveTaskObservable,
                File.ReadAllText(reactiveTaskObservable, Encoding.UTF8)
                    .Replace("[AsyncMethodBuilder(typeof(TaskObservableMethodBuilder<>))]", ""),
                Encoding.UTF8);            
        }
    }
}
