using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Service;

public class CreateServiceRequest
{
    public string Name { get; set; } = null!;
}