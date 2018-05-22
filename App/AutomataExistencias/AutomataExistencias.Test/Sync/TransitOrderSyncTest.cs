using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class TransitOrderSyncTest: AutofacConfigurator
    {
        private readonly ITransitOrderSynchronize _transitOrderSynchronize;
        private readonly Domain.Aldebaran.ITransitOrderService _aldebaranTransitOrderService;
        public TransitOrderSyncTest(ITransitOrderSynchronize transitOrderSynchronize)
        {
            var container = GetContainer();
            _transitOrderSynchronize = container.Resolve<ITransitOrderSynchronize>();
            _aldebaranTransitOrderService = container.Resolve<Domain.Aldebaran.ITransitOrderService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranTransitOrderService.Get(attempts);
                _transitOrderSynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
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
                var data = _aldebaranTransitOrderService.Get(attempts);
                _transitOrderSynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}