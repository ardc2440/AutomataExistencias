using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class UnitMeasuredSyncTest: AutofacConfigurator
    {
        private readonly IUnitMeasuredSynchronize _unitMeasuredSynchronize;
        private readonly Domain.Aldebaran.IUnitMeasuredService _aldebaranUnitMeasuredService;
        public UnitMeasuredSyncTest()
        {
            var container = GetContainer();
            _unitMeasuredSynchronize = container.Resolve<IUnitMeasuredSynchronize>();
            _aldebaranUnitMeasuredService = container.Resolve<Domain.Aldebaran.IUnitMeasuredService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranUnitMeasuredService.Get(attempts);
                _unitMeasuredSynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
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
                var data = _aldebaranUnitMeasuredService.Get(attempts);
                _unitMeasuredSynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}