using EonaCat.Linux.UFW.Models;
using System.Collections.Generic;
namespace EonaCat.UFW.Manager.Models
{
    public class RuleViewModel
    {
        public List<EonaCat.Linux.UFW.Models.Rule> Rules { get; set; }
        public bool IsActive { get; internal set; }
    }
}
