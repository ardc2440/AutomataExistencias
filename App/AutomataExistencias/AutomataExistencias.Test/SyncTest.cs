using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutomataExistencias.Domain.Cataprom;
using AutomataExistencias.Application;

namespace AutomataExistencias.Test
{
    [TestClass]
    public class SyncTest : AutofacConfigurator
    {
        private readonly ISynchronize _synchronize;
        public SyncTest()
        {
            var container = GetContainer();
            _synchronize = container.Resolve<ISynchronize>();
        }
        [TestMethod]
        public void ItemsByColorSyncTest()
        {
            try
            {
                _synchronize.ItemsByColorSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [TestMethod]
        public void ItemsSyncTest()
        {
            try
            {
                _synchronize.ItemsSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [TestMethod]
        public void LinesSyncTest()
        {
            try
            {
                _synchronize.LinesSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [TestMethod]
        public void MoneySyncTest()
        {
            try
            {
                _synchronize.MoneySync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [TestMethod]
        public void StockSyncTest()
        {
            try
            {
                _synchronize.StockSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [TestMethod]
        public void TransitOrderSyncTest()
        {
            try
            {
                _synchronize.TransitOrderSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [TestMethod]
        public void UnitMeasuredSyncTest()
        {
            try
            {
                _synchronize.UnitMeasuredSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
