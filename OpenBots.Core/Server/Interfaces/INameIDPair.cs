using System;
using System.ComponentModel.DataAnnotations;

namespace OpenBots.Server.Model.Core
{
    public interface INameIDPair
    {
        [Display(Name = "Id")]
        Guid? Id { get; set; }

        [Display(Name = "Name")]
        string Name { get; set; }
    }
}
