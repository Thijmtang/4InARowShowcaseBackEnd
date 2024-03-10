using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Contracts.Dto
{
    public class UserDTO
    {
        public string? Id { get; set; } = string.Empty;

        [Required]
        public string? Name { get; set; } = string.Empty;
    }
}
