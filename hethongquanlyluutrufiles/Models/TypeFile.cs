using System;
using System.Collections.Generic;

#nullable disable

namespace hethongquanlyluutrufiles.Models
{
    public partial class TypeFile
    {
        public TypeFile()
        {
            Files = new HashSet<File>();
        }

        public int TypeFileId { get; set; }
        public string TypeFileName { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }
}
