using System;
using System.Collections.Generic;
using System.Text;

namespace KernelBlazeMind.Abstraction.Authentications
{
    public interface IModelEndpointConfiguration
    {

        string this[string key] { get; set; }



        string Name { get; }

        string BaseUri { get; set; }
    }

}
