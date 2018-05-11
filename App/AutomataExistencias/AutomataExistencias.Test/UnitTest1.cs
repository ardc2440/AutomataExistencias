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
    public class UnitTest1 : AutofacConfigurator
    {
        private readonly IBalanceService _balanceService;
        private readonly IStockItemService _stockItemService;
        private readonly ISynchronize _synchronize;
        public UnitTest1()
        {
            var container = GetContainer();
            _balanceService = container.Resolve<IBalanceService>();
            _stockItemService = container.Resolve<IStockItemService>();
            _synchronize = container.Resolve<ISynchronize>();
        }
        [TestMethod]
        public void AldebaranTest()
        {
            try
            {
                var data = _balanceService.Get().ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [TestMethod]
        public void CatapromTest()
        {
            try
            {
                var data = _stockItemService.Get().ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _synchronize.Sync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
