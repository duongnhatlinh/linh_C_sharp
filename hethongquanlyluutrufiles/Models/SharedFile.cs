using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace hethongquanlyluutrufiles.Models
{
    public partial class SharedFile
    {
        public int SharedFileId { get; set; }
        public int FileId { get; set; }
        public int SharedWithUserId { get; set; }
        public DateTime DateShared { get; set; }
        [StringLength(300)]
        public string Message { get; set; }
        public bool Notify { get; set; }

        public virtual File File { get; set; }
        public virtual User SharedWithUser { get; set; }
    }
}
