using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class MoneySyncTest : AutofacConfigurator
    {
        private readonly IMoneySynchronize _moneySynchronize;
        private readonly Domain.Aldebaran.IMoneyService _aldebaranMoneyService;
        public MoneySyncTest()
        {
            var container = GetContainer();
            _moneySynchronize = container.Resolve<IMoneySynchronize>();
            _aldebaranMoneyService = container.Resolve<Domain.Aldebaran.IMoneyService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranMoneyService.Get(attempts);
                _moneySynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
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
                var data = _aldebaranMoneyService.Get(attempts);
                _moneySynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}