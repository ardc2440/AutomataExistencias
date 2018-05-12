using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutomataExistencias.Domain.Cataprom;
using AutomataExistencias.Application;
using NLog;

namespace AutomataExistencias.Test
{
    [TestClass]
    public class SyncTest : AutofacConfigurator
    {
        private readonly ISynchronize _synchronize;
        private readonly Logger _logger;
        public SyncTest()
        {
            var container = GetContainer();
            _synchronize = container.Resolve<ISynchronize>();
            _logger = LogManager.GetCurrentClassLogger();
        }
        [TestMethod]
        public void Sync()
        {
            try
            {
                _logger.Info("SyncTest has started");
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
            finally
            {
                _logger.Info("SyncTest has finished");
            }
        }
        
    }
}
