using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test
{
    [TestClass]
    public class CatapromTest: AutofacConfigurator
    {
        private readonly Domain.Cataprom.IMoneyService _catapromMoneyService;
        public CatapromTest()
        {
            var container = GetContainer();
            _catapromMoneyService = container.Resolve<Domain.Cataprom.IMoneyService>();
        }
        [TestMethod]
        public void GetTestMethod()
        {
            try
            {
                var a = _catapromMoneyService.Get().ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
