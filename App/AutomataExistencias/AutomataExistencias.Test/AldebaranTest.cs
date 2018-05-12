using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test
{
    [TestClass]
    public class AldebaranTest : AutofacConfigurator
    {
        private readonly Domain.Aldebaran.IMoneyService _aldebaranMoneyService;
        private readonly Domain.Aldebaran.IStockService _aldebaranStockService;
        private readonly Domain.Aldebaran.IItemService _aldebaranItemService;
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        private readonly Domain.Aldebaran.IItemByColorService _aldebaranItemByColorService;
        private readonly Domain.Aldebaran.ITransitOrderService _aldebaranTransitOrderService;
        private readonly Domain.Aldebaran.IUnitMeasuredService _aldebaranUnitMeasuredService;
        public AldebaranTest()
        {
            var container = GetContainer();
            _aldebaranMoneyService = container.Resolve<Domain.Aldebaran.IMoneyService>();
            _aldebaranStockService = container.Resolve<Domain.Aldebaran.IStockService>();
            _aldebaranItemService = container.Resolve<Domain.Aldebaran.IItemService>();
            _aldebaranLineService = container.Resolve<Domain.Aldebaran.ILineService>();
            _aldebaranItemByColorService = container.Resolve<Domain.Aldebaran.IItemByColorService>();
            _aldebaranTransitOrderService = container.Resolve<Domain.Aldebaran.ITransitOrderService>();
            _aldebaranUnitMeasuredService = container.Resolve<Domain.Aldebaran.IUnitMeasuredService>();
        }

        [TestMethod]
        public void GetTestMethod()
        {
            try
            {
                var a = _aldebaranMoneyService.Get().ToList();
                var b = _aldebaranStockService.Get().ToList();
                var c = _aldebaranItemService.Get().ToList();
                var d = _aldebaranLineService.Get().ToList();
                var e = _aldebaranItemByColorService.Get().ToList();
                var f = _aldebaranTransitOrderService.Get().ToList();
                var g = _aldebaranUnitMeasuredService.Get().ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
