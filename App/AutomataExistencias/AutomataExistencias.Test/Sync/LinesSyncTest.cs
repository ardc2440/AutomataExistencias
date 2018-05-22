using System;
using System.Linq;
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
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        public LinesSyncTest()
        {
            var container = GetContainer();
            _lineSynchronize = container.Resolve<ILineSynchronize>();
            _aldebaranLineService = container.Resolve<Domain.Aldebaran.ILineService>();
        }

        [TestMethod]
        public void Sync()
        {
            try
            {
                var attempts = 2;
                var data = _aldebaranLineService.Get(attempts);
                _lineSynchronize.Sync(data.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), attempts);
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
                var data = _aldebaranLineService.Get(attempts);
                _lineSynchronize.ReverseSync(data.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), attempts);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}