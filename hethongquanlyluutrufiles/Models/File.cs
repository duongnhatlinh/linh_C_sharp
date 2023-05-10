using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace hethongquanlyluutrufiles.Models
{
    public partial class File
    {
        public File()
        {
            SharedFiles = new HashSet<SharedFile>();
        }

        public int FileId { get; set; }
        public int UserId { get; set; }
        [StringLength(100)]
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime DateUploaded { get; set; }
        public int TypeFileId { get; set; }
        public bool IsActive { get; set; }

        public virtual TypeFile TypeFile { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<SharedFile> SharedFiles { get; set; }
    }
}
