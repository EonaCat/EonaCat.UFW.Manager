using EonaCat.Linux.UFW.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EonaCat.UFW.Manager.Models
{
    public class Rule
    {
        public int RuleIndex { get; set; }

        [Required]
        public RuleType Type { get; set; }
        public RuleOperation Operation { get; set; }
        public RuleProtocol Protocol { get; set; }

        [Required]
        public int Port { get; set; }

        public string From { get; set; }

        public SourceType SourceType { get; set; }

        public string Source { get; set; }
    }
}
