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
        public void Sync()
        {
            try
            {
                _synchronize.MoneySync();
                _synchronize.UnitMeasuredSync();
                _synchronize.LinesSync();
                _synchronize.ItemsSync();
                _synchronize.ItemsByColorSync();
                _synchronize.TransitOrderSync();
                _synchronize.StockSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
    }
}
