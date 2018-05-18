using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Test.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomataExistencias.Test.Sync
{
    [TestClass]
    public class UpdateProcessSyncTest: AutofacConfigurator
    {
        private readonly IUpdateProcessSynchronize _updateProcessSynchronize;

        public UpdateProcessSyncTest()
        {
            var container = GetContainer();
            _updateProcessSynchronize = container.Resolve<IUpdateProcessSynchronize>();
        }

        [TestMethod]
        public void UpdateProcess()
        {
            try
            {
                _updateProcessSynchronize.UpdateProcess();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}