using System;
using System.Linq;
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
        private readonly Domain.Aldebaran.IItemService _aldebaranItemService;
        public ItemsSyncTest()
        {
            var container = GetContainer();
            _itemSynchronize = container.Resolve<IItemSynchronize>();
            _aldebaranItemService = container.Resolve<Domain.Aldebaran.IItemService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranItemService.Get(attempts);
                _itemSynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
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
                var data = _aldebaranItemService.Get(attempts);
                _itemSynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}