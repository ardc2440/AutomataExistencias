using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class UnitMeasuredSyncTest: AutofacConfigurator
    {
        private readonly IUnitMeasuredSynchronize _unitMeasuredSynchronize;

        public UnitMeasuredSyncTest()
        {
            var container = GetContainer();
            _unitMeasuredSynchronize = container.Resolve<IUnitMeasuredSynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _unitMeasuredSynchronize.Sync();
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
                _unitMeasuredSynchronize.ReverseSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}