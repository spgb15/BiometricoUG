using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Application.Interfaces
{
    public interface IZKProcessingService
    {
        Task ProcesarTextoPlano(string rawBody, string SerialNumber);
    }
}
