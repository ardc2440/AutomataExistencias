using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class StockSyncTest : AutofacConfigurator
    {
        private readonly IStockSynchronize _stockSynchronize;
        private readonly Domain.Aldebaran.IStockService _aldebaranStockService;
        public StockSyncTest()
        {
            var container = GetContainer();
            _stockSynchronize = container.Resolve<IStockSynchronize>();
            _aldebaranStockService = container.Resolve<Domain.Aldebaran.IStockService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranStockService.Get(attempts);
                _stockSynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
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
                var data = _aldebaranStockService.Get(attempts);
                _stockSynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}