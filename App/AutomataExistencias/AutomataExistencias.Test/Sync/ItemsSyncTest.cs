using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class ItemsSyncTest: AutofacConfigurator
    {
        private readonly IItemSynchronize _itemSynchronize;

        public ItemsSyncTest()
        {
            var container = GetContainer();
            _itemSynchronize = container.Resolve<IItemSynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _itemSynchronize.Sync();
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
                _itemSynchronize.ReverseSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}