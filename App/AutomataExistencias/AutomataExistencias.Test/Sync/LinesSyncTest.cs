using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class LinesSyncTest: AutofacConfigurator
    {
        private readonly ILineSynchronize _lineSynchronize;

        public LinesSyncTest()
        {
            var container = GetContainer();
            _lineSynchronize = container.Resolve<ILineSynchronize>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                _lineSynchronize.Sync();
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
                _lineSynchronize.ReverseSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}