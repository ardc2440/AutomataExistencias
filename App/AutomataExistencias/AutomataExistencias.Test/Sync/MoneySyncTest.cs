using System;
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

        public MoneySyncTest()
        {
            var container = GetContainer();
            _moneySynchronize = container.Resolve<IMoneySynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _moneySynchronize.Sync();
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
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}