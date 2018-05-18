using System;
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

        public StockSyncTest()
        {
            var container = GetContainer();
            _stockSynchronize = container.Resolve<IStockSynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _stockSynchronize.Sync();
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
                _stockSynchronize.ReverseSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}