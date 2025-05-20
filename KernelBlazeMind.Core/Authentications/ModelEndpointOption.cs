using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace KernelBlazeMind.Abstraction.Authentications
{
    public class ModelEndpointOption
    {
         
        public string Name { get; set; } =default!;
        public string BaseUri { get; set; } = default!;
    }

}
