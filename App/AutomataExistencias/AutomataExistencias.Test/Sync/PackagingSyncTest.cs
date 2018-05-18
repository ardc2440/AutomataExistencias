using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class PackagingSyncTest: AutofacConfigurator
    {
        private readonly IPackagingSynchronize _packagingSynchronize;

        public PackagingSyncTest()
        {
            var container = GetContainer();
            _packagingSynchronize = container.Resolve<IPackagingSynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _packagingSynchronize.Sync();
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
                _packagingSynchronize.ReverseSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}