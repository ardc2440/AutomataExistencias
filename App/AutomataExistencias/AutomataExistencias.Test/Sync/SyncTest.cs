using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class SyncTest : AutofacConfigurator
    {
        private readonly IItemByColorSynchronize _itemByColorSynchronize;
        private readonly IItemSynchronize _itemSynchronize;
        private readonly ILineSynchronize _lineSynchronize;
        private readonly IMoneySynchronize _moneySynchronize;
        private readonly IPackagingSynchronize _packagingSynchronize;
        private readonly IStockSynchronize _stockSynchronize;
        private readonly ITransitOrderSynchronize _transitOrderSynchronize;
        private readonly IUnitMeasuredSynchronize _unitMeasuredSynchronize;
        private readonly IUpdateProcessSynchronize _updateProcessSynchronize;

        
        private readonly Domain.Aldebaran.IItemByColorService _aldebaranItemByColorService;
        private readonly Domain.Aldebaran.IItemService _aldebaranItemService;
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        private readonly Domain.Aldebaran.IMoneyService _aldebaranMoneyService;
        private readonly Domain.Aldebaran.IPackagingService _aldebaranPackagingService;
        private readonly Domain.Aldebaran.IStockService _aldebaranStockService;
        private readonly Domain.Aldebaran.ITransitOrderService _aldebaranTransitOrderService;
        private readonly Domain.Aldebaran.IUnitMeasuredService _aldebaranUnitMeasuredService;
        public SyncTest()
        {
            var container = GetContainer();
            _itemByColorSynchronize = container.Resolve<IItemByColorSynchronize>();
            _itemSynchronize = container.Resolve<IItemSynchronize>();
            _lineSynchronize = container.Resolve<ILineSynchronize>();
            _moneySynchronize = container.Resolve<IMoneySynchronize>();
            _packagingSynchronize = container.Resolve<IPackagingSynchronize>();
            _transitOrderSynchronize = container.Resolve<ITransitOrderSynchronize>();
            _unitMeasuredSynchronize = container.Resolve<IUnitMeasuredSynchronize>();
            _updateProcessSynchronize = container.Resolve<IUpdateProcessSynchronize>();
            _stockSynchronize = container.Resolve<IStockSynchronize>();

            _aldebaranItemByColorService = container.Resolve<Domain.Aldebaran.IItemByColorService>();
            _aldebaranItemService = container.Resolve<Domain.Aldebaran.IItemService>();
            _aldebaranLineService = container.Resolve<Domain.Aldebaran.ILineService>();
            _aldebaranMoneyService = container.Resolve<Domain.Aldebaran.IMoneyService>();
            _aldebaranPackagingService = container.Resolve<Domain.Aldebaran.IPackagingService>();
            _aldebaranStockService = container.Resolve<Domain.Aldebaran.IStockService>();
            _aldebaranTransitOrderService = container.Resolve<Domain.Aldebaran.ITransitOrderService>();
            _aldebaranUnitMeasuredService = container.Resolve<Domain.Aldebaran.IUnitMeasuredService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                _moneySynchronize.Sync(_aldebaranMoneyService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _unitMeasuredSynchronize.Sync(_aldebaranUnitMeasuredService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _lineSynchronize.Sync(_aldebaranLineService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _itemSynchronize.Sync(_aldebaranItemService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _itemByColorSynchronize.Sync(_aldebaranItemByColorService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _transitOrderSynchronize.Sync(_aldebaranTransitOrderService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _stockSynchronize.Sync(_aldebaranStockService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _packagingSynchronize.Sync(_aldebaranPackagingService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _updateProcessSynchronize.UpdateProcess();
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
                _moneySynchronize.ReverseSync(_aldebaranMoneyService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _unitMeasuredSynchronize.ReverseSync(_aldebaranUnitMeasuredService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _lineSynchronize.ReverseSync(_aldebaranLineService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _itemSynchronize.ReverseSync(_aldebaranItemService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _itemByColorSynchronize.ReverseSync(_aldebaranItemByColorService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _transitOrderSynchronize.ReverseSync(_aldebaranTransitOrderService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _stockSynchronize.ReverseSync(_aldebaranStockService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _packagingSynchronize.ReverseSync(_aldebaranPackagingService.Get(attempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
                _updateProcessSynchronize.UpdateProcess();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}