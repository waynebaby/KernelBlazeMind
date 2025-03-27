using KernelBlazeMind.Abstraction.Services.Images;
using KernelBlazeMind.Core.Services.Images;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelBlazeMind.Core.Test
{
    [TestClass]
    public abstract class TestBase
    {
        private static IServiceProvider? serviceProvider;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            ServiceCollection sc = new();
            // Add services to the container.
            sc.AddTransient<IImageNormalizeService, ImageNormalizeService>();

            serviceProvider = sc.BuildServiceProvider();

        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // This method is called once for the test assembly, after all tests are run.
        }

        protected static IServiceProvider ServiceProvider => serviceProvider!;
    }
}
