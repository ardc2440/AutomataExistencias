using System;
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

        public TransitOrderSyncTest(ITransitOrderSynchronize transitOrderSynchronize)
        {
            var container = GetContainer();
            _transitOrderSynchronize = container.Resolve<ITransitOrderSynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _transitOrderSynchronize.Sync();
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
                _transitOrderSynchronize.ReverseSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}