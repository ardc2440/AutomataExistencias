using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class ItemsByColorSyncTest : AutofacConfigurator
    {
        private readonly IItemByColorSynchronize _itemByColorSynchronize;
        private readonly Domain.Aldebaran.IItemByColorService _aldebaranItemByColorService;
        public ItemsByColorSyncTest()
        {
            var container = GetContainer();
            _itemByColorSynchronize = container.Resolve<IItemByColorSynchronize>();
            _aldebaranItemByColorService = container.Resolve<Domain.Aldebaran.IItemByColorService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranItemByColorService.Get(attempts);
                _itemByColorSynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
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
                var data = _aldebaranItemByColorService.Get(attempts);
                _itemByColorSynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}