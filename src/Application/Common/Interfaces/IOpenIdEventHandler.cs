using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IOpenIdEventHandler
{

    public Task OnSignIn(string userName);
    public Task OnSignOut(string userName);

}
