using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace hethongquanlyluutrufiles.Models
{
    public partial class User
    {
        public User()
        {
            Files = new HashSet<File>();
            SharedFiles = new HashSet<SharedFile>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IsAdminOrUser { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Cmnd { get; set; }
        public int? RoleId { get; set; }
        public int? DepartmentId { get; set; }
        [Required]
        public string Typework { get; set; }
        public string Company { get; set; }
        public string Workplace { get; set; }
        public bool IsActive { get; set; }
        public bool AbleShared { get; set; }

        public virtual Department Department { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<SharedFile> SharedFiles { get; set; }
    }
}
