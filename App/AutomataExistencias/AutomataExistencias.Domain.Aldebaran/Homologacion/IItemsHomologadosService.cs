using AutomataExistencias.DataAccess.Aldebaran.Homologacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataExistencias.Domain.Aldebaran.Homologacion
{
    public interface IItemsHomologadosService
    {
        ItemHomologado GetById(int id);
    }
}
