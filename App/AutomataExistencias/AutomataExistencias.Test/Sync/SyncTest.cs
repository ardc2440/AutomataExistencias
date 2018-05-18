using System;
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
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _moneySynchronize.Sync();
                _unitMeasuredSynchronize.Sync();
                _lineSynchronize.Sync();
                _itemSynchronize.Sync();
                _itemByColorSynchronize.Sync();
                _transitOrderSynchronize.Sync();
                _stockSynchronize.Sync();
                _packagingSynchronize.Sync();
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
                _moneySynchronize.ReverseSync();
                _unitMeasuredSynchronize.ReverseSync();
                _lineSynchronize.ReverseSync();
                _itemSynchronize.ReverseSync();
                _itemByColorSynchronize.ReverseSync();
                _transitOrderSynchronize.ReverseSync();
                _stockSynchronize.ReverseSync();
                _packagingSynchronize.ReverseSync();
                _updateProcessSynchronize.UpdateProcess();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}