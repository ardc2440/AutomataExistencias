using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class ItemsByColorSyncTest: AutofacConfigurator
    {
        private readonly IItemByColorSynchronize _itemByColorSynchronize;

        public ItemsByColorSyncTest()
        {
            var container = GetContainer();
            _itemByColorSynchronize = container.Resolve<IItemByColorSynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _itemByColorSynchronize.Sync();
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
                _itemByColorSynchronize.ReverseSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}