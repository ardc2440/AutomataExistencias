using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class PackagingSyncTest: AutofacConfigurator
    {
        private readonly IPackagingSynchronize _packagingSynchronize;
        private readonly Domain.Aldebaran.IPackagingService _aldebaranPackagingService;
        public PackagingSyncTest()
        {
            var container = GetContainer();
            _packagingSynchronize = container.Resolve<IPackagingSynchronize>();
            _aldebaranPackagingService = container.Resolve<Domain.Aldebaran.IPackagingService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranPackagingService.Get(attempts);
                _packagingSynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [TestMethod]
        public void UnSync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranPackagingService.Get(attempts);
                _packagingSynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}